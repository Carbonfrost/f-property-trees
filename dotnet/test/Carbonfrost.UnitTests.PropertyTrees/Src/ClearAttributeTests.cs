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

using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class ClearAttributeTests {

        [Clear]
        public void M() {}

        [Fact]
        public void ComputeName_should_derive_from_Name_property() {
            var method = GetType().GetMethod("M");
            var name = new ClearAttribute { Name = "N" }.ComputeName(method);
            Assert.Equal("N", name);
        }

        [Fact]
        public void ComputeName_should_derive_from_method_name() {
            var method = GetType().GetMethod("M");
            var name = new ClearAttribute().ComputeName(method);
            Assert.Equal("M", name);
        }

        [Fact]
        public void Clear_attribute_should_create_operator_by_name() {
            var schema = PropertyTreeDefinition.FromType(GetType());
            Assert.NotNull(schema.Operators["M", null]);
        }
    }
}
