//
// Copyright 2010, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Xml;


using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    sealed class DocumentPTNavigator : PropertyTreeNavigator, IXmlNamespaceResolver, IUriContext {

        private PropertyNode _current;

        public DocumentPTNavigator(PropertyNode current) {
            _current = current;
        }

        public override Uri BaseUri {
            get {
                return _current.BaseUri;
            }
        }

        public override int Position {
            get {
                return _current.Position;
            }
        }

        public override int LineNumber {
            get {
                return _current.LineNumber;
            }
        }

        public override int LinePosition {
            get {
                return _current.LinePosition;
            }
        }

        public override bool MoveTo(PropertyTreeNavigator other) {
            if (other == null)
                throw new ArgumentNullException("other");

            DocumentPTNavigator otherNav = other as DocumentPTNavigator;
            if (otherNav != null && _current.Root == otherNav._current.Root) {
                _current = otherNav._current;
                return true;
            }
            return false;
        }

        public override bool MoveToChild(int position) {
            if (position < 0 && _current.Parent != null) {
                position = position % _current.Parent.Children.Count;
            }
            return MoveToChild(node => node.Position == position);
        }

        public override bool MoveToChild(PropertyNodeType nodeType) {
            return MoveToChild(node => node.NodeType == nodeType);
        }

        public override bool MoveToChild(string ns, string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString("name");
            }
            ns = ns ?? string.Empty;

            return MoveToChild(node => node.Name == name && node.Namespace == ns);
        }

        public override bool MoveToFirst() {
            return MoveToGeneric(_current.FirstSibling);
        }

        public override void MoveToRoot() {
            _current = _current.Root;
        }

        public override bool MoveToSibling(int index) {
            return MoveToSibling(node => node.Position == index);
        }

        public override bool MoveToSibling(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString("name");
            }
            return MoveToSibling(node => node.Name == name);
        }

        public override bool MoveToSibling(string ns, string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString("name");
            }
            ns = ns ?? string.Empty;

            return MoveToSibling(node => node.Name == name && node.Namespace == ns);
        }

        public override bool MoveToNext() {
            return MoveToGeneric(_current.NextSibling);
        }

        public override bool MoveToPrevious() {
            return MoveToGeneric(_current.PreviousSibling);
        }

        public override bool MoveToParent() {
            return MoveToGeneric(_current.Parent);
        }

        public override PropertyTreeNavigator Clone() {
            return new DocumentPTNavigator(_current);
        }

        public override object Value {
            get { return _current.Value; }
            set { _current.Value = value; }
        }

        public override int Depth {
            get { return _current.Depth; } }

        public override PropertyNodeDefinition Definition {
            get { return _current.Definition; } }

        public override PropertyNodeType NodeType {
            get { return _current.NodeType; } }

        public override string Namespace {
            get { return _current.Namespace; } }

        public override string Name {
            get { return _current.Name; } }

        internal override bool IsExpressNamespace {
            get {
                return _current.IsExpressNamespace;
            }
        }

        public override PropertyTreeWriter Append() {
            if (_current.IsPropertyTree) {
                var tree = (PropertyTree) _current;
                return new PropertyTreeNodeWriter(tree);
            } else {
                // TODO Could there be a schema tree here?
                throw new NotImplementedException();
            }
        }

        public override PropertyTreeNavigator AppendProperty(string localName, string ns, object value) {
            _current = _current.AppendProperty(localName, ns, value);
            return this;
        }

        public override PropertyTreeNavigator AppendPropertyTree(string localName, string ns) {
            _current = _current.AppendPropertyTree(localName, ns);
            return this;
        }

        Uri IUriContext.BaseUri {
            get {
                return BaseUri;
            }
            set {
            }
        }

        // ---
        private bool MoveToChild(Func<PropertyNode, bool> predicate) {
            return MoveToSibling(_current.FirstChild, predicate);
        }

        private bool MoveToSibling(Func<PropertyNode, bool> predicate) {
            return MoveToSibling(_current.FirstSibling, predicate);
        }

        private bool MoveToSibling(PropertyNode start,
                                   Func<PropertyNode, bool> predicate) {
            PropertyNode node = start;
            if (node != null) {
                while (!(predicate(node))) {
                    node = node.NextSibling;
                    if (node == null)
                        return false;
                }

                _current = node;
                return true;
            }
            return false;
        }

        private bool MoveToGeneric(PropertyNode node) {
            if (node == null)
                return false;

            _current = node;
            return true;
        }

        // `IXmlNamespaceResolver'
        public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope) {
            return null;
        }

        public string LookupNamespace(string prefix) {
            return _current.LookupNamespace(prefix);
        }

        public string LookupPrefix(string namespaceName) {
            return null;
        }
    }
}
