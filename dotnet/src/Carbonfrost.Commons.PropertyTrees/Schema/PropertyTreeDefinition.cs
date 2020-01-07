
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
using System.Linq;

using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public abstract class PropertyTreeDefinition : PropertyNodeDefinition {

        private readonly static IDictionary<Type, PropertyTreeDefinition> map = new Dictionary<Type, PropertyTreeDefinition>();

        public abstract OperatorDefinitionCollection Operators { get; }
        public abstract OperatorDefinition Constructor { get; }
        public abstract PropertyDefinitionCollection Properties { get; }
        public abstract IEnumerable<PropertyDefinition> DefaultProperties { get; }
        public abstract Type SourceClrType { get; }
        public abstract PropertyTreeSchema Schema { get; }

        internal IEnumerable<PropertyDefinition> UriContextCache {
            get {
                return Properties.Where(t => typeof(IUriContext).GetTypeInfo().IsAssignableFrom(t.PropertyType));
            }
        }

        public IEnumerable<PropertyTreeDefinition> BaseTypes {
            get {
                return Utility.EnumerateInheritedTypes(this.SourceClrType)
                    .Select(t => FromType(t));
            }
        }

        public static PropertyTreeDefinition FromType(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            // TODO Optimize: Don't create defs for types that wouldn't be usable.
            return map.GetValueOrCache(type, t => new ReflectedPropertyTreeDefinition(t));
        }

        private static PropertyTreeDefinition SafeFromType(Type type) {
            if (type == null || type.IsGenericParameter)
                return null;

            return FromType(type);
        }

        public static PropertyTreeDefinition FromValue(object component) {
            if (component == null)
                throw new ArgumentNullException("component");

            // TODO Handle using TypeDescriptor
            return FromType(component.GetType());
        }

        public abstract IEnumerable<OperatorDefinition> EnumerateOperators(bool declaredOnly = false);
        public abstract IEnumerable<PropertyDefinition> EnumerateProperties(bool declaredOnly = false);

        public OperatorDefinition GetOperator(string name, string ns, bool declaredOnly = false) {
            return GetOperator(QualifiedName.Create(ns, name), declaredOnly);
        }

        public abstract OperatorDefinition GetOperator(string name, bool declaredOnly = false);
        public abstract OperatorDefinition GetOperator(QualifiedName name, bool declaredOnly = false);

        public PropertyDefinition GetProperty(AttachedPropertyID name) {
            if (name == null)
                throw new ArgumentNullException("name");

            return GetProperty(name.DeclaringType.GetQualifiedName().Namespace + Utility.GetExtenderName(name), GetPropertyOptions.IncludeExtenders);
        }

        public PropertyDefinition GetProperty(string name, string ns, GetPropertyOptions options = GetPropertyOptions.None) {
            return GetProperty(QualifiedName.Create(ns, name), options);
        }

        public abstract PropertyDefinition GetProperty(QualifiedName name, GetPropertyOptions options = GetPropertyOptions.None);
        public abstract PropertyDefinition GetProperty(string name, GetPropertyOptions options = GetPropertyOptions.None);

        internal virtual IEnumerable<string> GetSerializationCandidateNamespaces() {
            var type = SourceClrType;
            var result = BaseTypes.Select(t => t.Namespace);

            if (type.GetTypeInfo().IsGenericType && !type.GetTypeInfo().IsGenericTypeDefinition) {
                result = result.Concat(
                    type.GetTypeInfo().GetGenericArguments()
                    .Select(t => {
                                var pd = PropertyTreeDefinition.SafeFromType(t);
                                return pd == null ? null : pd.Namespace;
                            }));
                }

            return result.Where(t => t != null).Distinct();
        }
    }
}
