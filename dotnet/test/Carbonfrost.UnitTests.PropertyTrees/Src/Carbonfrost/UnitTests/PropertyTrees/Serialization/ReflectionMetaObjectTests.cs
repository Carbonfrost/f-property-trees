//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.PropertyTrees.Serialization;
using Carbonfrost.Commons.Spec;
using Prototypes;

namespace Carbonfrost.UnitTests.PropertyTrees.Serialization {

    public class ReflectionMetaObjectTests {

        [Fact]
        public void FindAddonMethod_should_use_best_matching_addon_method_homogeneous() {
            var items = new object[] { new Beta(), new Beta(), new Beta() };
            var addon = ReflectionMetaObject.FindAddonMethod(typeof(BetaList), items);

            Assert.NotNull(addon);
            Assert.Equal(typeof(BetaList).GetTypeInfo().GetMethod("Add", new[] { typeof(Beta) } ), addon);
        }

        [Fact]
        public void FindAddonMethod_should_use_best_matching_addon_method_heterogenous() {
            var items = new object[] { "a", 2, new Beta() };
            var addon = ReflectionMetaObject.FindAddonMethod(typeof(BetaList), items);

            Assert.NotNull(addon);
            Assert.Equal(typeof(List<object>), addon.DeclaringType);
        }

        [Fact]
        public void GetAddonElements_should_handle_nonwhitespace_strings() {
            var items = new object[] { new Beta(), " text ", new Beta() };
            var addonElements = ReflectionMetaObject.GetAddonElements(items);
            Assert.Same(items, addonElements);
        }

        [Fact]
        public void GetAddonElements_should_allow_whitespace_filtering() {
            Beta item1 = new Beta();
            Beta item2 = new Beta();
            var items = new object[] { " ", item1, item2, "  " };

            var addonElements = ReflectionMetaObject.GetAddonElements(items);
            Assert.Equal(new[] { item1, item2 } , addonElements.Cast<Beta>());
        }

        [Fact]
        public void GetAddonElements_should_convert_to_strings_and_trim() {
            var items = new object[] { " ", "text", "a", "  ", "b  " };

            var addonElements = ReflectionMetaObject.GetAddonElements(items);
            Assert.Equal(new[] { "texta  b" } , addonElements);
        }
    }
}
