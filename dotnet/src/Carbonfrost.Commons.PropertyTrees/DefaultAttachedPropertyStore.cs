//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

namespace Carbonfrost.Commons.PropertyTrees {

    sealed class DefaultAttachedPropertyStore : IAttachedPropertyStore {

        private readonly Lazy<Dictionary<AttachedPropertyID, object>> _data = new Lazy<Dictionary<AttachedPropertyID, object>>();

        public bool RemoveProperty(AttachedPropertyID propertyID) {
            if (_data.IsValueCreated) {
                var dictionary = _data.Value;
                lock (dictionary) {
                    return dictionary.Remove(propertyID);
                }
            }
            return false;
        }

        public void SetProperty(AttachedPropertyID propertyID, object value) {
            var dictionary = _data.Value;
            lock (dictionary) {
                dictionary[propertyID] = value;
            }
        }

        public bool TryGetProperty(AttachedPropertyID propertyID, out object value) {
            if (_data.IsValueCreated) {
                var dictionary = _data.Value;

                lock (dictionary) {
                    object answer;

                    if (dictionary.TryGetValue(propertyID, out answer)) {
                        value = answer;
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        public IEnumerator<KeyValuePair<AttachedPropertyID, object>> GetEnumerator() {
            if (_data.IsValueCreated) {
                return _data.Value.GetEnumerator();
            } else {
                return Enumerable.Empty<KeyValuePair<AttachedPropertyID, object>>().GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
