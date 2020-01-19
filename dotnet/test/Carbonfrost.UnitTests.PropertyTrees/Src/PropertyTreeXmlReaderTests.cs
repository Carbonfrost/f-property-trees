//
// - PropertyTreeXmlReaderTests.cs -
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
using System.IO;
using System.Xml;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.Spec;
using ReadState = Carbonfrost.Commons.PropertyTrees.ReadState;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeXmlReaderTests : TestBase {

        [Fact]
        public void throw_before_moved() {
            PropertyTreeReader pt = LoadContent("beta.xml");
            // TODO pt.ReadState
            Assert.Throws(typeof(PropertyTreeException),
                          () => {
                              PropertyNodeType p = pt.NodeType;
                          });
        }

        [Fact]
        public void Value_should_equal_Expression() {
            PropertyTreeReader pt = LoadContent("beta-upsilon.xml");
            pt.Read();
            pt.Read();

            // Assert.IsAssignableFrom<Expression>(pt.Value);
        }

        [Fact]
        public void BaseUri_should_equal_absolute_uri_of_local_files() {
            PropertyTreeReader pt = LoadContent("beta-upsilon.xml");
            pt.Read();

            var baseUri = GetContentUri("beta-upsilon.xml");
            Assert.Equal(baseUri, pt.BaseUri);
        }

        [Fact]
        public void BaseUri_should_equal_absolute_uri_of_local_file_streams() {
            PropertyTreeReader pt = PropertyTreeReader.CreateXml(File.OpenRead(GetContentPath("beta-upsilon.xml")));
            pt.Read();

            var baseUri = GetContentUri("beta-upsilon.xml");
            Assert.Equal(baseUri, pt.BaseUri);
        }

        [Fact]
        public void node_types() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));

            Assert.Equal(ReadState.Initial, reader.ReadState);
            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.PropertyTree, reader.NodeType);
            Assert.Equal("beta", reader.Name);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("c", reader.Name);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("d", reader.Name);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.PropertyTree, reader.NodeType);
            Assert.Equal("a", reader.Name);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("a", reader.Name);

            // Additional reads
            TestUtils.Times(() => reader.Read(), 5);

            Assert.Equal(PropertyNodeType.EndPropertyTree, reader.NodeType);
            Assert.Equal("a", reader.Name);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.PropertyTree, reader.NodeType);
            Assert.Equal("b", reader.Name);

            Assert.True(reader.Read());
            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.EndPropertyTree, reader.NodeType);
            Assert.Equal("b", reader.Name);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.EndPropertyTree, reader.NodeType);
            Assert.Equal("beta", reader.Name);
        }

        [Fact]
        public void depth_and_position() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));

            Assert.Equal(ReadState.Initial, reader.ReadState);
            Assert.True(reader.Read());
            Assert.Equal(0, reader.Depth);
            Assert.Equal(0, reader.Position);

            Assert.True(reader.Read());
            Assert.Equal(1, reader.Depth);
            Assert.Equal(0, reader.Position);

            Assert.True(reader.Read());
            Assert.Equal(1, reader.Depth); // d
            Assert.Equal(1, reader.Position);

            Assert.True(reader.Read());
            Assert.Equal(1, reader.Depth); // a
            Assert.Equal(2, reader.Position);

            Assert.True(reader.Read());
            Assert.Equal(2, reader.Depth);
            Assert.Equal(0, reader.Position); // a.a

            // 9 additional reads
            TestUtils.Times(() => reader.Read(), 9);

            Assert.False(reader.Read());
            Assert.Equal(ReadState.EndOfFile, reader.ReadState);
        }

        [Fact]
        public void read_property_tree_implicitly_moves_initial() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));
            Assert.Equal(ReadState.Initial, reader.ReadState);
            PropertyTree tree = reader.ReadPropertyTree();
            Assert.Equal("beta", tree.Name);
        }

        [Fact]
        public void read_property_tree_implicitly_moves_initial_xml() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetXmlReader("beta.xml"));
            Assert.Equal(ReadState.Initial, reader.ReadState);
            PropertyTree tree = reader.ReadPropertyTree();
            Assert.Equal("beta", tree.Name);
        }

        [Fact]
        public void read_property_tree_from_root() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));
            Assert.True(reader.Read());
            PropertyTree tree = reader.ReadPropertyTree();
            AssertBetaFile(tree);
        }

        [Fact]
        public void read_property_tree_from_root_xml() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetXmlReader("beta.xml"));
            Assert.True(reader.Read());
            PropertyTree tree = reader.ReadPropertyTree();
            AssertBetaFile(tree);
        }

        [Fact]
        public void Read_should_handle_elided_property_trees() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetXmlReader("omicron-theta-2.xml"));
            // 6 Reads to get to the elision
            Assert.True(reader.Read()); // omicronTheta
            Assert.True(reader.Read()); // alpha
            Assert.True(reader.Read()); // id
            Assert.True(reader.Read()); // d
            Assert.True(reader.Read()); // e
            Assert.True(reader.Read()); // alpha
            Assert.True(reader.Read()); // beta
            Assert.True(reader.Read());

            Assert.Equal("a", reader.Name);
            Assert.Null(reader.Value);
            Assert.Equal(PropertyNodeType.PropertyTree, reader.NodeType);

            Assert.True(reader.Read());
            Assert.Equal("source", reader.Name);
            Assert.Equal("#alpha", reader.Value);
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);

            Assert.True(reader.Read());
            Assert.Equal("a", reader.Name);
            Assert.Equal(PropertyNodeType.EndPropertyTree, reader.NodeType);
        }

        [Fact]
        public void ReadPropertyTree_should_retain_prefix_map_in_nodes() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetXmlReader("alpha.xml"));
            Assert.True(reader.Read());
            Assert.True(reader.Read());
            Assert.True(reader.Read());

            // The tree created here should also support ns resolving
            PropertyTree tree = PropertyTreeReader.CreateXml(GetXmlReader("alpha.xml")).ReadPropertyTree();
            var nav = tree.CreateNavigator();
            var resolver = (IXmlNamespaceResolver) nav;

            nav.MoveToFirstChild();
            Assert.Equal("a", nav.Name);
            Assert.Equal("https://ns.carbonfrost.com/commons/core",
                          resolver.LookupNamespace("shared"));
            Assert.Null(resolver.LookupNamespace(string.Empty));
        }

        private void AssertBetaFile(PropertyTree tree) {
            Assert.Equal("beta", tree.Name);
            Assert.Equal("c", tree.FirstChild.Name);
            Assert.Equal("d", tree.Children[1].Name);
            Assert.Equal("a", tree.Children[2].Name);
            Assert.Equal("aa", tree[2][1].Name);
            Assert.Equal("b", tree[2][2].Name);
            Assert.Equal("a", tree[3][0].Name);
        }

        [Fact]
        public void read_full_document() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));

            Assert.Equal(ReadState.Initial, reader.ReadState);
            Assert.True(reader.Read());

            Assert.Equal(PropertyNodeType.PropertyTree, reader.NodeType);
            Assert.Equal("beta", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("c", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("d", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.PropertyTree, reader.NodeType);
            Assert.Equal("a", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("a", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("aa", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("b", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("bb", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("e", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.EndPropertyTree, reader.NodeType);
            Assert.Equal("a", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            // b --
            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.PropertyTree, reader.NodeType);
            Assert.Equal("b", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.Property, reader.NodeType);
            Assert.Equal("a", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.EndPropertyTree, reader.NodeType);
            Assert.Equal("b", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.True(reader.Read());
            Assert.Equal(PropertyNodeType.EndPropertyTree, reader.NodeType);
            Assert.Equal("beta", reader.Name);
            Assert.Equal(string.Empty, reader.Namespace);

            Assert.False(reader.Read());
            Assert.Equal(ReadState.EndOfFile, reader.ReadState);

        }

    }
}
