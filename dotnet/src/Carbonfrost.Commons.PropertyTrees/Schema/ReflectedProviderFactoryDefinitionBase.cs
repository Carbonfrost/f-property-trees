//
// Copyright 2010 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Reflection;

using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    abstract class ReflectedProviderFactoryDefinitionBase : OperatorDefinition {

        private readonly Type outputType;
        private readonly QualifiedName qname;
        private readonly PropertyDefinitionCollection parameters;
        private readonly MethodBase method;

        protected ReflectedProviderFactoryDefinitionBase(MethodBase method,
                                                         QualifiedName qname,
                                                         Type outputType) {
            this.qname = qname;
            this.outputType = outputType;
            this.method = method;

            this.parameters = new PropertyDefinitionCollection();
            parameters.AddRange(this, qname.NamespaceName, method.GetParameters(), method.IsExtension());
        }

        public sealed override Type OutputType {
            get { return this.outputType; } }

        public override OperatorType OperatorType {
            get { return OperatorType.Add; } }

        public sealed override PropertyDefinitionCollection Parameters {
            get { return this.parameters; } }

        public sealed override MethodBase UnderlyingMethod {
            get { return this.method; } }

        public override string Namespace {
            get { return qname.NamespaceName; }
        }

        public override string Name {
            get { return qname.LocalName; }
        }

        public override PropertyDefinition DefaultParameter {
            get {
                return null;
            }
        }

        public static ReflectedProviderFactoryDefinitionBase Create(Type providerType, MemberInfo concrete, QualifiedName name = null) {
            if (name == null) {
                name = App.GetProviderName(providerType, concrete);
            }
            if (concrete.MemberType == MemberTypes.TypeInfo)
                return new ProviderTypeFactoryDefinition(((TypeInfo) concrete).AsType(), name);

            else if (concrete.MemberType == MemberTypes.Method)
                return new ProviderMethodFactoryDefinition((MethodInfo) concrete, name);

            // TODO Should fieldInfo be available here?
            return null;
        }

        private object DoAddChild(object parent, object child) {
            var addChild = parent as IAddChild;
            if (addChild != null)
                addChild.AddChild(child);

            return child;
        }

        sealed class ProviderTypeFactoryDefinition : ReflectedProviderFactoryDefinitionBase {

            public ProviderTypeFactoryDefinition(Type concreteProviderType, QualifiedName name)
                : base(concreteProviderType.GetActivationConstructor(),
                       name,
                       concreteProviderType)
            {
            }

            public override object Apply(object component,
                                         object parent,
                                         IReadOnlyDictionary<string, object> parameters) {

                var parms = MapParameters(UnderlyingMethod, parent, parameters);
                return DoAddChild(parent, ((ConstructorInfo) UnderlyingMethod).Invoke(parms));
            }
        }

        sealed class ProviderMethodFactoryDefinition : ReflectedProviderFactoryDefinitionBase {

            public ProviderMethodFactoryDefinition(MethodInfo providerFactoryMethod, QualifiedName name)
                : base(providerFactoryMethod,
                       name,
                       providerFactoryMethod.ReturnType)
            {
            }

            public override object Apply(object component,
                                         object parent,
                                         IReadOnlyDictionary<string, object> parameters) {

                var parms = MapParameters(UnderlyingMethod, parent, parameters);
                return DoAddChild(parent, UnderlyingMethod.Invoke(null, parms));
            }

        }
    }

}
