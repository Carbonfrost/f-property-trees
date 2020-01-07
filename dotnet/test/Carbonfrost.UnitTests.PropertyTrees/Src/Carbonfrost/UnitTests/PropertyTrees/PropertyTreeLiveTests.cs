//
// - PropertyTreeLiveTests.cs -
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
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;
using Prototypes;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeLiveTests {

        [Fact, Skip]
        public void live_binding_from_object_to_tree() {
            AlphaChi a = new AlphaChi();

            // Changing the object should materialize in tree
            PropertyTree pt = PropertyTree.FromValue(a, PropertyTreeValueOptions.Live);
            a.A = true;
            a.TT = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));

            Assert.Equal(true, pt["A"].Value);
            Assert.Equal(a.TT, pt["TT"].Value);
        }

        [Fact, Skip]
        public void live_binding_from_tree_to_object() {
            AlphaChi a = new AlphaChi();

            PropertyTree pt = PropertyTree.FromValue(a, PropertyTreeValueOptions.Live);
            Assert.NotNull(pt);
            Assert.Equal(typeof(Alpha).GetTypeInfo().GetProperties().Length, pt.Children.Count);

            // Changing the tree should update the object
            pt["A"].Value = "true";
            pt["TT"].Value = "1/1/2000 12:00:00 AM -08:00";

            Assert.True(a.A);
            Assert.Equal(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8)), a.TT);
        }

        [Fact, Skip]
        public void live_blessed_object_causes_error_missing_property() {
            Alpha a = new Alpha();
            PropertyTree pt = PropertyTree.FromValue(a);
            Assert.Throws<InvalidOperationException>(() => (pt["XXX"].Value = 5));
        }
    }
}

