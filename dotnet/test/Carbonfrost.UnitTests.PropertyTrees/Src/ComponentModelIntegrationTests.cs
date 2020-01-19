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

using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    // TODO test sourcing, test pt:property,
    // test circular refs (#), test type converters,
    // test whitespace handling (never trim from attributes)

    public class ComponentModelIntegrationTests {

        [Fact]
        public void should_provide_streaming_source_using_file_extensions() {
            Assert.IsInstanceOf<PropertyTreeSource>(StreamingSource.Create(typeof(object), (ContentType) null, ".pt"));
            Assert.IsInstanceOf<PropertyTreeSource>(StreamingSource.Create(typeof(object), (ContentType) null, ".ptx"));
        }

        [Fact]
        public void should_provide_streaming_source_using_content_type() {
            Assert.IsInstanceOf<PropertyTreeSource>(StreamingSource.Create(typeof(object), ContentType.Parse(ContentTypes.PropertyTrees)));
        }

        [Fact]
        public void should_provide_xmlns() {
            Assert.Equal(Xmlns.PropertyTrees2010, typeof(PropertyTree).GetQualifiedName().NamespaceName);
            Assert.Equal(Xmlns.PropertyTreesSchema2010, typeof(PropertyTreeDefinition).GetQualifiedName().NamespaceName);
        }
    }
}
