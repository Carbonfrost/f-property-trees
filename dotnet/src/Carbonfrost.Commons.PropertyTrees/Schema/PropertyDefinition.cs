//
// - PropertyDefinition.cs -
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
using System.Reflection;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public abstract class PropertyDefinition : PropertyNodeDefinition {

        public abstract Type PropertyType { get; }
        public abstract bool IsReadOnly { get; }
        public abstract bool IsOptional { get; }
        public abstract object DefaultValue { get; }
        public abstract PropertyTreeDefinition DeclaringTreeDefinition { get; }

        public override string ToString() {
            return string.Format("{2}.{0}:{1}", Name, PropertyType, DeclaringTreeDefinition);
        }

        public virtual bool IsIndexer {
            get {
                return false;
            }
        }

        public virtual bool IsExtender {
            get {
                return false;
            }
        }

        public virtual bool IsParamArray {
            get {
                return false;
            }
        }

        public object GetValue(object component) {
            return GetValue(component, null);
        }

        public void SetValue(object component, object value) {
            SetValue(component, null, null, value);
        }

        public object GetValue(object component, QualifiedName name) {
            object result;
            if (TryGetValue(component, null, name, out result)) {
                return result;
            }
            throw new NotImplementedException();
        }

        public void SetValue(object component, QualifiedName name, object value) {
            SetValue(component, null, name, value);
        }

        public abstract bool TryGetValue(object component, object ancestor, QualifiedName name, out object result);
        public abstract void SetValue(object component, object ancestor, QualifiedName name, object value);

        public virtual bool CanExtend(Type extendeeType) {
            return false;
        }

        internal virtual PropertyInfo GetUnderlyingDescriptor() {
            return null;
        }

        internal bool TryGetValue(object component, QualifiedName qualifiedName) {
            object dummy;
            return TryGetValue(component, null, qualifiedName, out dummy);
        }
    }
}
