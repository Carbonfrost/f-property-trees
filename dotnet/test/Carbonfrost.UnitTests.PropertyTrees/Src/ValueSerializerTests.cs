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

    public class ValueSerializerTests : TestClass {

        [Fact]
        public void FromName_should_get_hex() {
            Assert.Same(ValueSerializer.Base16, ValueSerializer.FromName("hex"));
        }

        [Theory]
        [InlineData("base16")]
        [InlineData("base64")]
        [InlineData("invalid")]
        [InlineData("percentage")]
        [InlineData("percentageRange")]
        [InlineData("timeout")]
        [InlineData("default")]
        public void FromName_should_get_builtin_serializers(string name) {
            Assert.NotNull(ValueSerializer.FromName(name));
        }

        public void ConvertFromString_should_throw_InvalidOperationException_by_default() {
            var subject = new DefaultValueSerializer();
            Assert.Throws<InvalidOperationException>(
                () => subject.ConvertFromString("-", typeof(object), null)
            );
        }

        public void ConvertToString_should_throw_InvalidOperationException_by_default() {
            var subject = new DefaultValueSerializer();
            Assert.Throws<InvalidOperationException>(
                () => subject.ConvertToString("-", null)
            );
        }

        class DefaultValueSerializer : ValueSerializer {}
    }
}
