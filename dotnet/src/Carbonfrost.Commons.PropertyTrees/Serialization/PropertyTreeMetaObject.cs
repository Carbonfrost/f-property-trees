//
// Copyright 2014, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Reflection;

using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    public abstract class PropertyTreeMetaObject {

        public static readonly PropertyTreeMetaObject Null
            = new NullMetaObject();

        private static readonly IPropertyNameLookupHelper propertyLookup
            = new PropertyNameLookupHelper();

        private bool _probedComponents;

        internal IEnumerable<PropertyTreeMetaObject> Ancestors {
            get {
                var current = Parent;
                while (current != null) {
                    yield return current;
                    current = current.Parent;
                }
            }
        }

        internal IEnumerable<object> AncestorComponents {
            get {
                return Ancestors.Select(t => t.Component);
            }
        }

        public abstract Type ComponentType {
            get;
        }

        public abstract object Component {
            get;
        }

        public PropertyTreeMetaObject Root {
            get {
                if (Parent == null)
                    return this;

                return Parent.Root;
            }
        }

        internal bool IsExpressionBagOrApply {
            get {
                return IsExpressionBag || typeof(IApplyProperties).IsAssignableFrom(ComponentType);
            }
        }

        internal bool IsExpressionBag {
            get {
                return typeof(IProperties).IsAssignableFrom(ComponentType)
                    || typeof(IPropertiesContainer).IsAssignableFrom(ComponentType);
            }
        }

        internal virtual bool ShouldBindChildren {
            get {
                return Component != null;
            }
        }

        internal virtual bool ShouldConstruct {
            get {
                return Component == null;
            }
        }

        public PropertyTreeMetaObject Parent {
            get;
            private set;
        }

        protected PropertyTreeMetaObject() {}

        protected PropertyTreeMetaObject(PropertyTreeMetaObject parent) {
            this.Parent = parent;
        }

        internal virtual void ApplyUriContextToProperties(Uri baseUri) {
        }

        public virtual PropertyTreeMetaObject BindEndObject(IServiceProvider serviceProvider) {
            return this;
        }

        public virtual PropertyTreeMetaObject BindBaseUri(Uri baseUri, IServiceProvider serviceProvider) {
            return this;
        }

        public virtual PropertyTreeMetaObject BindFileLocation(int lineNumber, int linePosition, IServiceProvider serviceProvider) {
            return this;
        }

        public virtual PropertyTreeMetaObject BindAddChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            return null;
        }

        public virtual void BindRemoveChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
        }

        public virtual void BindClearChildren(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
        }

        public virtual PropertyTreeMetaObject BindInitializeValue(string text, IServiceProvider serviceProvider) {
            object value;
            TryConvertFromText(text, serviceProvider, out value);
            return PropertyTreeMetaObject.Create(value);
        }

        public virtual PropertyTreeMetaObject BindInitializer(Expression expression, ExpressionContext context, IServiceProvider serviceProvider) {
            return this;
        }

        public virtual void BindSetMember(PropertyDefinition property, QualifiedName name, PropertyTreeMetaObject value, PropertyTreeMetaObject ancestor, IServiceProvider serviceProvider) {
        }

        // TODO This may become API soon
        internal virtual PropertyTreeMetaObject BindGenericParameters(IEnumerable<Type> types) {
            return this;
        }

        internal PropertyTreeMetaObject BindTargetProvider(TargetProviderDirective binder, IServiceProvider serviceProvider) {
            return BindTargetProvider(binder.Name, binder, serviceProvider);
        }

        public virtual PropertyTreeMetaObject BindTargetProvider(QualifiedName name, object criteria, IServiceProvider serviceProvider) {
            return this;
        }

        internal PropertyTreeMetaObject BindStreamingSource(TargetSourceDirective targetSourceBinder, IServiceProvider serviceProvider) {
            return BindStreamingSource(targetSourceBinder.GetStreamingContext(), serviceProvider);
        }

        public virtual PropertyTreeMetaObject BindStreamingSource(StreamContext input, IServiceProvider serviceProvider) {
            return this;
        }

        internal void BindTargetType(TargetTypeDirective e, IServiceProvider serviceProvider) {
            BindTargetType(e.Type, serviceProvider);
        }

        public virtual void BindTargetType(TypeReference type, IServiceProvider serviceProvider) {
        }

        public virtual PropertyTreeMetaObject BindConstructor(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            return this;
        }

        public static PropertyTreeMetaObject Create(object component, Type componentType) {
            if (component == null && componentType == null) {
                throw PropertyTreesFailure.PropertyTreeMetaObjectComponentNull();
            }

            componentType = componentType ?? component.GetType();
            if (component != null && !componentType.IsInstanceOfType(component)) {
                throw new NotImplementedException();
            }

            if (component is System.Collections.ICollection)
                return new CollectionMetaObject(component);

            return new ReflectionMetaObject(component, componentType);
        }

        public static PropertyTreeMetaObject Create(object component) {
            if (component == null)
                throw new ArgumentNullException("component");

            return Create(component, component.GetType());
        }

        public static PropertyTreeMetaObject Create(Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            if (componentType == typeof(Type))
                return new TypeMetaObject();

            var ct = componentType.GetTypeInfo();
            if (ct.IsGenericType && ct.GetGenericTypeDefinition() == typeof(Template<>)) {
                return TemplateMetaObject.FromTemplateType(componentType);
            }

            if (componentType == typeof(ITemplate)) {
                return new UntypedToTypedMetaObject(typeof(ITemplate),
                                                    t => TemplateMetaObject.FromInstanceType(null, t.Single()));
            }

            return new PreactivationMetaObject(componentType);
        }

        public virtual PropertyTreeMetaObject CreateChild(Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            var result = Create(componentType);
            result.Parent = this;
            return result;
        }

        public virtual PropertyTreeMetaObject CreateChild(object component, Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            var result = Create(component, componentType);
            result.Parent = this;
            return result;
        }

        public PropertyTreeMetaObject CreateChild(object component) {
            if (component == null)
                throw new ArgumentNullException("component");

            return CreateChild(component, component.GetType());
        }

        public virtual PropertyTreeMetaObject CreateChild(PropertyDefinition property,
                                                          QualifiedName name,
                                                          PropertyTreeMetaObject ancestor)
        {
            object value;
            property.TryGetValue(this.Component, ancestor, name, out value);
            var propertyType = property.PropertyType;

            // Refine property type if possible
            if (value == null) {
                return CreateChild(propertyType);
            }
            else {
                return CreateChild(value, propertyType);
            }
        }

        internal bool TryConvertFromText(string text,
                                         IServiceProvider serviceProvider,
                                         out object result)
        {
            // TODO In some cases, this will be treated as if a streaming source hydration
            Type neededType = this.ComponentType;
            PropertyInfo property = null;

            var context = serviceProvider.GetServiceOrDefault<IValueSerializerContext>();
            if (context != null) {
                property = context.Property;

                // Get type from property providers
                var pp = context.Instance as IPropertyProvider;
                if (pp != null && property != null) {
                    neededType = property == null ? typeof(string) : (pp.GetPropertyType(property.Name) ?? property.PropertyType);
                }
            }

            // Apply concrete classes
            var cp = neededType.GetConcreteClass() ?? neededType;
            var conv = TypeHelper.GetConverter(property, cp);
            // TODO Could be a culture-specific vonersion
            result = conv.ConvertFromString(text, cp, context);
            return true;
        }

        internal PropertyTreeDefinition GetDefinition() {
            object component = this.Component;

            if (component != null)
                return PropertyTreeDefinition.FromValue(component);

            else if (this.ComponentType != null)
                return PropertyTreeDefinition.FromType(this.ComponentType);

            else
                return null;

        }

        internal PropertyNodeDefinition SelectMember(QualifiedName qn) {
            return SelectProperty(qn) ?? (PropertyNodeDefinition) SelectOperator(qn);
        }

        internal PropertyDefinition SelectProperty(QualifiedName qn, bool allowDefaultProperties = true) {
            var result = SelectPropertyCore(qn);

            if (result == null && ProbeRuntimeComponents()) {
                result = SelectPropertyCore(qn);
            }

            var target = Component;
            if (result != null) {
                return result;
            }

            if (allowDefaultProperties && target != null) {
                return GetDefinition().DefaultProperties.FirstOrDefault(p => p.TryGetValue(target, qn));
            }

            return null;
        }

        private PropertyDefinition SelectPropertyCore(QualifiedName qn) {
            return propertyLookup.FindProperty(GetDefinition(),
                                               this.ComponentType,
                                               qn,
                                               Ancestors.Select(t => t.GetDefinition())
                                              );
        }

        // TODO According to spec, dependencies should be enumerated first, which should mean we don't probe after they have been
        internal bool ProbeRuntimeComponents() {
            if (_probedComponents) {
                return false;
            }

            // TODO Support probing dependencies
            _probedComponents = true;
            return false;
        }

        internal virtual OperatorDefinition SelectOperator(QualifiedName qn) {
            var treeDef = PropertyTreeDefinition.FromType(this.ComponentType);
            var factory = propertyLookup.FindOperator(treeDef, this.ComponentType, qn);

            if (factory == null && ProbeRuntimeComponents()) {

                factory = propertyLookup.FindOperator(treeDef, this.ComponentType, qn);
            }
            return factory;
        }

    }

}
