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

namespace Carbonfrost.Commons.PropertyTrees {

    interface IPropertyNode<TSelf, TProperty, TPropertyTree> : IPropertyTreeNavigator, IPropertyTreeReader
        where TSelf : IPropertyNode<TSelf, TProperty, TPropertyTree>
        where TProperty : IPropertyNode<TSelf, TProperty, TPropertyTree>
        where TPropertyTree : IPropertyNode<TSelf, TProperty, TPropertyTree> {

        // This interface mainly ensures that
        // PropertyNode and PropertyTreeNavigator have similar public APIs

        PropertyTreeWriter Append();
        TSelf Append(PropertyNode node);
        TSelf Append(PropertyTreeReader newChild);
        TSelf Append(PropertyTreeNavigator newChild);
        TPropertyTree AppendPropertyTree(string localName, string ns);
        TPropertyTree AppendPropertyTree(string localName);
        TProperty AppendProperty(string localName, string ns, object value);
        TProperty AppendProperty(string localName, object value);
        TProperty AppendProperty(string localName);

        PropertyTreeNavigator CreateNavigator();
        TSelf RemoveChildren();
        TSelf RemoveSelf();
        PropertyTreeWriter InsertAfter();
        PropertyTreeWriter InsertBefore();
    }
}
