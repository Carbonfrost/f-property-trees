//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Spec;
using Prototypes;
using Carbonfrost.UnitTests.PropertyTrees;

namespace Carbonfrost.UnitTests.PropertyTrees.Serialization {

    public class PropertyTreeReaderBindPropertiesTests : TestBase {

        class D {

            private readonly Delta _u = new Delta();
            private readonly IProperties _underlying;

            internal Delta U { get { return _u; } }

            public object this[string property] {
                get {
                    return _underlying.GetProperty(property);
                }
                set {
                    _underlying.SetProperty(property, value);
                }
            }

            public D() {
                _underlying = Properties.FromValue(_u);
            }
        }

        [Fact]
        public void Bind_should_apply_collection_aggregation_on_readwrite_untyped_values_from_IProperties() {
            const string text = @"<d d='a b c' />";
            var item = PropertyTree.FromStreamContext(StreamContext.FromText(text)).Bind<D>();

            // We are using the indexer on D to provide an indirection to Delta.D, which is itself
            // a List<string>.  This should mean that aggregation using space-delimited list
            // is allowed.
            // var list = (IList<string>) item["D"];
            var list = item.U.D;
            Assert.Equal(new object[] { "a", "b", "c" }, list);
        }
    }
}
