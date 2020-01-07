//
// - PropertyNodeTests.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.IO;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyNodeTests : TestBase {

        [Fact]
        public void test_position_nested() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Assert.Equal(0, pt.Root.Position);
            Assert.Equal("items", pt.Root.Name);
            Assert.Equal(5, pt.Children.Count);

            Assert.Equal(0, pt.FirstChild.Position);
            Assert.Equal(1, pt.Children[1].Position);
            Assert.Equal(0, pt.Children[1].FirstChild.Position);
        }

        [Fact]
        public void test_position_siblings() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Assert.Equal(0, pt.Root.Position);
            Assert.Equal(0, pt.FirstChild.Position);
            Assert.Equal(2, pt.Children[1].NextSibling.Position);
            Assert.Equal(3, pt.Children[1].NextSibling.NextSibling.Position);
        }

        [Fact]
        public void test_children_count() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Assert.True(pt.Children[1].HasChildren);
            Assert.Equal(22, pt.Children[1].Children.Count);
            Assert.Equal(0, pt.Children[1].NextSibling.Children.Count);
            Assert.Equal(1, pt.Children[1].NextSibling.NextSibling.Children.Count);
        }

        [Fact]
        public void test_is_root_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Assert.True(pt.Root.IsRoot);
            Assert.False(pt.FirstChild.IsRoot);
        }

        [Fact]
        public void test_parent_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            var p1 = pt.Root.Children[1];
            var p2 = pt.Root.Children[1].FirstChild;

            Assert.Same(pt.Root, p1.Parent);
            Assert.Same(pt.Root, p1.Root);
            Assert.Same(p1, p2.Parent);
            Assert.Same(pt.Root, p2.Root);
        }

        [Fact]
        public void BaseUri_should_be_absolute_uri() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            var p1 = pt.Root.Children[1];
            var baseUri = GetContentUri("alpha-list.xml");

            Assert.Equal(baseUri, p1.BaseUri);
        }
    }
}
