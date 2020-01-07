//
// - PropertyTreeDefinitionTests.cs -
//
// Copyright 2012, 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.ObjectModel;
using System.Linq;

using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Spec;

using Prototypes;

namespace Carbonfrost.UnitTests.PropertyTrees.Schema {

    public class PropertyTreeDefinitionTests : TestBase {

        [Fact]
        public void simple_properties_should_equal_expected_values_in_simple_types() {
            var a = PropertyTreeDefinition.FromType(typeof(Alpha));
            Assert.Equal("Alpha", a.Name);
            Assert.Equal("https://ns.example.com/", a.Namespace);
            Assert.Equal(typeof(Alpha), a.SourceClrType);
        }

        [Fact]
        public void Name_should_equal_mangle_in_constructed_generic_type() {
            var a = PropertyTreeDefinition.FromType(typeof(List<Alpha>));
            Assert.Equal("List-1", a.Name);
            Assert.Equal("", a.Namespace);
            Assert.Equal(typeof(List<Alpha>), a.SourceClrType);
        }

        [Fact]
        public void get_property_nominal() {
            var a = PropertyTreeDefinition.FromType(typeof(Alpha));
            Assert.Contains("A", a.Properties.Select(t => t.Name));
            Assert.NotNull(a.GetProperty("A"));
        }

        [Fact]
        public void get_property_case_insensitive() {
            var a = PropertyTreeDefinition.FromType(typeof(Alpha));
            Assert.NotNull(a.GetProperty("a"));
        }

        [Fact]
        public void get_generic_list_operators() {
            var def = PropertyTreeDefinition.FromType(typeof(Delta));
            Assert.Equal(0, def.Operators.Count);

            def = PropertyTreeDefinition.FromType(typeof(IList<Alpha>));
            Assert.NotNull(def.GetOperator("add"));
            Assert.NotNull(def.GetOperator("remove"));
            Assert.NotNull(def.GetOperator("clear"));
        }

        [Fact]
        public void generic_list_operators_should_override() {
            var def = PropertyTreeDefinition.FromType(typeof(GenericList));

            Assert.Null(def.GetOperator("add"));
            Assert.NotNull(def.GetOperator("s"));
        }

        [Fact]
        public void add_child_operators_nominal() {
            var def = PropertyTreeDefinition.FromType(typeof(IAddChild<Control>));
            var fac = def.GetOperator("p");
            var fac2 = def.Operators.GetByLocalName("p");

            Assert.NotNull(fac);
            Assert.NotEmpty(fac2);
        }

        [Fact]
        public void add_child_operators_alternate_name() {
            var def = PropertyTreeDefinition.FromType(typeof(IAddChild<Control>));
            var fac = def.GetOperator("p");
            var fac2 = def.Operators.GetByLocalName("p");

            Assert.NotNull(fac);
            Assert.NotEmpty(fac2);
        }

        [Fact]
        public void properties_should_include_text_type_indexer() {
            var def = PropertyTreeDefinition.FromType(typeof(Dictionary<QualifiedName, int>));
            Assert.NotNull(def.Properties.FirstOrDefault(t => t.IsIndexer));

            def = PropertyTreeDefinition.FromType(typeof(Dictionary<string, int>));
            Assert.NotNull(def.Properties.FirstOrDefault(t => t.IsIndexer));
        }

        [Fact]
        public void properties_should_not_include_non_text_types() {
            var def = PropertyTreeDefinition.FromType(typeof(List<string>));
            Assert.Null(def.Properties.FirstOrDefault(t => t.IsIndexer));
        }

        [Fact]
        public void Properties_should_include_the_IProperties_indexer() {
            var def = PropertyTreeDefinition.FromType(typeof(Properties));
            var indexer = def.Properties.OfType<IndexerUsingIPropertiesPropertyDefinition>().FirstOrDefault();
            Assert.NotNull(indexer);
        }

        [Fact]
        public void add_child_operators_inherited() {
            var def = PropertyTreeDefinition.FromType(typeof(ContainerControl));
            var fac = def.GetOperator("p");

            Assert.NotNull(fac);
        }

