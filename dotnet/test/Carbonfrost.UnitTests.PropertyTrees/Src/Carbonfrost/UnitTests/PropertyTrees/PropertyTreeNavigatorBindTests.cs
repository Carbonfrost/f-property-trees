//
// - PropertyTreeNavigatorBindTests.cs -
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
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;
using Prototypes;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeNavigatorBindTests : TestBase {

        [Fact]
        public void bind_empty_node() {
            PropertyTree pt = LoadTree("alpha-empty.xml");
            Alpha a = new Alpha();

            pt.Bind(a);
            Assert.False(a.A);
            Assert.Null(a.C);
        }

        [Fact]
        public void bind_empty_node_in_parent_context() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Alpha a = new Alpha();

            pt.Children[0].Bind(a);

            Assert.False(a.A);
            Assert.Null(a.C);
            Assert.Null(a.U);
        }

        [Fact]
        public void bind_empty_self_closing_node_in_parent_context() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Alpha a = new Alpha();

            pt.Children[2].Bind(a);

            Assert.False(a.A);
            Assert.Null(a.C);
            Assert.Null(a.U);
        }
    }
}
