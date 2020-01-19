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

using System;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class AttachedPropertyStoreTests {

        [Fact]
        public void FromValue_should_create_reusable_store() {
            object instance = new object();
            var store1 = AttachedPropertyStore.FromValue(instance);
            var store2 = AttachedPropertyStore.FromValue(instance);
            Assert.Same(store1, store2);
        }

        [Fact]
        public void FromValue_should_throw_on_missing_argument() {
            Assert.Throws<ArgumentNullException>(
                () => AttachedPropertyStore.FromValue(null)
            );
        }
    }
}
