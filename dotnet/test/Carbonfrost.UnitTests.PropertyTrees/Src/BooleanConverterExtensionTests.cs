//
// Copyright 2014, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.PropertyTrees;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class BooleanConverterExtensionTests {

        private BooleanConverterExtension Subject {
            get {
                return BooleanConverterExtension.Instance;
            }
        }

        [Fact]
        public void ConvertFromString_test_convert_builtins() {
            Assert.Equal(true, Subject.ConvertFromString("true", null, null));
            Assert.Equal(false, Subject.ConvertFromString("false", null, null));
        }

        [Fact]
        public void ConvertFromString_test_convert_aliases() {
            Assert.Equal(true, Subject.ConvertFromString("yes", null, null));
            Assert.Equal(false, Subject.ConvertFromString("no", null, null));
            Assert.Equal(true, Subject.ConvertFromString("YES", null, null));
            Assert.Equal(false, Subject.ConvertFromString("NO", null, null));
        }

        [Theory]
        [InlineData(true, "yes")]
        [InlineData(false, "no")]
        public void ConvertToString_test_convert_aliases(bool input, string expected) {
            Assert.Equal(expected, Subject.ConvertToString(input, null));
        }
    }
}
