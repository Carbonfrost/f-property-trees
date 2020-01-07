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
using System.Runtime.CompilerServices;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract class AttachedPropertyStore : IAttachedPropertyStore {

        private static readonly ConditionalWeakTable<object, DefaultAttachedPropertyStore> instances = new ConditionalWeakTable<object, DefaultAttachedPropertyStore>();

        public abstract bool RemoveProperty(AttachedPropertyID propertyID);
        public abstract void SetProperty(AttachedPropertyID propertyID, object value);
        public abstract bool TryGetProperty(AttachedPropertyID propertyID, out object value);

        public abstract IEnumerator<KeyValuePair<AttachedPropertyID, object>> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public static IAttachedPropertyStore FromValue(object instance) {
            if (instance == null)
                throw new ArgumentNullException("instance");

            var result = instance.TryAdapt<IAttachedPropertyStore>();
            if (result == null) {
                return GetOrCreateDefault(instance);
            } else
                return null;
        }

        static IAttachedPropertyStore GetOrCreateDefault(object instance) {
            return instances.GetOrCreateValue(instance);
        }

    }

}
