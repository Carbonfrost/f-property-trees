//
// - PropertyTreeNodeWriterTest.cs -
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
using System.Linq;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeNodeWriterTest : TestBase {

        // TODO Do some invald documents
        // TODO Error messages on invalid documents, including on invalid XML reading should not be obscure, and should include line pos

        [Fact]
        public void trivial_document() {
            PropertyTreeNodeWriter writer = new PropertyTreeNodeWriter();
            writer.WriteStartDocument();
            writer.WriteEndDocument();

            Assert.Null(writer.Root);
        }

        [Fact]
        public void document() {
            PropertyTreeNodeWriter writer = new PropertyTreeNodeWriter();
            writer.WriteStartDocument();
            writer.WriteStartTree("hello");
            writer.WriteStartTree("george");
            writer.WriteEndTree();
            writer.WriteEndTree();
            writer.WriteEndDocument();

            Assert.Equal("hello", writer.Root.Name);
            Assert.Equal(1, writer.Root.Children.Count);
            Assert.Equal("george", writer.Root.Children[0].Name);
            Assert.NotNull(writer.Root.Children["george"]);
            Assert.NotNull(writer.Root["george"]);
        }

        [Fact]
        public void document2() {
            PropertyTreeNodeWriter writer = new PropertyTreeNodeWriter();
            writer.WriteStartDocument();
            writer.WriteStartTree("hello");
            writer.WriteStartProperty("george");
            writer.WritePropertyValue("burdell");
            writer.WriteEndProperty();
            writer.WriteEndTree();
            writer.WriteEndDocument();

            Assert.Equal(1, writer.Root.Children.Count);
            Assert.NotNull(writer.Root.Children["george"]);
            Assert.NotNull(writer.Root["george"]);
            Assert.Equal("burdell", writer.Root["george"].Value);
        }


        [Fact]
        public void document3() {
            PropertyTreeNodeWriter writer = new PropertyTreeNodeWriter();
            writer.WriteStartDocument();
            writer.WriteStartTree("hello");

            writer.WriteStartProperty("george");
            writer.WritePropertyValue("burdell");
            writer.WriteEndProperty();

            writer.WriteStartProperty("buzz");
            writer.WritePropertyValue("234");
            writer.WriteEndProperty();

            writer.WriteProperty("hey", "arnold");

            writer.WriteEndTree();
            writer.WriteEndDocument();

            Assert.Equal(3, writer.Root.Children.Count);
            Assert.Equal(new [] { "george", "buzz", "hey" },
                         writer.Root.Children.Select(t => t.Name).ToArray());
            Assert.NotNull(writer.Root.Children["buzz"]);
            Assert.NotNull(writer.Root["buzz"]);
        }

    }
}
