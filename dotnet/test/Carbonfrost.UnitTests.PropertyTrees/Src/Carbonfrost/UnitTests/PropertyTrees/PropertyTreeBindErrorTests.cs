//
// -PropertyTreeBindErrorTests.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.UnitTests.PropertyTrees;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeBindErrorTests : TestBase {

        [Fact]
        public void bind_type_conversion_error() {
            PropertyTreeReader pt = LoadContent("alpha-invalid-1.xml");
            Assert.True(pt.Read());

            var ex = ExpectPropertyTreeException(() => pt.Bind<Alpha>());

            Assert.Equal(3, ex.FileLocation.LineNumber);
            Assert.Equal(3, ex.FileLocation.LinePosition);
            //Assert.Matches(@"Cannot parse .+ property `A' \(Prototypes.Alpha\).", ex.Message);
            Assert.Contains("not recognized as a valid Boolean", ex.InnerException.Message);
        }

        [Fact]
        public void bind_type_reference_does_not_resolve() {
            PropertyTreeReader pt = LoadContent("alpha-invalid-2.xml");
            Assert.True(pt.Read());

            var ex = ExpectPropertyTreeException(() => pt.Bind<Alpha>());

            Assert.Equal(4, ex.FileLocation.LineNumber);
            Assert.Equal(3, ex.FileLocation.LinePosition);
            //Assert.Matches(@"Cannot parse .+ property `U' \(Prototypes.Alpha\).", ex.Message);
            Assert.Contains("type was not found", ex.InnerException.Message);
        }

        [Fact]
        public void bind_missing_required_parameter() {
            PropertyTreeReader pt = LoadContent("eta-invalid-1.xml");
            Assert.True(pt.Read());

            var ex = ExpectPropertyTreeException(() => pt.Bind<Eta>());

            Assert.Equal(3, ex.FileLocation.LineNumber);
            Assert.Equal(2, ex.FileLocation.LinePosition);
            //Assert.Matches(@"required properties .+c \(https://ns.example.com/\) \(Prototypes.Eta\)", ex.Message);
        }

        [Fact]
        public void bind_missing_required_parameter_different_namespace() {
            PropertyTreeReader pt = LoadContent("eta-invalid-2.xml");
            Assert.True(pt.Read());

            var ex = ExpectPropertyTreeException(() => pt.Bind<Eta>());

            //Assert.Matches(@"required properties .+d \(https://ns.example.com/\) \(Prototypes.Eta\)", ex.Message);
            Assert.Equal(4, ex.FileLocation.LineNumber);
            Assert.Equal(2, ex.FileLocation.LinePosition);
        }


        private PropertyTreeException ExpectPropertyTreeException(Action action) {
            string text = "<none>";
            Exception error = null;
            try {
                action();
            } catch (PropertyTreeException ex) {
                return ex;
            } catch (Exception ex) {
                text = ex.GetType().FullName;
                error = ex;
            }

            Assert.Fail(string.Format("Expected PropertyTreeException, but {0} thrown. \n{1}", text, error));
            return null;
        }
    }
}
