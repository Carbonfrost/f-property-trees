//
// Copyright 2010, 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Diagnostics;
using System.Xml;

using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees {

    [DebuggerDisplay("{debuggerDisplayProxy}")]
    public abstract class PropertyTreeNavigator
        : IPropertyNode<PropertyTreeNavigator, PropertyTreeNavigator, PropertyTreeNavigator>, IXmlLineInfo {

        // TODO Probably need additional APIs similar to XPath

        public virtual bool CanEdit { get { return false; } }
        public virtual bool HasChildren { get { return false; } }
        public virtual string Path { get { throw new NotImplementedException(); } }

        private string debuggerDisplayProxy {
            get {
                return this.QualifiedName.ToString();
            }
        }

        public bool IsProperty {
            get { return this.NodeType == PropertyNodeType.Property; }
        }

        public bool IsPropertyTree {
            get { return this.NodeType == PropertyNodeType.PropertyTree; }
        }

        public virtual Uri BaseUri {
            get { return null; }
        }

        bool IPropertyTreeNavigator.IsExpressNamespace {
            get { return IsExpressNamespace; }
        }

        internal virtual bool IsExpressNamespace {
            get {
                return false;
            }
        }

        protected PropertyTreeNavigator() {}

        public virtual PropertyTreeWriter Append() {
            throw new NotSupportedException();
        }

        public PropertyTreeNavigator Append(PropertyNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            Append().CopyFrom(node);
            return this;
        }

        public virtual PropertyTreeNavigator Append(PropertyTreeReader newChild) {
            if (newChild == null) {
                throw new ArgumentNullException(nameof(newChild));
            }

            Append().ReadToEnd(newChild);
            return this;
        }

        public virtual PropertyTreeNavigator Append(PropertyTreeNavigator newChild) {
            if (newChild == null)
                throw new ArgumentNullException("newChild");

            PropertyTreeReader reader = CreateReader();
            return Append(reader);
        }

        public abstract PropertyTreeNavigator AppendPropertyTree(string localName, string ns);

        public PropertyTreeNavigator AppendPropertyTree(string localName) {
            return AppendPropertyTree(localName, null);
        }

        public PropertyTreeNavigator AppendProperty(string localName, object value) {
            return AppendProperty(localName, null, value);
        }

        public PropertyTreeNavigator AppendProperty(string localName) {
            return AppendProperty(localName, null);
        }

        public abstract PropertyTreeNavigator AppendProperty(string localName, string ns, object value);

        public virtual PropertyTreeNavigator CreateNavigator() {
            return Clone();
        }

        public virtual PropertyTreeNavigator RemoveChildren() {
            throw new NotSupportedException();
        }

        public virtual PropertyTreeNavigator RemoveSelf() {
            throw new NotSupportedException();
        }

        public virtual string GetString(string name) {
            throw new NotImplementedException();
        }

        public virtual PropertyTreeWriter InsertAfter() {
            throw new NotSupportedException();
        }

        public virtual PropertyTreeWriter InsertBefore() {
            throw new NotSupportedException();
        }

        public virtual void InsertBefore(PropertyTreeNavigator newSibling) {
            throw new NotSupportedException();
        }

        public virtual bool MoveTo(PropertyTreeNavigator other) {
            throw new NotSupportedException();
        }

        public virtual bool MoveToChild(string ns, string name) {
            throw new NotSupportedException();
        }

        public abstract bool MoveToChild(PropertyNodeType nodeType);
        public abstract bool MoveToChild(int position);
        public abstract bool MoveToFirst();

        public virtual bool MoveToFirstChild() {
            return MoveToChild(0);
        }

        public virtual bool MoveToSibling(int index) {
            throw new NotImplementedException();
        }

        public abstract bool MoveToSibling(string name);
        public abstract bool MoveToSibling(string ns, string name);
        public abstract bool MoveToNext();
        public abstract bool MoveToParent();
        public abstract bool MoveToPrevious();
        public abstract void MoveToRoot();

        public abstract PropertyTreeNavigator Clone();

        public abstract int Position { get; }
        public abstract int Depth { get; }
        public abstract string Name { get; }
        public abstract string Namespace { get; }

        public abstract PropertyNodeDefinition Definition { get; }

        public Type ValueType {
            get {
                throw new NotImplementedException();
            }
        }

        public abstract object Value { get; set; }
        public abstract PropertyNodeType NodeType { get; }

        internal FileLocation FileLocation {
            get {
                return new FileLocation(LineNumber, LinePosition, null);
            }
        }

        // IXmlLineInfo implementation
        public virtual int LineNumber { get { return -1; } }
        public virtual int LinePosition { get { return -1; } }

        bool IXmlLineInfo.HasLineInfo() {
            return true;
        }

        public QualifiedName QualifiedName {
            get {
                return QualifiedName.Create(this.Namespace, this.Name);
            }
        }

        private PropertyTreeReader CreateReader() {
            return new PropertyTreeNavigatorReader(this);
        }

        // `IPropertyTreeReader' implemenation
        public object Bind(Type componentType) {
            return Bind(componentType, null);
        }

        public object Bind(Type componentType, PropertyTreeBinderOptions options) {
            if (componentType == null) {
                throw new ArgumentNullException("componentType");
            }

            var obj = PropertyTreeMetaObject.Create(componentType);
            return TopLevelBind(obj, options, null).Component;
        }

        public T Bind<T>() {
            return Bind<T>(null);
        }

        public T Bind<T>(PropertyTreeBinderOptions options) {
            return (T) Bind(typeof(T), options);
        }

        public T Bind<T>(T component) {
            return Bind<T>(component, null);
        }

        public T Bind<T>(T component, PropertyTreeBinderOptions options) {
            PropertyTreeMetaObject obj;
            if (ReferenceEquals(component, null))
                obj = PropertyTreeMetaObject.Create(typeof(T));
            else
                obj = PropertyTreeMetaObject.Create(component);

            return (T) TopLevelBind(obj, options, null).Component;
        }

        public void CopyContentsTo(PropertyTree tree) {
            throw new NotImplementedException();
        }

        public void WriteContentsTo(PropertyTreeWriter writer) {
            throw new NotImplementedException();
        }

        public void WriteTo(PropertyTreeWriter writer) {
            throw new NotImplementedException();
        }

        public void CopyTo(PropertyNode node) {
            throw new NotImplementedException();
        }

        internal PropertyTreeMetaObject TopLevelBind(PropertyTreeMetaObject obj, PropertyTreeBinderOptions options, IServiceProvider serviceProvider) {
            return PropertyTreeMetaObjectBinder.Create(options).Bind(obj, this, serviceProvider);
        }
    }
}
