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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class ReflectionMetaObject : PropertyTreeMetaObject {

        private readonly object component;
        private readonly Type componentType;

        public ReflectionMetaObject(object component, Type componentType) {
            this.componentType = componentType;
            this.component = component;
        }

        public override Type ComponentType {
            get { return componentType; }
        }

        public override object Component {
            get {
                return component;
            }
        }

        internal override void ApplyUriContextToProperties(Uri baseUri) {
            if (Component != null) {
                foreach (var prop in PropertyTreeDefinition.FromType(ComponentType).UriContextCache) {
                    object value;
                    if (prop.TryGetValue(Component, null, prop.QualifiedName, out value) && value != null) {
                        ((IUriContext) value).BaseUri = baseUri;
                    }
                }
            }
        }

        public override PropertyTreeMetaObject BindAddChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            var result = definition.Apply(null, component, arguments);
            if (result == null)
                return Null;
            return CreateChild(result);
        }

        public override PropertyTreeMetaObject BindBaseUri(Uri baseUri, IServiceProvider serviceProvider) {
            var uc = Component as IUriContext;
            if (uc != null) {
                uc.BaseUri = baseUri;
            }

            return this;
        }

        public override PropertyTreeMetaObject BindFileLocation(int lineNumber, int linePosition, IServiceProvider serviceProvider) {
            var uc = Component as IFileLocationConsumer;
            if (uc != null) {
                uc.SetFileLocation(lineNumber, linePosition);
            }
            return this;
        }

        public override PropertyTreeMetaObject BindStreamingSource(StreamContext input, IServiceProvider serviceProvider) {
            var binder = serviceProvider.GetRequiredService<PropertyTreeMetaObjectBinder>();
            var ss = binder.CreateStreamingSource(this, input, serviceProvider);

            if (ss == null) {
                var errors = serviceProvider.GetServiceOrDefault(PropertyTreeBinderErrors.Default);
                errors.CouldNotBindStreamingSource(this.ComponentType, PropertyTreeBinderImpl.FindFileLocation(serviceProvider));
                return this;
            }

            // Hydrate the existing instance
            ss.Load(input, this.Component);
            return this;
        }

        public override void BindClearChildren(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            definition.Apply(null, component, arguments);
        }

        public override void BindRemoveChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            definition.Apply(null, component, arguments);
        }

        public override void BindSetMember(PropertyDefinition property, QualifiedName name, PropertyTreeMetaObject value, PropertyTreeMetaObject ancestor, IServiceProvider serviceProvider) {
            if (TryAggregation(value, name, property, serviceProvider)) {
                return;
            }

            if (property.IsReadOnly) {
                return;
            }

            object outputValue = value.Component;
            object anc = ancestor == null ? null : ancestor.Component;

            // In the rare case where an expression is being set, downgrade to IProperties
            var comp = value.Component;
            if (!property.PropertyType.IsInstanceOfType(comp) && IsExpressionBag) {
                property = GetDefinition().DefaultProperties.OfType<IndexerUsingIPropertiesPropertyDefinition>().First();
            }
            property.SetValue(component, anc, name, outputValue);
        }

        static IEnumerable<Type> GetBestItemTypes(IEnumerable enumerable) {
            var enumerableTypes = enumerable.Cast<object>().Where(t => !ReferenceEquals(t, null)).Select(t => t.GetType()).Distinct();

            if (enumerableTypes.Count() == 1)
                yield return enumerableTypes.First();

            var available = Utility.EnumerateInheritedTypes(enumerableTypes.First());
            foreach (var type in available) {
                if (enumerableTypes.All(type.IsAssignableFrom))
                    yield return type;
            }
        }

        internal static IEnumerable GetAddonElements(IEnumerable enumerable) {
            var result = new List<object>();
            foreach (var o in enumerable) {
                if (o is string) {
                    if (!string.IsNullOrWhiteSpace(o.ToString())) {
                        return TryToStringSingleton(enumerable);
                    } else {
                        continue;
                    }
                }
                result.Add(o);
            }
            return result;
        }

        static IEnumerable TryToStringSingleton(IEnumerable enumerable) {
            var e = enumerable.Cast<object>();
            if (e.All(t => t is string)) {
                return new[] { string.Concat(e).Trim() };
            }
            return enumerable;
        }

        internal static MethodInfo FindAddonMethod(Type type, IEnumerable enumerable) {
            var bestItemTypes = GetBestItemTypes(enumerable);

            foreach (var itemType in bestItemTypes) {
                var result = type.GetMethod("Add", new [] { itemType });
                if (result != null)
                    return result;
            }

            return null;
        }

        private bool TryAggregation(PropertyTreeMetaObject value,
                                    QualifiedName name,
                                    PropertyDefinition property,
                                    IServiceProvider serviceProvider)
        {
            object current;
            if (!property.TryGetValue(component, null, name, out current)) {
                return false;
            }

            var enumerable = value.Component as IEnumerable;
            if (current != null && enumerable != null) {

                var items = enumerable;
                if (!ReferenceEquals(current, items) && enumerable.GetEnumerator().MoveNext()) {
                    MethodInfo mi = FindAddonMethod(current.GetType(), enumerable);

                    if (mi == null) {
                        // Error because aggregation will be needed on read-only properties
                        if (property.IsReadOnly) {
                            var errors = serviceProvider.GetServiceOrDefault(PropertyTreeBinderErrors.Default);
                            errors.NoAddMethodSupported(component.GetType(), PropertyTreeBinderImpl.FindFileLocation(serviceProvider));
                        }
                        return false;
                    }

                    foreach (var item in items) {
                        mi.Invoke(current, new object[] { item });
                    }
                    // Success because aggregation was applied
                    return true;
                }
            }
            return false;
        }

        public override PropertyTreeMetaObject BindInitializeValue(string text, IServiceProvider serviceProvider) {
            object value;
            TryConvertFromText(text, serviceProvider, out value);
            return PropertyTreeMetaObject.Create(value);
        }

        public override PropertyTreeMetaObject BindInitializer(Expression expression, ExpressionContext context, IServiceProvider serviceProvider) {
            if (Parent.IsExpressionBag) {
                return PropertyTreeMetaObject.Create(expression);
            }
            object value = expression.Evaluate(context ?? ExpressionContext.Empty);
            return PropertyTreeMetaObject.Create(value);
        }
    }

}
