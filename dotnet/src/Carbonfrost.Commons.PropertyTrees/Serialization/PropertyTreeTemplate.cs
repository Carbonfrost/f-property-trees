//
// Copyright 2014, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    sealed class PropertyTreeTemplate : ITemplate {

        readonly IList<ITemplateCommand> items;

        public PropertyTreeTemplate(ITemplateCommand[] items) {
            this.items = items;
        }

        public void Apply(object value) {
            var values = new Stack<object>();
            values.Push(value);

            foreach (var m in items) {
                m.Apply(values);
            }
        }
    }

}
