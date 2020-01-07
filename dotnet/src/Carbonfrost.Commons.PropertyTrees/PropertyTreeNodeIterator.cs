//
// - PropertyTreeNodeIterator.cs -
//
// Copyright 2010 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract class PropertyTreeNodeIterator : IEnumerable<PropertyTreeNavigator> {

        public abstract int Position { get; }
        public abstract int Count { get; }

        // IEnumerator implementation
        public abstract bool MoveNext();
        public abstract PropertyTreeNavigator Current { get; }

        // IEnumerable implementation
        public abstract IEnumerator<PropertyTreeNavigator> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public abstract PropertyTreeNodeIterator Clone();

    }
}
