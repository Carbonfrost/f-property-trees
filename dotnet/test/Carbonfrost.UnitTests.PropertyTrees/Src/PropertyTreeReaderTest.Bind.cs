//
// - PropertyTreeReaderTest.Bind.cs -
//
// Copyright 2010 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Spec;
using Prototypes;
using Carbonfrost.UnitTests.PropertyTrees;


namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeReaderTest : TestBase {

        [Fact]
        public void bind_complex_types() {
            PropertyTreeReader pt = LoadContent("beta.xml");
            Assert.True(pt.Read());

            Beta b = pt.Bind(new Beta());

            Assert.True(b.A.A);
            Assert.False(b.A.AA.HasValue);
            Assert.Equal(0, b.A.B);
            Assert.True(b.A.BB.HasValue);

            Assert.Equal(new Uri("http://carbonfrost.com"), b.C);
            Assert.Equal("Generic text", b.D);
        }

        [Fact]
        public void bind_complex_types_latebound() {
            // Demonstrates that the late bound version of Gamma
            // should be used (GammaExtension); As such, the shadowed
            // property should bind correctly
            PropertyTreeReader pt = LoadContent("beta-2.xml");
            Assert.True(pt.Read());

            Beta b = pt.Bind(new Beta());

            Assert.IsInstanceOf<GammaExtension>(b.E);

            GammaExtension ge = (GammaExtension) b.E;
            Assert.Equal(DateTime.Parse("6/16/2012 6:48 PM"), ge.B);
        }

        [Fact]
        public void bind_missing_properties_throws_exception() {
            PropertyTreeReader pt = LoadContent("beta-4.xml");
            Assert.True(pt.Read());

            var ex = Record.Exception(() => pt.Bind<Beta>());
            Assert.IsInstanceOf<PropertyTreeException>(ex);
            Assert.Contains("line 2, pos 5", ex.Message);
        }

        [Fact]
        public void bind_streaming_source() {
            PropertyTreeReader pt = PropertyTreeReader.CreateXml(
                StreamContext.FromSource(GetContentUri("beta-8.xml")));

            Assert.True(pt.Read());

            Beta b = pt.Bind(new Beta());

            Assert.NotNull(b.A);
            Assert.Equal(float.NaN, b.A.L);
            Assert.Equal("Carbonfrost F5 Project", b.A.M);
            Assert.Equal(65535, b.A.N);
        }

        [Fact]
        public void bind_should_prefer_source_on_class() {
            PropertyTreeReader pt = LoadContent("beta-3.xml");
            Assert.True(pt.Read());

            Beta b = pt.Bind(new Beta());
            Assert.Equal("not a streaming source", b.Source);
        }

        [Fact]
        public void bind_should_prefer_source_property_or_operator_on_class_namespace_specifier() {
            PropertyTreeReader pt = LoadContent("beta-9.xml");
            Assert.True(pt.Read());

            Beta b = pt.Bind(new Beta());
            Assert.Equal("not a streaming source", b.Source);
            Assert.Equal("also not a streaming source", b.E.Source.Source);
            Assert.Equal("addon streaming", b.K.B.Source);
        }

        [Fact]
        public void bind_streaming_source_data_uri() {
            PropertyTreeReader pt = LoadContent("beta-3.xml");
            Assert.True(pt.Read());

            Beta b = pt.Bind(new Beta());

            Assert.NotNull(b.F);
            Assert.Equal("after", b.F.GetProperty("time"));
            Assert.Equal("i", b.F.GetProperty("fall"));
            Assert.Equal("not a streaming source", b.F.GetProperty("source"));
        }

        [Fact]
        public void bind_latebound_provider() {
            Assert.NotNull(StreamingSource.FromName("xmlFormatter"));

            PropertyTreeReader pt = LoadContent("iota-chi.xml");
            Assert.True(pt.Read());

            IotaChi b = pt.Bind(new IotaChi());

            Assert.Equal("xmlFormatter", App.GetProviderName(typeof(StreamingSource), b.A).LocalName);
        }

        [Fact]
        public void bind_latebound_provider_criteria() {
            PropertyTreeReader pt = LoadContent("iota-chi-2.xml");
            Assert.True(pt.Read());

            IotaChi b = pt.Bind(new IotaChi());

            Assert.Equal("properties", App.GetProviderName(typeof(StreamingSource), b.A).LocalName);
        }

        [Fact]
        public void bind_concrete_class_indirections() {
            PropertyTreeReader pt = LoadContent("mu.xml");
            Assert.True(pt.Read());

            var b = (MuAlpha) pt.Bind<Mu>();
            Assert.Equal(420, b.B);
        }

        [Fact]
        public void bind_composable_providers() {
            PropertyTreeReader pt = LoadContent("pi-chi.xml");
            Assert.True(pt.Read());

            var b = pt.Bind<PiChi>().Preconditions;
            Assert.Equal("compose(environment(\"PROCESSOR_LEVEL\", \"l\"), environment(\"QR\", \"m\"), platform())",
                          b.ToString());
        }

        [Fact]
        public void bind_composable_providers_add_child() {
            PropertyTreeReader pt = LoadContent("pi-chi.xml");
            Assert.True(pt.Read());

            var b = pt.Bind<PiChi>().Controls;
            Assert.True(b.Controls[0] is Paragraph);
            Assert.True(b.Controls[0].Controls[0] is TextBox);
        }

        [Fact]
        public void bind_nondefault_constructors() {
            // Demonstrates that an object with a nondefault constructor
            // can be populated with arbitrary simple values

            PropertyTreeReader pt = LoadContent("eta.xml");
            Assert.True(pt.Read());

            Eta e = pt.Bind<Eta>();

            Assert.Equal(256, e.A);
            Assert.Equal(TimeSpan.Parse("2.5:05:05.200"), e.B);
            Assert.Equal(2256.231250002, e.C);
            Assert.Equal(293680235, e.D);
        }

        [Fact]
        public void bind_nondefault_constructors_complex() {
            // Demonstrates that an object with a nondefault constructor
            // can be populated with a complex object like Eta

            PropertyTreeReader pt = LoadContent("iota.xml");
            Assert.True(pt.Read());

            Iota i = pt.Bind<Iota>();

            Assert.Equal(8256, i.A);
            Assert.Equal(TimeSpan.Parse("82.5:05:05.200"), i.B);
            Assert.Equal(82256.231250002, i.C);
            Assert.Equal(8293680235, i.D);
            Assert.Equal("Typedescriptor", i.F);
            Assert.Equal(new Uri("http://carbonfrost.com"), i.G);

            Eta e = i.E;

            Assert.Equal(256, e.A);
            Assert.Equal(TimeSpan.Parse("2.5:05:05.200"), e.B);
            Assert.Equal(2256.231250002, e.C);
            Assert.Equal(293680235, e.D);
        }

        [Fact]
        public void bind_abstract_builder_types() {
            // Demonstrates that the builder indirection can be used on abstract types

            PropertyTreeReader pt = LoadContent("epsilon-chi-builder.xml");
            Assert.True(pt.Read());

            EpsilonChi e = pt.Bind<EpsilonChi>();
            Assert.IsInstanceOf<EpsilonChiAlpha>(e);
        }

        [Fact]
        public void bind_abstract_builder_types_xmlns_lookup_and_builder() {
            // Checks that xmlns prefixes can be expanded and that the builder is used
            PropertyTreeReader pt = LoadContent("epsilon-chi-builder-2.xml");
            Assert.True(pt.Read());

            var tup = pt.Bind<Tuple<EpsilonChi, Upsilon>>();
            EpsilonChi e = tup.Item1;
            Upsilon u = tup.Item2;
            Assert.IsInstanceOf<EpsilonChiAlpha>(e);
            Assert.Equal('q', ((EpsilonChiAlpha) e)._M);

            Assert.Equal('r', ((EpsilonChiAlpha) u.C)._M);
        }

        [Fact]
        public void bind_builder_types() {
            // Demonstrates that the builder indirection can be used

            PropertyTreeReader pt = LoadContent("epsilon-builder.xml");
            Assert.True(pt.Read());

            Epsilon e = pt.Bind<Epsilon>();

            Assert.True(e is EpsilonAlpha);
            Assert.Equal(256, e.A);
            Assert.Equal(TimeSpan.Parse("2.5:05:05.200"), e.B);
            Assert.Equal(2256.231250002, e.C);
            Assert.Equal(293680235, e.D);

            Assert.IsInstanceOf<EpsilonExtended>(e.E);

            EpsilonExtended f = (EpsilonExtended) e.E;
            Assert.Equal(1256, f.A);
            Assert.Equal(TimeSpan.Parse("12.5:05:05.200"), f.B);
            Assert.Equal(12256.231250002, f.C);
            Assert.Equal(1293680235, f.D);
            Assert.Equal(DateTime.Parse("2011-05-12 8:45AM"), f.F);
        }

        [Fact]
        public void bind_empty_node() {
            // Trivial binding of primitive types

            PropertyTree pt = LoadTree("alpha-empty.xml");
            Alpha a = pt.Bind<Alpha>();
            Assert.False(a.A);
        }

        [Fact]
        public void bind_primitive_types() {
            // Trivial binding of primitive types

            PropertyTreeReader pt = LoadContent("alpha.xml");
            Assert.True(pt.Read());

            Alpha a = pt.Bind(new Alpha());

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
            Assert.Equal(DateTimeStyles.AssumeLocal | DateTimeStyles.RoundtripKind, a.V);
        }

        [Fact]
        public void bind_ordered_list() {
            // Binding of an ordered list IList<T> implementation

            PropertyTreeReader pt = LoadContent("delta.xml");
            Assert.True(pt.Read());

            Delta originalD = new Delta();
            Delta d = pt.Bind(originalD);
            Assert.True(object.ReferenceEquals(d, originalD));
            Assert.Equal(3, d.A.Count);

            Assert.True(d.A[0].A);
            Assert.Equal(0, d.A[0].B);
            Assert.Equal('g', d.A[0].D);
            Assert.Equal(DateTime.Parse("3/30/2011 1:50 AM"), d.A[0].E);

            Assert.Equal(10.5000m, d.A[1].F);
            Assert.Equal(10.5, d.A[1].G);
            Assert.Equal(256, d.A[1].H);
            Assert.Equal(1024, d.A[1].I);

            Assert.Equal("Carbonfrost F5 Project", d.A[2].M);
            Assert.Equal(new Uri("http://carbonfrost.com"), d.A[2].Q);
            Assert.Equal(TimeSpan.Parse("4.12:0:30.5"), d.A[2].R);
            Assert.Equal(new Guid("{ED826F6C-47B5-4C40-B5B1-E847CB193E03}"), d.A[2].S);

            Assert.Equal(10.5000m, d.B[0].A.F);
            Assert.Equal(10.5, d.B[0].A.G);

            // <add> is inherited, but still binds
            Assert.Equal("Carbonfrost F5 Project", d.C[0].M);
        }

        [Fact]
        public void bind_with_type_conversion() {
            PropertyTreeReader pt = LoadContent("gamma.xml");
            Assert.True(pt.Read());

            Gamma g = new Gamma();
            pt.Bind(g);

            Assert.Equal(145 * 1000 * 1000, g.B);
            Assert.Equal(@"http(s)?://carbonfrost\.(com|net|org)", g.C.ToString());
        }

        [Fact]
        public void bind_add_method_factories() {
            // Binding of add methods as factories

            PropertyTreeReader pt = LoadContent("omicron.xml");
            Assert.True(pt.Read());

            // Add methods defined on the type and defined via an extension method
            Omicron o = pt.Bind<Omicron>();
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
        public void bind_add_method_factories_latebound() {

            // The latebound type should be used from add method factories
            PropertyTreeReader pt = LoadContent("omicron-4.xml");
            Assert.True(pt.Read());

            Omicron o = pt.Bind<Omicron>();
            Assert.True(o.G is GammaExtension);
            GammaExtension ge = (GammaExtension) o.G;
            Assert.Equal(DateTime.Parse("6/16/2012 6:48 PM"), ge.B);
        }

        [Fact]
        public void bind_add_method_factory_extension_no_parameters() {
            PropertyTreeReader pt = LoadContent("omicron-2.xml");
            Assert.True(pt.Read());

            Omicron o = pt.Bind<Omicron>();
            Assert.Equal(new CultureInfo("fr-FR"), o.G.A);
        }

        [Fact]
        public void bind_add_method_factory_extension_generic() {
            // Demonstrates that a generic interface implementation can
            // supply add methods
            PropertyTreeReader pt = LoadContent("phi.xml");
            Assert.True(pt.Read());

            Phi p = pt.Bind<Phi>();
            Assert.NotNull(p.G);
        }

        [Fact]
        public void bind_optional_parameters() {
            PropertyTreeReader pt = LoadContent("upsilon.xml");
            Assert.True(pt.Read());

            Upsilon p = pt.Bind<Upsilon>();
            Assert.Equal(45, p.A);
            Assert.Equal("yipp", p.B);
        }

        // TODO Optionals on ext method

        [Fact]
        public void bind_clear_method() {
            PropertyTreeReader pt = LoadContent("psi.xml");
            Assert.True(pt.Read());

            Psi p = new Psi();
            Assert.Equal(3, p.A.Count);

            p = pt.Bind<Psi>(p);
            Assert.Equal(0, p.A.Count);
        }

        // TODO Test add range where best item type is needed

        [Fact]
        public void bind_add_range_property() {
            PropertyTreeReader pt = LoadContent("psi-add-range.xml");
            Assert.True(pt.Read());

            Psi p = pt.Bind<Psi>(new Psi());
            Assert.Equal(4, p.B.Count);
            Assert.Equal("a", p.B[0]);
            Assert.Equal("c", p.B[2]);
            Assert.Equal("d", p.B[3]);
        }

        [Fact]
        public void bind_remove_method() {
            PropertyTreeReader pt = LoadContent("psi-remove.xml");
            Assert.True(pt.Read());

            Psi p = new Psi();
            Assert.Equal(3, p.A.Count);

            p = pt.Bind<Psi>(p);
            Assert.Equal(2, p.A.Count);
            Assert.Equal(47, p.A[0].B);
            Assert.Equal(47, p.A[1].B);
        }

        [Fact]
        public void bind_dictionary_access() {
            PropertyTreeReader pt = LoadContent("omicron-5.xml");
            Assert.True(pt.Read());

            OmicronAlpha p = pt.Bind<OmicronAlpha>();
            Assert.Equal(0, p.A_.B);
            Assert.Equal('g', p.A_.D);

            // These should not bind as dictionary accesses
            Assert.True(p.B);
            Assert.Equal(1, p.C_);
        }

        [Fact]
        public void bind_dictionary_access_kvp_syntax() {
            PropertyTreeReader pt = LoadContent("beta-5.xml");
            Assert.True(pt.Read());

            Beta p = pt.Bind<Beta>();
            Assert.Equal(new DateTime(2011, 3, 30, 1, 50, 00), p.G["gu"].E);
            Assert.Equal(10.5000m, p.G["gu"].F);
            Assert.Equal(10.5, p.G["gu"].G);
        }

        [Fact]
        public void bind_should_apply_explicit_factory_names() {
            PropertyTreeReader pt = LoadContent("beta-list.xml");
            Assert.True(pt.Read());

            var p = pt.Bind<BetaList>();

            Assert.Equal("Generic text", p[0].D);
            Assert.Equal(new Uri("http://carbonfrost.com"), p[0].C);
            Assert.Equal(true, p[0].A.A);
            Assert.Equal(new DateTime(2011, 3, 30, 1, 50, 00), p[0].A.E);

            // TODO Behavior of <add> is technically undefined because it could be Add(Beta) or Add(Object)
            Assert.Equal("Built-in add method", p[1].D);

        }

        [Fact]
        public void bind_should_invoke_ancestor_attached_property_context() {
            PropertyTreeReader pt = LoadContent("control-extension-property.xml");
            Assert.True(pt.Read());

            var p = pt.Bind<Canvas>();

            Assert.Equal(40, p.Controls[0]._Top);
            Assert.Equal(80, p.Controls[1]._Top);
        }

        [Fact]
        public void bind_should_invoke_generic_ancestor_attached_property_context() {
            PropertyTreeReader pt = LoadContent("control-extension-property-3.xml");
            Assert.True(pt.Read());

            var c = pt.Bind<Canvas>();

            Assert.Equal(2, c.Controls.Count);
            Assert.Equal(1, c.Controls[0].Controls.Count);
            Assert.Equal(132, c.Controls[0].Controls[0]._Left);
            Assert.Equal(62, c.Controls[1]._Left);
        }

        [Fact]
        public void bind_should_invoke_extension_method() {
            PropertyTreeReader pt = LoadContent("control-extension-property-2.xml");
            Assert.True(pt.Read());

            var p = pt.Bind<Canvas>();

            Assert.Equal(132, p.Controls[0]._Left);
            Assert.Equal(66, p.Controls[1]._Left);
        }

        // [Fact]
        // public void bind_should_transparently_handle_implied_operator_ns() {
        //     PropertyTreeReader pt = LoadContent("bravo-1.xml");
        //     Assert.True(pt.Read());

        //     var p = pt.Bind<Bravo>();

        //     Assert.Equal("assembly", p.Components[0].Type);
        //     Assert.Equal("Carbonfrost.Commons.SharedRuntime", p.Components[0].Name.Name);
        // }

        // [Fact]
        // public void bind_should_transparently_handle_implied_parameter_ns() {
        //     PropertyTreeReader pt = LoadContent("bravo-2.xml");
        //     Assert.True(pt.Read());

        //     var p = pt.Bind<Bravo>();

        //     Assert.Equal("assembly", p.Components[0].Type);
        //     Assert.Equal("Carbonfrost.Commons.SharedRuntime", p.Components[0].Name.Name);
        // }

        [Fact]
        public void bind_should_bind_nullable_reference_types() {
            // Reference types in ctor don't have to be specified

            PropertyTreeReader pt = LoadContent("iota-2.xml");
            Assert.True(pt.Read());

            Iota i = pt.Bind<Iota>();

            Assert.Equal(8256, i.A);
            Assert.Equal(TimeSpan.Parse("82.5:05:05.200"), i.B);
            Assert.Equal(82256.231250002, i.C);
            Assert.Equal(8293680235, i.D);
        }

        [Fact]
        public void bind_should_bind_nested_class() {
            PropertyTreeReader pt = LoadContent("iota-2.xml");
            Assert.True(pt.Read());

            Iota i = pt.Bind<Iota>();

            Assert.Equal("ng", i.H.A);
        }

        [Fact]
        public void bind_should_be_invalid_on_value_type() {
            // Values types in ctor must be specified

            PropertyTreeReader pt = LoadContent("iota-invalid-1.xml");
            Assert.True(pt.Read());

            var ex = Record.Exception(() => pt.Bind<Iota>());
            Assert.IsInstanceOf<PropertyTreeException>(ex);
            Assert.Contains("line 3, pos 2", ex.Message);
        }

        [Fact]
        public void bind_should_resolve_relative_source_url() {
            PropertyTreeReader pt = LoadContent("beta-7.xml");
            Assert.True(pt.Read());

            var a = pt.Bind<Beta>().H;
            Assert.Equal('g', a.D);
            Assert.Equal(DateTime.Parse("3/30/2011 1:50 AM"), a.E);
        }

        [Fact]
        public void bind_should_resolve_fragment_source_url() {
            PropertyTreeReader pt = LoadContent("omicron-theta.xml");
            Assert.True(pt.Read());

            var a = pt.Bind<OmicronTheta>().Alpha;
            Assert.Equal('g', a.D);
            Assert.Equal(DateTime.Parse("3/30/2011 1:50 AM"), a.E);
        }

        [Fact]
        public void bind_should_resolve_source_ellison() {
            PropertyTreeReader pt = LoadContent("omicron-theta-2.xml");
            Assert.True(pt.Read());

            var a = pt.Bind<OmicronTheta>().Alpha;
            Assert.Equal('g', a.D);
            Assert.Equal(DateTime.Parse("3/30/2011 1:50 AM"), a.E);
        }

        [Fact]
        public void bind_should_store_id_in_name_scope() {
            PropertyTreeReader pt = LoadContent("omicron-theta.xml");
            Assert.True(pt.Read());

            var ot = pt.Bind<OmicronThetaWithScope>();
            Assert.NotNull(ot.FindName("alpha"));
            Assert.Same(ot.Alpha, ot.FindName("alpha"));
        }

        [Fact]
        public void bind_should_store_name_in_name_scope() {
            PropertyTreeReader pt = LoadContent("omicron-theta-2.xml");
            Assert.True(pt.Read());

            var ot = pt.Bind<OmicronThetaWithScope>();
            Assert.NotNull(ot.FindName("my"));
            Assert.Same(ot.Delta, ot.FindName("my"));
            Assert.Equal("my", ot.Delta.Name);
        }

        [Fact]
        public void bind_should_resolve_NameValueCollection() {
            PropertyTreeReader pt = LoadContent("echo.xml");
            Assert.True(pt.Read());

            var a = pt.Bind<Echo>().V;
            Assert.Equal(1, a.Keys.Count);
            Assert.Equal("b", a["a"]);
        }

        [Fact]
        public void bind_should_apply_uri_context_to_object() {
            PropertyTreeReader pt = LoadContent("alpha-chi.xml");
            Assert.True(pt.Read());

            var a = pt.Bind<AlphaChi>();
            Assert.True(Convert.ToString(a.BaseUri).EndsWith("alpha-chi.xml"));
        }

        [Fact]
        public void bind_should_apply_uri_context_to_accessible_properties() {
            PropertyTreeReader pt = LoadContent("beta-6.xml");
            Assert.True(pt.Read());

            var a = pt.Bind<Beta>();
            Assert.True(Convert.ToString(a.J.BaseUri).EndsWith("beta-6.xml"));
        }

        [Fact]
        public void bind_should_apply_file_location_to_object() {
            PropertyTreeReader pt = LoadContent("beta-6.xml");
            Assert.True(pt.Read());
            var a = pt.Bind<Beta>();
            Assert.True(a.I.SetFileLocationCalled);
            Assert.Equal(9, a.I.LineNumber);
            Assert.Equal(6, a.I.LinePosition);
        }
    }
}
