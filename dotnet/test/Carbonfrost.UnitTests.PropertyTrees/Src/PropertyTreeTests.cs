//
// - PropertyTreeTests.cs -
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
using System.Linq;
using System.Reflection;
using System.Xml;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;
using Prototypes;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeTests {

        [Fact]
        public void copy_property_nominal() {
            Property a = new Property("a") { Value = 420 };
            Property b = new Property("b") { Value = 24 };
            b.CopyTo(a);

            Assert.Equal("b", a.Name);
            Assert.Equal(24, a.Value);
        }

        [Fact]
        public void copy_property_tree_nominal() {
            PropertyTree a = new PropertyTree("a");
            a.AppendProperty("x", 300);
            a.AppendProperty("y", 300);

            PropertyTree b = new PropertyTree("t");
            a.CopyTo(b);

            Assert.Equal("a", b.Name);
            Assert.Equal(2, b.Children.Count);
            Assert.Equal(300, b.Children["x"].Value);
            Assert.Equal(300, b.Children["y"].Value);
        }

        [Fact]
        public void binding_from_object_into_tree_value() {
            PropertyTree tree = new PropertyTree("t");
            Alpha a = new Alpha();
            a.A = true;
            a.TT = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));
            tree.Value = a;

            Assert.Equal(typeof(Alpha).GetTypeInfo().GetProperties().Length, tree.Children.Count);
            Assert.Contains("TT", tree.Children.Select(t => t.Name));

            Assert.Equal(a.TT, tree["TT"].Value);
            Assert.Equal(true, tree["A"].Value);
        }

        [Fact]
        public void binding_from_object_to_tree() {
            Alpha a = new Alpha();
            a.A = true;
            a.TT = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));

            PropertyTree pt = PropertyTree.FromValue(a);
            Assert.Equal(true, pt["A"].Value);
            Assert.Equal(a.TT, pt["TT"].Value);
        }

        [Fact]
        public void copy_from_object_to_tree() {
            Alpha a = new Alpha();
            a.A = true;
            a.TT = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));

            PropertyTree p0 = new PropertyTree("t");
            PropertyTree pt = PropertyTree.FromValue(a);
            pt.CopyTo(p0);

            Assert.Equal(true, p0["A"].Value);
            Assert.Equal(a.TT, p0["TT"].Value);
        }

    }
}

