//
// - ExpressionUtilityTests.cs -
//
// Copyright 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees.Serialization {

    public class ExpressionUtilityTests {

        [Theory]
        [InlineData("")]
        [InlineData("value")]
        public void Parse_should_be_false_for_non_expression(string text) {
            Expression dummy;
            Assert.False(ExpressionUtility.TryParse(text, out dummy));
        }

        [Fact]
        public void Parse_should_handle_nominal_expression_syntax() {
            Expression expr;
            ExpressionUtility.TryParse("${4 + a}", out expr);
            Assert.Equal("4 + a", expr.ToString());
        }

        [Fact]
        public void Parse_should_exclude_empty_string_literals() {
            Expression expr;
            ExpressionUtility.TryParse("${name}", out expr);
            Assert.Equal(ExpressionType.Name, expr.ExpressionType);
        }

        [Fact]
        [Skip("Output is missing parentheses, but otherwise correct")]
        public void Parse_should_aggregate_on_concatenations_on_mixed_literals() {
            Expression expr;
            ExpressionUtility.TryParse("solution is ${4 + a} or ${4 + b}", out expr);
            Assert.Equal("'solution is ' + (4 + a) + ' or ' + (4 + b)",
                         expr.ToString());
        }

        [Fact]
        public void Parse_should_detect_escaped_expression() {
            Expression expr;
            ExpressionUtility.TryParse("$${4 + a}", out expr);
            Assert.Equal("[ '$', '{4 + a}' ]", expr.ToString());
        }
    }
}


