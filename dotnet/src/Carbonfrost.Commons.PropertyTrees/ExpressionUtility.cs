//
// Copyright 2010, 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Core.Runtime.Expressions;

namespace Carbonfrost.Commons.PropertyTrees {

    static class ExpressionUtility {

        // TODO Escape with $$

        private static readonly Regex EXPR_FORMAT = new Regex(@"\$ (
(\{ (?<Expression> [^\}]+) \}) | (?<Expression> [:a-z0-9_\.]+) )", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public static bool TryParse(string text, out Expression result) {
            result = null;
            if (!text.Contains("$")) {
                return false;
            }

            var matches = MatchVariables(text);

            var exprs = EnumerateExpressions(matches, text).Where(t => t != null).ToList();
            if (exprs.Count == 1) {
                result = exprs[0];
            } else {
                result = Expression.NewArray(exprs);
            }

            return true;
        }

        public static Expression LiftToCall(Expression expression, IExpressionContext context) {
            return Expression.Call(Expression.Lambda(
                expression,
                context,
                null));
        }

        private static MatchCollection MatchVariables(string text) {
            MatchCollection matches = EXPR_FORMAT.Matches(text);
            return matches;
        }

        private static Expression ParseExprHelper(string expText) {
            return Expression.Parse(expText);
        }

        private static IEnumerable<Expression> EnumerateExpressions(MatchCollection matches, string text) {
            int previousIndex = 0;
            foreach (Match match in matches) {
                yield return Constant(text.Substring(previousIndex, match.Index - previousIndex));
                string expText = match.Groups["Expression"].Value;

                yield return ParseExprHelper(expText);
                previousIndex = match.Index + match.Length;
            }

            yield return Constant(text.Substring(previousIndex, text.Length - previousIndex));
        }

        static ConstantExpression Constant(string text) {
            if (text.Length > 0) {
                return Expression.Constant(text);
            }
            return null;
        }
    }
}
