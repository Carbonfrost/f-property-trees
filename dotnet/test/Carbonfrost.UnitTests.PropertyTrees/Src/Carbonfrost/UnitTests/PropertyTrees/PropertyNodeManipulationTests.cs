//
// - PropertyNodeManipulationTests.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Prototypes;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyNodeManipulationTests : TestBase {

        [Fact]
        public void test_append_to_nominal() {
            PropertyTree pt = new PropertyTree("a");
            Property p = new Property("p");
            p.AppendTo(pt);

            Assert.Same(p, pt.FirstChild);
            Assert.Equal(0, p.Position);
        }

        [Fact]
        public void test_remove_child_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Assert.Equal("alpha1", pt.Children[0].Name);
            pt.RemoveChildAt(0);
            Assert.Equal("alpha2", pt.Children[0].Name);
        }

        [Fact]
        public void test_insert_child_at_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Property property = new Property("p");

            Assert.Equal("alpha2", pt.Children[1].Name);
            pt.InsertChildAt(1, property);
            Assert.Equal("alpha2", pt.Children[2].Name);
            Assert.Same(property, pt.Children[1]);
            Assert.Equal(1, property.Position);
        }

        [Fact]
        public void test_insert_child_at_first_child() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Property property = new Property("p");

            Assert.Equal("alpha1", pt.Children[0].Name);
            pt.InsertChildAt(0, property);
            Assert.Same(property, pt.Children[0]);
            Assert.Equal(0, property.Position);
        }

        [Fact]
        public void test_insert_child_at_last_child() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Property property = new Property("o");
            int count = pt.Children.Count;

            Assert.Equal("alpha5", pt.Children[count - 1].Name);
            pt.InsertChildAt(count, property);

            Assert.Same(property, pt.Children[count]);
            Assert.Equal(count, property.Position);
        }

        [Fact]
        public void test_replace_with_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Property property = new Property("p");
            pt.Children[1].ReplaceWith(property);

            Assert.Same(property, pt.Children[1]);
            Assert.Same(property, pt.FirstChild.NextSibling);
            Assert.Equal(0, pt.FirstChild.Position);
            Assert.Equal(1, pt.FirstChild.NextSibling.Position);
        }

        [Fact]
        public void test_replace_first_child() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Property property = new Property("p");
            pt.FirstChild.ReplaceWith(property);

            Assert.Same(property, pt.Children[0]);
            Assert.Equal(0, pt.FirstChild.Position);
            Assert.Equal(1, pt.FirstChild.NextSibling.Position);
        }

        [Fact]
        public void test_replace_last_child() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Property property = new Property("p");
            pt.LastChild.ReplaceWith(property);

            Assert.Same(property, pt.Children[4]);
            Assert.Same(property, pt.LastChild);
            Assert.Equal(4, pt.LastChild.Position);
            Assert.Equal(3, pt.LastChild.PreviousSibling.Position);
        }
    }
}
