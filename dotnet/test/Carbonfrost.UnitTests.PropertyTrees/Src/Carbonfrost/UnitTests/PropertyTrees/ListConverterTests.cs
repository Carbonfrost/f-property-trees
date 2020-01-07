//
// Copyright 2014, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class ListConverterTests {

        [Fact]
        public void ConvertFromString_should_parse_closed_generic_type_collection_interface() {
            var conv = TypeHelper.GetConverter(null, typeof(IList<Glob>));
            Assert.IsInstanceOf<ListConverter>(conv);

            var items = (IEnumerable<Glob>) conv.ConvertFromString("**/*.* abc/**/*.txt \t\t\r\n */.cs", typeof(IList<Glob>), null);
            // TODO Should the return type be Collection<Glob> rather than List?  (Currently,
            // we only use converters for aggregation - not directly)
            Assert.IsInstanceOf<List<Glob>>(items);
            Assert.Equal(3, items.ToList().Count);
            Assert.Contains(Glob.Anything, items);
        }

        [Fact]
        public void ConvertFromString_should_parse_closed_generic_type_derived_collection() {
            var conv = TypeHelper.GetConverter(null, typeof(Collection<Glob>));
            Assert.IsInstanceOf<ListConverter>(conv);

            var items = (IEnumerable<Glob>) conv.ConvertFromString("**/*.* abc/**/*.txt \t\t\r\n */.cs", typeof(Collection<Glob>), null);
            Assert.Equal(3, items.ToList().Count);
            Assert.Contains(Glob.Anything, items);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void ConvertFromString_should_parse_empty_string_or_whitespace(string text) {
            var conv = TypeHelper.GetConverter(null, typeof(Collection<Glob>));
            Assert.IsInstanceOf<ListConverter>(conv);

            var items = (IEnumerable<Glob>) conv.ConvertFromString(text, typeof(Collection<Glob>), null);
            Assert.Equal(0, items.ToList().Count);
        }

        class Int32List : List<Int32> {}

        [Fact]
        public void ConvertFromString_should_apply_specific_list_derived_type() {
            var conv = TypeHelper.GetConverter(null, typeof(Int32List));
            Assert.IsInstanceOf<ListConverter>(conv);

            var items = (IEnumerable<int>) conv.ConvertFromString(" 2 3 ", typeof(Int32List), null);
            Assert.IsInstanceOf<Int32List>(items);
            Assert.Equal(2, items.ToList().Count);
            Assert.Equal(new[] { 2, 3 }, items);
        }
    }
}
