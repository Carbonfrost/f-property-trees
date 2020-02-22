//
// Copyright 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class DefaultAttachedPropertyStoreTests {

        private readonly AttachedPropertyID Prop = new AttachedPropertyID(typeof(string), "C");

        [Fact]
        public void SetProperty_should_add_value_to_store() {
            object instance = new object();
            var store = AttachedPropertyStore.FromValue(instance);

            store.SetProperty(Prop, instance);
            Assert.True(store.TryGetProperty(Prop, out _));
        }

        [Fact]
        public void GetEnumerator_should_contain_contents() {
            object instance = new object();
            var store = AttachedPropertyStore.FromValue(instance);

            store.SetProperty(Prop, instance);
            Assert.Equal(new [] {
                KeyValuePair.Create(Prop, instance),
            }, store.ToArray());
        }

        [Fact]
        public void TryGetProperty_should_retrieve_value() {
            object instance = new object();
            var store = AttachedPropertyStore.FromValue(instance);

            store.SetProperty(Prop, instance);
            store.TryGetProperty(Prop, out var result);
            Assert.Same(instance, result);
        }
    }
}
