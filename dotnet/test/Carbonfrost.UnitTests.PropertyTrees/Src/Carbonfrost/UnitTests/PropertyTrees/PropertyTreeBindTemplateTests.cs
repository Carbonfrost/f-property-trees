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
using System.Globalization;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Spec;
using Prototypes;
using Carbonfrost.UnitTests.PropertyTrees;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeBindTemplateTests : TestBase {

        [Fact]
        public void bind_template_primitive_types() {
            PropertyTreeReader pt = LoadContent("alpha.xml");
            Assert.True(pt.Read());
            var template = pt.Bind<Template<Alpha>>();
            var a = new Alpha();
            template.Apply(a);

            Assert.True(a.A);
            Assert.False(a.AA.HasValue);
            Assert.Equal(0, a.B);
            Assert.True(a.BB.HasValue);
            Assert.Equal(0, a.BB.Value);
            Assert.Equal('g', a.D);
            Assert.Equal(DateTime.Parse("3/30/2011 1:50 AM"), a.E);
            Assert.Equal(10.5000m, a.F);
            Assert.Equal(10.5, a.G);
            Assert.Equal(256, a.H);
            Assert.Equal(1024, a.I);
            Assert.Equal(102410241024, a.J);
            Assert.Equal(-120, a.K);
            Assert.Equal(float.NaN, a.L);
            Assert.Equal("Carbonfrost F5 Project", a.M);
            Assert.Equal(65535, a.N);
            Assert.True(6553620 == a.O);
            Assert.True(6553620655362 == a.P);
            Assert.Equal(new Uri("http://carbonfrost.com"), a.Q);
            Assert.Equal(TimeSpan.Parse("4.12:0:30.5"), a.R);
            Assert.Equal(new Guid("{BF972F0A-CB10-441B-9D25-3D6DEB9065D1}"), a.S);
            Assert.Equal(DateTimeOffset.Parse("4/1/2011 12:11:01 AM -04:00"), a.T);
            Assert.Equal(typeof(Glob), a.U);
        }

        [Fact]
        public void bind_template_complex_types_readonly_accessor() {
            PropertyTreeReader pt = LoadContent("beta-6.xml");
            Assert.True(pt.Read());
            var template = pt.Bind<Template<Beta>>();
            var b = new Beta();
            template.Apply(b);

            Assert.True(b.H.A);
            Assert.False(b.A.AA.HasValue);
            Assert.Equal(10, b.H.B);
            Assert.True(b.H.BB.HasValue);
            Assert.Equal('g', b.H.D);
            Assert.Equal(DateTime.Parse("3/30/2011 1:50 AM"), b.H.E);
        }

        [Fact]
        public void bind_template_add_method_factory_extension_no_parameters() {
            PropertyTreeReader pt = LoadContent("omicron-2.xml");
            Assert.True(pt.Read());

            var tmpl = pt.Bind<Template<Omicron>>();
            Omicron o = new Omicron();
            tmpl.Apply(o);

            Assert.Equal(new CultureInfo("fr-FR"), o.G.A);
        }

        [Fact]
        public void bind_template_untyped_transition() {
            // The property return type Template gets constructed as a
            // typed version
            PropertyTreeReader pt = LoadContent("iota-chi-3.xml");
            Assert.True(pt.Read());

            var template = pt.Bind<IotaChi>().Template;
            var a = new Alpha();
            template.Apply(a);

            // Assert.IsAssignableFrom<Template<Alpha>>(template);
            Assert.Equal(1024, a.I);
            Assert.Equal(102410241024, a.J);
            Assert.Equal(-120, a.K);
            Assert.Equal(float.NaN, a.L);
        }

        [Fact]
        public void bind_template_untyped_transition_add_operator() {
            PropertyTreeReader pt = LoadContent("iota-chi-4.xml");
            Assert.True(pt.Read());

            var template = pt.Bind<IotaChi>().B.Template;
            var a = new Alpha();
            template.Apply(a);

            // Assert.IsAssignableFrom<Template<Alpha>>(template);
            Assert.Equal(1024, a.I);
            Assert.Equal(102410241024, a.J);
            Assert.Equal(-120, a.K);
            Assert.Equal(float.NaN, a.L);
        }

        [Fact]
        public void bind_template_properties_object() {
            var pp = PropertyTree.FromStreamContext(StreamContext.FromText(@"
        <iotaChi xmlns='https://ns.example.com/'>
            <add>
                <template>
                    <foxtrot target='some'></foxtrot>
                </template>
                <a provider='properties' />
            </add>
        </iotaChi>"));
            var col = pp.Bind<IotaChi>().B;
            // Assert.IsAssignableFrom<Template<Foxtrot>>(col.Template);

            var template = (Template<Foxtrot>) col.Template;
            Assert.Equal("some", template.CreateInstance().Properties.GetProperty("target"));
        }

        [Fact]
        public void bind_template_add_method_factories() {
            PropertyTreeReader pt = LoadContent("omicron.xml");
            Assert.True(pt.Read());

            var template = pt.Bind<Template<Omicron>>();
            Omicron o = new Omicron();
            template.Apply(o);

            Assert.True(o.A.A);
            Assert.False(o.A.AA.HasValue);
            Assert.Equal(0, o.A.B);
            Assert.True(o.A.BB.HasValue);
            Assert.Equal('g', o.A.D);
            Assert.Equal(DateTime.Parse("3/30/2011 1:50 AM"), o.A.E);

            Assert.Equal(new Uri("http://carbonfrost.com"), o.B.C);
            Assert.Equal("Generic text", o.B.D);
            Assert.Equal(true, o.B.A.A);

            Assert.Equal(new CultureInfo("en-US"), o.G.A);
        }

        [Fact]
        public void bind_template_add_range_property() {
            PropertyTreeReader pt = LoadContent("psi-add-range.xml");
            Assert.True(pt.Read());

            Psi p = new Psi();
            var template = pt.Bind<Template<Psi>>();
            template.Apply(p);

            Assert.Equal(4, p.B.Count);
            Assert.Equal("a", p.B[0]);
            Assert.Equal("c", p.B[2]);
            Assert.Equal("d", p.B[3]);
        }
    }
}
