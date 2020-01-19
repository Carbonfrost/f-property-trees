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
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class AttachedPropertyIDTests {

        private readonly AttachedPropertyID A1 = new AttachedPropertyID(typeof(string), "C");
        private readonly AttachedPropertyID A2 = new AttachedPropertyID(typeof(string), "C");
        private readonly AttachedPropertyID B = new AttachedPropertyID(typeof(string), "D");

        [Fact]
        public void Equals_should_apply() {
            Assert.True(A1.Equals((object) A2));
        }

        [Fact]
        public void Equals_operator_should_apply() {
            Assert.True(A1 == A2);
        }

        [Fact]
        public void NotEquals_operator_should_apply() {
            Assert.True(A1 != B);
        }

        [Fact]
        public void ToString_formats_from_name() {
            Assert.Equal("System.String.C", A1.ToString());
        }
    }
}
