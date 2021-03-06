//
// Copyright 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class PropertyTreeBinderOptionsTests : TestBase {

        [Fact]
        public void ReadOnly_should_cause_properties_to_be_read_only() {
            var opts = new PropertyTreeBinderOptions();
            opts = PropertyTreeBinderOptions.ReadOnly(opts);

            Assert.Throws<InvalidOperationException>(
                () => opts.ExpressionContext = new ExpressionContext()
            );
        }
    }
}