        [Fact]
        public void add_child_operators_inherited_constructed_generics() {
            var def = PropertyTreeDefinition.FromType(typeof(Collection<Control>));
            var fac = def.GetOperator("add");

            Assert.NotNull(fac);
            Assert.Equal("", fac.Namespace);
            Assert.Equal("Add", fac.Name);
        }

        [Fact]
        public void get_extender_property_nominal() {
            var def = PropertyTreeDefinition.FromType(typeof(Canvas));
            var fac = def.GetProperty("Canvas.top");

            Assert.NotNull(fac);
            Assert.True(fac.IsExtender);
            Assert.True(fac.CanExtend(typeof(Control)));
            Assert.Equal("https://ns.example.com/", fac.Namespace);
        }

        private void AssertIsCharlieValueExtender(PropertyDefinition fac) {
            Assert.NotNull(fac);
            Assert.True(fac.IsExtender);
            Assert.True(fac.CanExtend(typeof(Alpha)));
            Assert.Equal("https://ns.example.com/", fac.Namespace);
        }

        [Fact]
        public void GetPropertyByLocalName_should_include_classes_marked_with_Extender() {
            var def = PropertyTreeDefinition.FromType(typeof(Alpha));
            var fac = def.GetProperty("ValueExtender", GetPropertyOptions.IncludeExtenders);
            AssertIsCharlieValueExtender(fac);
        }

        [Fact]
        public void GetPropertyByLocalName_should_include_classes_marked_with_Extender_compound_name() {
            var def = PropertyTreeDefinition.FromType(typeof(Alpha));
            var fac = def.GetProperty("Charlie.valueExtender", GetPropertyOptions.IncludeExtenders);
            AssertIsCharlieValueExtender(fac);
        }

        [Fact, Skip("Pending re-eval of the spec")]
        public void GetPropertyByFullName_should_include_classes_marked_with_Extender() {
            var def = PropertyTreeDefinition.FromType(typeof(Alpha));
            var fac = def.GetProperty("ValueExtender", "https://ns.example.com/", GetPropertyOptions.IncludeExtenders);
            AssertIsCharlieValueExtender(fac);
        }

        [Fact]
        public void GetPropertyByFullName_should_include_classes_marked_with_Extender_compound_name() {
            var def = PropertyTreeDefinition.FromType(typeof(Alpha));
            var fac = def.GetProperty("Charlie.ValueExtender", "https://ns.example.com/", GetPropertyOptions.IncludeExtenders);
            AssertIsCharlieValueExtender(fac);
        }

        [Fact]
        public void get_extender_property_extension_method() {
            var def = PropertyTreeDefinition.FromType(typeof(Control));
            var fac = def.GetProperty("ControlExtensions.left");

            Assert.NotNull(fac);
            Assert.True(fac.IsExtender);
            Assert.True(fac.CanExtend(typeof(Paragraph)));
            Assert.Equal("https://ns.example.com/", fac.Namespace);

            // Look for type inheritance
            def = PropertyTreeDefinition.FromType(typeof(Paragraph));
            var other = def.GetProperty("ControlExtensions.left");
            Assert.Same(fac, other);
        }

        [Fact]
        public void get_extender_property_extension_method_inherit() {
            var def = PropertyTreeDefinition.FromType(typeof(Paragraph));
            var fac = def.GetProperty("left", GetPropertyOptions.IncludeExtenders);

            Assert.NotNull(fac);
            Assert.True(fac.IsExtender);
            Assert.True(fac.CanExtend(typeof(Paragraph)));
            Assert.Equal("https://ns.example.com/", fac.Namespace);
        }

        private static void AssertParagraphExtender(PropertyDefinition fac) {
            Assert.NotNull(fac);
            Assert.True(fac.IsExtender);
            Assert.True(fac.CanExtend(typeof(Paragraph)));
            Assert.Equal("https://ns.example.com/", fac.Namespace);
        }

        // TODO Test generics, including open generics

        class GenericList {

            [Add(Name = "s")]
            public void Add(object value) {}
        }

    }
}

