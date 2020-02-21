//
// Copyright 2010, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.ComponentModel;
using System.Xml;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract partial class PropertyNode
        : INotifyPropertyChanged, IPropertyNode<PropertyNode, Property, PropertyTree> {

        internal PropertyTree _parent;
        private IDictionary<string, string> _prefixMap;
        private bool _isExpressNamespace;

        public abstract PropertyNodeCollection Children { get; }

        public PropertyNode this[int index] {
            get {
                return Children[index];
            }
        }

        public PropertyNode this[QualifiedName name] {
            get {
                return Children[name];
            }
        }

        public PropertyNode this[string name] {
            get {
                return Children[name];
            }
        }

        public PropertyNode this[string name, string ns] {
            get {
                return Children[name, ns];
            }
        }

        public PropertyNode PreviousSibling {
            get;
            internal set;
        }

        public PropertyNode NextSibling {
            get;
            internal set;
        }

        public Uri BaseUri {
            get;
            set;
        }

        public PropertyNode FirstSibling {
            get {
                return (Parent == null) ? null : Parent.FirstChild;
            }
        }

        public PropertyNode LastSibling {
            get {
                return (Parent == null) ? null : Parent.LastChild;
            }
        }

        public virtual PropertyNode LastChild {
            get {
                return null;
            }
        }

        public virtual PropertyNode FirstChild {
            get {
                return null;
            }
        }

        public QualifiedName QualifiedName {
            get {
                if (string.IsNullOrEmpty(Name)) {
                    return null;
                }

                return QualifiedName.Create(Namespace ?? string.Empty, Name);
            }
            internal set {
                if (value == null) {
                    Name = null;
                    Namespace = null;
                } else {
                    Name = value.LocalName;
                    Namespace = value.Namespace.NamespaceName;
                }
            }
        }

        public int LineNumber {
            get;
            set;
        }

        public int LinePosition {
            get;
            set;
        }

        bool IPropertyTreeNavigator.IsExpressNamespace {
            get {
                return IsExpressNamespace;
            }
        }

        internal virtual bool IsExpressNamespace {
            get {
                return _isExpressNamespace;
            }
        }

        protected internal abstract void AcceptVisitor(PropertyTreeVisitor visitor);
        protected internal abstract TResult AcceptVisitor<TArgument, TResult>(PropertyTreeVisitor<TArgument, TResult> visitor, TArgument argument);

        public IEnumerable<PropertyNode> SelectNodes(string path) {
            throw new NotImplementedException();
        }

        public virtual PropertyNode SelectNode(string path) {
            // UNDONE Actual implementation that uses paths
            return null;
        }

        public PropertyTreeNavigator CreateNavigator() {
            return new DocumentPTNavigator(this);
        }

        public PropertyNode RemoveSelf() {
            RemoveSelfCore();
            return this;
        }

        protected virtual void RemoveSelfCore() {
            if (Parent == null) {
                throw PropertyTreesFailure.CannotDeleteRoot();
            }

            Parent.RemoveChild(this);
        }

        public PropertyTreeReader ReadNode() {
            return new PropertyTreeNodeReader(this);
        }

        public void ReplaceWith(PropertyNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            if (Parent != null) {
                int index = Position;
                var parent = Parent;
                Parent.RemoveChild(this);
                parent.InsertChildAt(index, node);
            }
        }

        public void WriteTo(PropertyTreeWriter writer) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteNode(this);
        }

        public void WriteContentsTo(PropertyTreeWriter writer) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            foreach (var s in Children)
                writer.WriteNode(s);
        }

        public PropertyTree Root {
            get {
                PropertyNode current = this;
                while (current.Parent != null) {
                    current = current.Parent;
                }

                return (PropertyTree) current;
            }
        }

        public PropertyTree Parent {
            get { return _parent; }
            set {
                if (value == null) {
                    if (Parent != null) {
                        Parent.RemoveChild(this);
                    }

                } else {
                    value.Append(this);
                }
            }
        }

        internal PropertyNode(string name, string namespaceUri) {
            Name = name;
            Namespace = namespaceUri;
        }

        internal PropertyNode(string name) {
            Name = name;
        }

        internal PropertyNode(QualifiedName name) {
            QualifiedName = name;
        }

        public PropertyNode Append(PropertyNode propertyNode) {
            InsertChildAt(Children.Count, propertyNode);
            return this;
        }

        public void Prepend(PropertyNode propertyNode) {
            InsertChildAt(0, propertyNode);
        }

        public virtual void InsertChildAt(int index, PropertyNode node) {
            throw PropertyTreesFailure.CannotAppendChild();
        }

        public void RemoveChildAt(int index) {
            RemoveChild(this.Children[index]);
        }

        public virtual bool RemoveChild(PropertyNode node) {
            return false;
        }

        public PropertyNode RemoveChildren() {
            RemoveChildrenCore();
            return this;
        }

        protected virtual void RemoveChildrenCore() {
        }

        public virtual int IndexOfChild(PropertyNode propertyNode) {
            return -1;
        }

        public virtual IEnumerable<PropertyNode> GetDescendants(string name, Type blessedType) {
            throw new NotImplementedException();
        }

        public virtual void Bless(Type type) {
        }

        public void Unbless() {
            Bless(null);
        }

        public void AppendTo(PropertyTree other) {
            other.Append(this);
        }

        // `IPropertyTreeReader`
        public T Bind<T>() {
            return CreateNavigator().Bind<T>();
        }

        public T Bind<T>(T model) {
            return CreateNavigator().Bind<T>(model);
        }

        public object Bind(Type instanceType) {
            return CreateNavigator().Bind(instanceType);
        }

        public T Bind<T>(PropertyTreeBinderOptions options) {
            return CreateNavigator().Bind<T>(options);
        }

        public T Bind<T>(T model, PropertyTreeBinderOptions options) {
            return CreateNavigator().Bind<T>(model, options);
        }

        public object Bind(Type instanceType, PropertyTreeBinderOptions options) {
            return CreateNavigator().Bind(instanceType, options);
        }

        public object SelectAttribute(string attribute) {
            if (attribute == null) {
                throw new ArgumentNullException(nameof(attribute));
            }
            attribute = attribute.Trim();

            if (attribute.Length == 0)
                throw Failure.AllWhitespace(nameof(attribute));

            switch (attribute) {
                case "position":
                    return Position;
                case "empty":
                    return !HasChildren;
                case "root":
                    return IsRoot;
                case "tree":
                    return IsPropertyTree;
                case "property":
                    return IsProperty;
                default:
                    return SelectAttributeCore(attribute);
            }
        }

        protected abstract object SelectAttributeCore(string attribute);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            if (PropertyChanged != null) {
                PropertyChanged(this, e);
            }
        }

        protected abstract PropertyNode CloneCore();

        public PropertyNode Clone() {
            return CloneCore();
        }

        // TODO Name behavior -- arbitrary changes
        // Enforce schema logic (trees can rename and aggregate)

        public string Namespace { get; private set; }
        public string Name { get; private set; }
        public virtual int Depth { get { return 0; } }
        public abstract PropertyNodeDefinition Definition { get; }

        public int Position { get; internal set; }

        public virtual string Path { get { return string.Empty; } }
        public bool IsPropertyTree { get { return NodeType == PropertyNodeType.PropertyTree; } }
        public bool IsProperty { get { return NodeType == PropertyNodeType.Property; } }
        public bool IsRoot { get { return Parent == null; } }
        public abstract PropertyNodeType NodeType { get; }

        public abstract object Value { get; set; }

        public Type ValueType {
            get {
                return TypeHelper.TypeOf(Value);
            }
        }

        public bool HasChildren {
            get {
                return Children.Count > 0;
            }
        }

        public PropertyTreeWriter Append() {
            if (IsPropertyTree) {
                return new PropertyTreeNodeWriter((PropertyTree) this);
            }

            throw PropertyTreesFailure.CannotAppendChild();
        }

        public PropertyNode Append(PropertyTreeReader newChild) {
            if (newChild == null) {
                throw new ArgumentNullException(nameof(newChild));
            }

            Append().CopyFrom(newChild);
            return this;
        }

        public PropertyNode Append(PropertyTreeNavigator newChild) {
            throw new NotImplementedException();
        }

        public PropertyTree AppendPropertyTree(string localName, string ns) {
            if (localName == null) {
                throw new ArgumentNullException(nameof(localName));
            }
            if (localName.Length == 0) {
                throw Failure.EmptyString(nameof(localName));
            }

            var result = new PropertyTree(localName, ns);
            Append(result);
            return result;
        }

        public PropertyTree AppendPropertyTree(string localName) {
            return AppendPropertyTree(localName, string.Empty);
        }

        public Property AppendProperty(string localName, string ns, object value) {
            if (localName == null) {
                throw new ArgumentNullException(nameof(localName));
            }
            if (localName.Length == 0) {
                throw Failure.EmptyString(nameof(localName));
            }
            var result = new Property(localName, ns) {
                Value = value
            };
            Append(result);
            return result;
        }

        public Property AppendProperty(string localName, object value) {
            return AppendProperty(localName, string.Empty, value);
        }

        public Property AppendProperty(string localName) {
            return AppendProperty(localName, null);
        }

        public PropertyTreeWriter InsertAfter() {
            throw new NotImplementedException();
        }

        public PropertyTreeWriter InsertBefore() {
            throw new NotImplementedException();
        }

        internal string LookupNamespace(string prefix) {
            string result;
            if (_prefixMap != null && _prefixMap.TryGetValue(prefix, out result)) {
                return result;
            }
            if (Parent != null) {
                return Parent.LookupNamespace(prefix);
            }

            return null;
        }

        public void CopyContentsTo(PropertyTree tree) {
            if (tree == null) {
                throw new ArgumentNullException(nameof(tree));
            }

            foreach (var child in Children) {
                tree.Append(child.Clone());
            }
        }

        public abstract void CopyTo(PropertyNode node);

        internal void InitFrom(IXmlLineInfo lineInfo,
                               IUriContext uriContext,
                               IDictionary<string, string> prefixMap,
                               bool isExpressNamespace)
        {
            if (lineInfo != null) {
                LinePosition = lineInfo.LinePosition;
                LineNumber = lineInfo.LineNumber;
            }
            if (uriContext != null) {
                BaseUri = uriContext.BaseUri;
            }
            _prefixMap = prefixMap;
            _isExpressNamespace = isExpressNamespace;
        }
    }
}
