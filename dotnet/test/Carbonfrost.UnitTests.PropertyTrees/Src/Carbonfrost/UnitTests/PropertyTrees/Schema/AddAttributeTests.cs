//
// Copyright 2013, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Reflection;

using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Spec;
using Prototypes;

namespace Carbonfrost.UnitTests.PropertyTrees.Schema {

    public class AddAttributeTests {

        sealed class HelperBuilderCollection {}

        [Fact]
        public void addon_method_implicit_collection() {
            string name = AddAttribute.TrimImplicitAdd("AddNew", typeof(PropertyNodeCollection));
            Assert.Equal("propertyNode", name);
        }

        [Fact]
        public void addon_method_implicit_builder_collection() {
            string name = AddAttribute.TrimImplicitAdd("AddNew", typeof(HelperBuilderCollection));
            Assert.Equal("helper", name);
        }

        [Theory]
        [InlineData(typeof(List<string>), "String")]
        [InlineData(typeof(Collection<string>), "String")]
        [InlineData(typeof(IEnumerable<string>), "String")]
        public void GetNaturalName_should_consider_generic_arguments(Type type, string expected) {
            string name = AddAttribute.GetNaturalName(type);
            Assert.Equal(expected, name);
        }

        [Theory]
        [InlineData(typeof(PropertyNodeCollection), "PropertyNode")]
        public void GetNaturalName_should_consider_derived_argument_types(Type type, string expected) {
            string name = AddAttribute.GetNaturalName(type);
            Assert.Equal(expected, name);
        }

        [Theory]
        [InlineData(typeof(S<Alpha>), "s")]
        public void ComputeName_Natural_should_use_return_type(Type type, string expected) {
            var method = type.GetTypeInfo().GetMethod("AddNew");
            string name = ((IRoleAttribute) AddAttribute.Natural).ComputeName(method);
            Assert.Equal(expected, name, StringComparer.OrdinalIgnoreCase);
            Assert.Contains(expected, PropertyTreeDefinition.FromType(type).EnumerateOperators().Select(t => t.Name));
        }

        class S<T> {
            [Add]
            public T AddNew(string s, string[] aliases) {
                throw new NotImplementedException();
            }
        }

        // TODO Support Dictionary<,> natural name - entry
    }
}
