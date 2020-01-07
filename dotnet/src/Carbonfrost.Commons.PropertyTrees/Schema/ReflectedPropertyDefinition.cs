//
// - ReflectedPropertyDefinition.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.ComponentModel;
using System.Reflection;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    class ReflectedPropertyDefinition : PropertyDefinition {

        private readonly PropertyInfo _property;

        public ReflectedPropertyDefinition(PropertyInfo property) {
            _property = property;
        }

        internal override PropertyInfo GetUnderlyingDescriptor() {
            return this._property;
        }

        public override PropertyTreeDefinition DeclaringTreeDefinition {
            get {
                return PropertyTreeDefinition.FromType(_property.DeclaringType);
            }
        }

        public override object DefaultValue {
            get {
                DefaultValueAttribute dva = _property.GetCustomAttribute<DefaultValueAttribute>();
                if (dva == null)
                    return null;
                else
                    return dva.Value;
            }
        }

        public override bool IsOptional {
            get {
                return true;
            }
        }

        public override string Namespace {
            get {
                return TypeHelper.GetNamespaceName(_property.DeclaringType);
            }
        }

        public override string Name {
            get {
                return _property.Name;
            }
        }

        public override void SetValue(object component, object ancestor, QualifiedName name, object value) {
            _property.SetValue(component, value);
        }

        public override bool TryGetValue(object component, object ancestor, QualifiedName name, out object result) {
            result = _property.GetValue(component);
            return true;
        }

        public override Type PropertyType {
            get {
                return _property.PropertyType;
            }
        }

        public override bool IsReadOnly {
            get {
                return !_property.CanWrite;
            }
        }

    }
}
