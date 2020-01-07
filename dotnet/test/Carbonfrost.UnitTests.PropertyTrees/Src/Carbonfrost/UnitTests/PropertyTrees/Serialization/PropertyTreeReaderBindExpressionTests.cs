//
// - PropertyTreeReaderBindExpressionTests.cs -
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
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.Spec;
using Prototypes;
using Carbonfrost.UnitTests.PropertyTrees;


namespace Carbonfrost.UnitTests.PropertyTrees.Serialization {

    public class PropertyTreeReaderBindExpressionTests : TestBase {

        [Fact]
        public void Bind_should_bind_undefined_expressions_by_ignoring() {
            PropertyTreeReader pt = LoadContent("beta-upsilon.xml");
            Assert.True(pt.Read());

            Beta b = pt.Bind(new Beta());
            Assert.Null(b.D);
        }

        [Fact]
        public void Bind_should_bind_expression_string() {
            PropertyTreeReader pt = LoadContent("beta-upsilon.xml");
            Assert.True(pt.Read());

            var opts = new PropertyTreeBinderOptions();
            opts.ExpressionContext.Data["text"] = "Text from expr";
            Beta b = pt.Bind(new Beta(), opts);

            Assert.Equal("Text from expr", b.D);
        }

        [Fact]
        public void Bind_should_invoke_IApplyProperties_instead_of_addon() {
            var doc = @"<t>
                          <s a='${a}' b='6' />
                        </t>";
            PropertyTreeReader pt = PropertyTreeReader.CreateXml(StreamContext.FromText(doc));
            Assert.True(pt.Read());

            var item = pt.Bind(new T());
            Assert.NotNull(item.ApplyInvokedWith);
            Assert.Same(item, item.ApplyInvokedWith[0]);
            Assert.Equal(NamespaceUri.Default + "S", item.ApplyInvokedWith[1]);

            var props = item.ApplyInvokedWith[2] as IProperties;
            Assert.NotNull(props);
            // Assert.IsAssignableFrom<Expression>(props.GetProperty("a"));
            Assert.IsInstanceOf<string>(props.GetProperty("b"));
        }

        class T : IApplyProperties {

            public object[] ApplyInvokedWith { get; private set; }

            [Add]
            public void S(string a, string b) {}

            public object Apply(object thisArg, object method, IProperties arguments) {
                ApplyInvokedWith = new object[] { thisArg, method, arguments };
                return null;
            }

        }

        [Fact]
        public void Bind_should_bind_expression_value() {
            PropertyTreeReader pt = LoadContent("beta-upsilon-2.xml");
            Assert.True(pt.Read());

            var obj = new Alpha();
            var opts = new PropertyTreeBinderOptions();
            opts.ExpressionContext.Data["obj"] = obj;
            Beta b = pt.Bind(new Beta(), opts);

            Assert.Equal(obj, b.A);
        }

        [Fact]
        public void Bind_should_bind_expression_value_via_string_conversion() {
            PropertyTreeReader pt = LoadContent("beta-upsilon-3.xml");
            Assert.True(pt.Read());

            var opts = new PropertyTreeBinderOptions();
            opts.ExpressionContext.Data["host"] = "carbonfrost.com";
            Beta b = pt.Bind(new Beta(), opts);

            Assert.Equal(new Uri("https://carbonfrost.com/health"), b.C);
        }

        [Fact]
        public void Bind_should_pickup_items_added_to_name_scope_and_bind_expression() {
            PropertyTreeReader pt = LoadContent("omicron-theta-3.xml");
            Assert.True(pt.Read());

            OmicronTheta b = pt.Bind<OmicronTheta>();
            Assert.Equal("Some string", b.Beta.D);
        }

        [Fact]
        public void Bind_should_collect_expression_value_on_IProperties() {
            PropertyTreeReader pt = LoadContent("beta-upsilon.xml");
            Assert.True(pt.Read());

            var opts = new PropertyTreeBinderOptions();
            opts.ExpressionContext.Data["text"] = "Text from expr";

            Properties b = pt.Bind(new Properties(), opts);

            // Assert.IsAssignableFrom<Expression>(b["d"]);
        }

        [Fact]
        public void Bind_should_collect_expression_value_on_IPropertiesContainer() {
            PropertyTreeReader pt = LoadContent("beta-upsilon.xml");
            Assert.True(pt.Read());

            var opts = new PropertyTreeBinderOptions();
            opts.ExpressionContext.Data["text"] = "Text from expr";

            var b = pt.Bind(new FakePropertiesContainer(), opts);

            // Assert.IsAssignableFrom<Expression>(b.Properties["d"]);
        }

        [Fact]
        public void Bind_should_collect_expression_value_on_lists() {
            var doc = @"<delta b='${values}' />";
            PropertyTreeReader pt = PropertyTreeReader.CreateXml(StreamContext.FromText(doc));
            Assert.True(pt.Read());

            var opts = new PropertyTreeBinderOptions();
            opts.ExpressionContext.Data["values"] = new [] {
                new Beta(),
                new Beta(),
                new Beta(),
            };

            var b = pt.Bind<Delta>(opts);

            Assert.Equal(3, b.B.Count);
        }

        [Fact]
        public void Bind_should_collect_multiple_expression_values_on_lists() {
            var doc = @"<delta b='${a} ${b}' />";
            PropertyTreeReader pt = PropertyTreeReader.CreateXml(StreamContext.FromText(doc));
            Assert.True(pt.Read());

            var opts = new PropertyTreeBinderOptions();
            opts.ExpressionContext.Data["a"] = new Beta();
            opts.ExpressionContext.Data["b"] = new Beta();

            var b = pt.Bind<Delta>(opts);

            Assert.Equal(2, b.B.Count);
        }

        [Fact]
        public void Bind_should_apply_to_add_operator() {
            PropertyTreeReader pt = LoadContent("omicron-upsilon.xml");
            Assert.True(pt.Read());

            var opts = new PropertyTreeBinderOptions();
            opts.ExpressionContext.Data["team"] = new { url = new Uri("https://example.com") };

            var b = pt.Bind(new Omicron(), opts);

            Assert.Equal(new Uri("https://example.com"), b.B.C);
        }

        class FakePropertiesContainer : IPropertiesContainer {

            private readonly Properties _properties = new Properties();

            IProperties IPropertiesContainer.Properties {
                get {
                    return Properties;
                }
            }

            public Properties Properties {
                get {
                    return _properties;
                }
            }
        }

    }
}
