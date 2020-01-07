//
// - PropertyBuilder.cs -
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
using System.Text;

namespace Carbonfrost.Commons.PropertyTrees {

    class PropertyBuilder {

        // Either an object or List<object> -- doing this because
        // adding multiple values is very rare
        private object _value;
        private readonly Property property;

        public Property Property { get { return property; } }

        public bool Any {
            get {
                return _value != null;
            }
        }

        public PropertyBuilder(string ns, string name) {
            this.property = new Property(name, ns);
        }

        public void AppendValue(object value) {
            // TODO Value could itself be null, which would break this assumption (uncommon)
            if (_value == null) {
                _value = value;
                return;
            }

            List<object> items = _value as List<object>;
            if (items == null) {
                _value = items = new List<object> { value };
            } else {
                items.Add(value);
            }
        }

        public void End() {
            List<object> items = _value as List<object>;
            if (items == null) {
                property.Value = _value;
            } else {
                property.Value = items.ToArray();
            }
        }
    }

}
