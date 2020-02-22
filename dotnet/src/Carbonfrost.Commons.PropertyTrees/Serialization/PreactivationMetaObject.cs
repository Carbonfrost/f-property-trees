//
// Copyright 2014, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class PreactivationMetaObject : PropertyTreeMetaObject {

        private Type componentType;
        private OperatorDefinition factoryDefinition;

        public PreactivationMetaObject(Type componentType) {
            this.componentType = componentType;
        }

        public override Type ComponentType {
            get {
                return this.componentType;
            }
        }

        public override object Component {
            get {
                return null;
            }
        }

        public OperatorDefinition FactoryOperator {
            get {
                return factoryDefinition
                    ?? PropertyTreeDefinition.FromType(this.ComponentType).Constructor;
            }
        }

        public override PropertyTreeMetaObject BindInitializeValue(string text, IServiceProvider serviceProvider) {
            object value = text;
            if (ComponentType != typeof(string) && ComponentType != typeof(object)) {
                TryConvertFromText(text, serviceProvider, out value);
            }
            return PropertyTreeMetaObject.Create(value, this.ComponentType);
        }

        public override PropertyTreeMetaObject BindInitializer(Expression expression, IExpressionContext context, IServiceProvider serviceProvider) {
            // No need to evaluate if the parent can accept expressions
            // TODO There will be other conditions for allowing expressions
            if (CanAllowExpressionInitializer(Parent)) {
                return PropertyTreeMetaObject.Create(expression);
            }

            var result = expression.Evaluate(context ?? ExpressionContext.Empty);

            if (result == Undefined.Value) {
                return PropertyTreeMetaObject.Null;
            }
            var items = result as object[];
            if (items != null) {
                if (items.Length == 0 || items.All(t => t == Undefined.Value)) {
                    return PropertyTreeMetaObject.Null;
                }
                result = string.Concat(items);
            }

            return BindInitializeValue(result.ToString(), serviceProvider);
        }

        public override PropertyTreeMetaObject BindConstructor(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            var op = definition;
            object component = null;
            object parent = null;

            var value = op.Apply(component, parent, arguments);
            if (this.Parent == null)
                return Create(value);
            else
                return this.Parent.CreateChild(value);
        }

        public override PropertyTreeMetaObject BindTargetProvider(QualifiedName name, object criteria, IServiceProvider serviceProvider) {
            var member = FindProviderMember(name, criteria);
            if (member == null) {
                ProbeRuntimeComponents();
                member = FindProviderMember(name, criteria);
            }

            return SetProviderMember(member);
        }

        private MemberInfo FindProviderMember(QualifiedName name, object criteria) {
            MemberInfo member = null;
            if (name != null)
                member = App.GetProviderMember(this.ComponentType, name);
            else
                member = App.GetProviderMember(this.ComponentType, criteria);
            if (name != null && name.Namespace.IsDefault) {
                member = member ?? App.GetProviderMember(this.ComponentType, name.LocalName);
            }
            return member;
        }

        public override PropertyTreeMetaObject BindStreamingSource(StreamContext input, IServiceProvider serviceProvider) {
            var ss = StreamingSource.Create(this.ComponentType);
            var comp = ss.Load(input, this.ComponentType);
            return PropertyTreeMetaObject.Create(comp);
        }

        internal PropertyTreeMetaObject SetProviderMember(MemberInfo member) {
            if (member == null)
                return this;

            if (member.MemberType == MemberTypes.TypeInfo) {
                this.componentType = ((TypeInfo) member).AsType();
                return this;

            } else if (member.MemberType == MemberTypes.Method) {
                this.factoryDefinition = ReflectedProviderFactoryDefinitionBase.Create(this.componentType, member);
                this.componentType = ((MethodInfo) member).ReturnType;
                return this;

            } else {
                var component = ((FieldInfo) member).GetValue(null);
                return this.Parent.CreateChild(component);
            }
        }

        public override void BindTargetType(TypeReference type, IServiceProvider serviceProvider) {
            // TODO Should only allow changing this component type via a target type bind once

            var newType = type.TryResolve();
            if (newType == null) {
                ProbeRuntimeComponents();

                newType = type.Resolve();
            }

            if (this.componentType.IsAssignableFrom(newType)) {
                this.componentType = newType;

            } else {
                throw Failure.NotAssignableFrom(newType, this.componentType);
            }

        }

        private static bool CanAllowExpressionInitializer(PropertyTreeMetaObject parent) {
            return parent.IsExpressionBagOrApply;
        }
    }
}
