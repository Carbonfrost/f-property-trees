//
// - EmptyPropertyNodeCollection.cs -
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
using System.Collections;
using System.Collections.Generic;

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees {

    sealed class EmptyPropertyNodeCollection : PropertyNodeCollection {

        internal static readonly PropertyNodeCollection Instance = new EmptyPropertyNodeCollection();

        // PropertyNodeCollection overrides

        public override int Count { get { return 0; } }
        public override IEnumerator<PropertyNode> GetEnumerator() {
            return new EnumeratorImpl();
        }

        // Nested types
        struct EnumeratorImpl : IEnumerator<PropertyNode> {
            public PropertyNode Current { get { throw Failure.OutsideEnumeration(); } }
            object IEnumerator.Current { get { return Current; } }
            public void Dispose() {}
            public bool MoveNext() { return false; }
            public void Reset() {}
        }
    }
}