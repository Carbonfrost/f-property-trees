//
// Copyright 2010, 2015, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
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

        private static readonly Regex EXPR_FORMAT = new Regex(
@"
(
    (?<DD> \$\$|\$$|\$(?=\s))
    | \$ \{ (?<Expression>  ([^\}])+      )  (?<ExpEnd> \} | $ )
    | \$    (?<Expression>  [:a-z0-9_\.]+ )
)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public static bool TryParse(string text, out Expression result) {
            result = null;
            if (!text.Contains("$")) {
                return false;
            }

            var matches = EXPR_FORMAT.Matches(text);

            var exprs = EnumerateExpressions(matches, text).Where(t => t != null).ToList();
            if (exprs.OfType<ErrorExp>().Any()) {
                return false;
            }
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

        private static Expression ParseExprHelper(string expText) {
            return Expression.Parse(expText);
        }

        private static IEnumerable<Expression> EnumerateExpressions(MatchCollection matches, string text) {
            int previousIndex = 0;
            foreach (Match match in matches) {
                yield return Constant(text.Substring(previousIndex, match.Index - previousIndex));

                if (match.Groups["DD"].Success) {
                    yield return Constant("$");

                } else {
                    string expText = match.Groups["Expression"].Value;
                    if (expText.Length == 0 || string.IsNullOrWhiteSpace(expText)) {
                        yield return new ErrorExp();
                        yield break;
                    }
                    else if (match.Groups["ExpEnd"].Success && match.Groups["ExpEnd"].Value != "}") {
                        yield return new ErrorExp();
                        yield break;
                    }
                    else {
                        Expression exp;
                        bool success = Expression.TryParse(expText, out exp);
                        if (!success) {
                            yield return new ErrorExp();
                            yield break;
                        }

                        yield return exp;
                    }
                }

                previousIndex = match.Index + match.Length;
            }
            yield return Constant(text.Substring(previousIndex, text.Length - previousIndex));
        }

        class ErrorExp : Expression {
            public override ExpressionType ExpressionType {
                get {
                    return (ExpressionType) 0;
                }
            }

            protected override void AcceptVisitor(IExpressionVisitor visitor) {}
            protected override TResult AcceptVisitor<TArgument, TResult>(IExpressionVisitor<TArgument, TResult> visitor, TArgument argument) {
                return default;
            }
            protected override TResult AcceptVisitor<TResult>(IExpressionVisitor<TResult> visitor) {
                return default;
            }
        }

        static ConstantExpression Constant(string text) {
            if (text.Length > 0) {
                return Expression.Constant(text);
            }
            return null;
        }
    }
}
