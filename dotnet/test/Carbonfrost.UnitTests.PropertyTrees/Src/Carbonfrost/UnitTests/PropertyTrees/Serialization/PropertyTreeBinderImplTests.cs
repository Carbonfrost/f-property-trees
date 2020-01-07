//
// - PropertyTreeBinderImplTests.cs -
//
// Copyright 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.PropertyTrees.Serialization;
using Carbonfrost.Commons.Spec;


namespace Carbonfrost.UnitTests.PropertyTrees.Serialization {

    public class PropertyTreeBinderImplTests {

        class C {

            internal readonly IDictionary<string, string> _values = new Dictionary<string, string>();

            public string this[string name] {
                get {
                    if (!_values.ContainsKey(name)) {
                        _values[name] = "implicit";
                    }

                    return _values[name];
                }
                set {
                    _values[name] = value;
                }
            }
        }

        [Fact]
        public void ImplicitDirective_should_not_try_indexer_properties() {
            var props = new C();
            var target = PropertyTreeMetaObject.Create(props);
            PropertyTreeBinderImpl.ImplicitDirective(target, "source");

            string text;
            Assert.False(props._values.TryGetValue("source", out text));
        }
    }
}
