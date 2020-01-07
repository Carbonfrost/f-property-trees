//
// - PropertyTreeReaderBindPropertiesObjectTests.cs -
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
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.Spec;
using Carbonfrost.UnitTests.PropertyTrees;


namespace Carbonfrost.UnitTests.PropertyTrees.Serialization {

    public class PropertyTreeReaderBindPropertiesObjectTests : TestBase {

        [Fact]
        public void Bind_should_prefer_reflection() {
            var props = PropertyTreeDefinition.FromType(typeof(F))
                .Properties.GetByLocalName("s");
            var def = PropertyTreeDefinition.FromType(typeof(F));

            const string text = @"<fileTarget s='50'> </fileTarget>";
            var f = StreamContext.FromText(text).ReadPropertyTree<F>();
            Assert.Equal(50, f.S);
        }

        [Fact]
        public void Bind_should_use_properties_for_expressions_even_for_reflection() {
            var opts = new PropertyTreeBinderOptions {
                ExpressionContext = new ExpressionContext {
                    Data = { { "element", 20 } }
                }
            };

            const string text = @"<fileTarget s='${element}'> </fileTarget>";
            var f = PropertyTree.FromStreamContext(StreamContext.FromText(text)).Bind<F>(opts);
            Assert.Equal(0, f.S); // Shouldn't be available to reflection
            Assert.Equal(20, f.Properties.GetInt32("s"));
            Assert.IsInstanceOf<CallExpression>(f.Properties.GetProperty<Expression>("s"));
        }

        [Fact]
        public void Bind_should_apply_global_expression_context_and_local_expression_context() {
            var opts = new PropertyTreeBinderOptions {
                ExpressionContext = new ExpressionContext {
                    Data = { { "element", 30 } }
                }
            };

            const string text = @"<fileTarget u='${a + q + element}'> </fileTarget>";
            var f = PropertyTree.FromStreamContext(StreamContext.FromText(text)).Bind<F>(opts);
            Assert.Equal(60, f.Properties.GetInt32("u"));
        }

        [Fact]
        public void Bind_should_apply_property_type_conversion() {
            var opts = new PropertyTreeBinderOptions {
                ExpressionContext = new ExpressionContext {
                    Data = {
                        { "m", "https://" },
                        { "b", "example.org" },
                    }
                }
            };

            const string text = @"<fileTarget u='${m}${b}/yas'> </fileTarget>";
            var f = PropertyTree.FromStreamContext(StreamContext.FromText(text)).Bind<F>(opts);
            Assert.Equal(new Uri("https://example.org/yas"), f.Properties.GetProperty<Uri>("u"));
        }


        class F : PropertiesObject {

            public int S {
                get;
                set;
            }

            public Uri T {
                get;
                set;
            }

            protected override ExpressionContext CreateExpressionContext() {
                var ec = new ExpressionContext {
                    Data = { { "a", 10 } },
                };
                ec.DataProviders.AddNew("b", new { q = 20 });
                return ec;
            }
        }
    }
}
