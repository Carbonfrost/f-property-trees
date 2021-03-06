//
// - PropertyTree.cs -
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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    public partial class PropertyTree : PropertyNode, IXmlSerializable {

        private readonly PropertyNodeLinkedList children;
        private object valueCache;
        private Type blessedType;

        public PropertyTree(string localName, string ns) : base(localName, ns) {
            this.children = new PropertyNodeLinkedList(this);
        }

        public PropertyTree(QualifiedName name) : base(name) {
            this.children = new PropertyNodeLinkedList(this);
        }

        public PropertyTree(string localName) : base(localName) {
            this.children = new PropertyNodeLinkedList(this);
        }

        public PropertyTreeWriter AppendNode() {
            return new PropertyTreeNodeWriter(this);
        }

        public void SaveXml(XmlWriter xmlWriter, PropertyTreeWriterSettings settings = null) {
            using (var writer = PropertyTreeWriter.CreateXml(xmlWriter, settings)) {
                SaveXmlCore(writer);
            }
        }

        public void SaveXml(Stream outputStream, Encoding encoding = null, PropertyTreeWriterSettings settings = null) {
            using (var writer = PropertyTreeWriter.CreateXml(outputStream, encoding, settings)) {
                SaveXmlCore(writer);
            }
        }

        public string ToXmlString(PropertyTreeWriterSettings settings = null) {
            var sw = new StringWriter();
            var xml = XmlWriter.Create(sw);
            SaveXml(xml, settings);
            return sw.ToString();
        }

        // `PropertyNode' overrides
        public override PropertyNode FirstChild {
            get { return this.children.Head; }
        }

        public override PropertyNode LastChild {
            get { return this.children.Tail; }
        }

        public override bool RemoveChild(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.Parent == this) {
                this.children.Remove(node);
                return true;

            } else
                return false;
        }

        public override void InsertChildAt(int index, PropertyNode propertyNode) {
            if (propertyNode == null)
                throw new ArgumentNullException("propertyNode");
            if (propertyNode.Parent == this) {
                return;
            }

            if (propertyNode.QualifiedName != null) {
                PropertyNode existing = this.Children[propertyNode.QualifiedName];
                if (existing != null && (propertyNode.IsProperty || existing.IsProperty))
                    throw PropertyTreesFailure.DuplicateProperty("propertyNode", propertyNode.QualifiedName);
            }

            this.children.InsertInternal(index, propertyNode);
        }

        public override PropertyNodeCollection Children { get { return this.children; } }

        protected internal override void AcceptVisitor(PropertyTreeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor"); // $NON-NLS-1

            visitor.VisitPropertyTree(this);
        }

        protected internal override TResult AcceptVisitor<TArgument, TResult>(PropertyTreeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitPropertyTree(this, argument);
        }

        protected override void RemoveChildrenCore() {
            bool c = children.Count > 0;
            this.children.Clear();
        }

        public override void CopyTo(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            node.QualifiedName = this.QualifiedName;

            if (node.IsPropertyTree) {
                foreach (var child in this.Children) {
                    node.Append(child.Clone());
                }

            } else {
                // TODO Possible that Value isn't appropriate for Property
                Property p = (Property) node;
                p.Value = this.Value;
            }
        }

        protected override PropertyNode CloneCore() {
            return Clone();
        }

        public new PropertyTree Clone() {
            PropertyTree tree = new PropertyTree(this.QualifiedName);
            CopyTo(tree);
            return tree;
        }

        public override void Bless(Type type) {
            if (this.blessedType != type) {
                this.blessedType = type;

                if (type == null) {
                    this.valueCache = Undefined.Value;
                } else {
                    this.valueCache = this.Bind(type);
                }
            }
        }

        public override PropertyNodeType NodeType {
            get { return PropertyNodeType.PropertyTree; } }

        public override PropertyNodeDefinition Definition {
            get {
                throw new NotImplementedException();
            }
        }

        public override object Value {
            get {
                if (this.valueCache == null && this.blessedType != null) {
                    this.valueCache = Bind(this.blessedType);
                }

                return this.valueCache;
            }
            set {
                this.valueCache = value;
                this.RemoveChildren();

                if (value != null) {
                    PropertyTree.FromValue(value).CopyContentsTo(this);
                }
            }
        }

        public new PropertyTree Append(PropertyNode propertyNode) {
            return (PropertyTree) base.Append(propertyNode);
        }

        public new PropertyTree Append(PropertyTreeReader newChild) {
            return (PropertyTree) base.Append(newChild);
        }

        public new PropertyTree Append(PropertyTreeNavigator newChild) {
            return (PropertyTree) base.Append(newChild);
        }

        public new PropertyTree RemoveChildren() {
            return (PropertyTree) base.RemoveChildren();
        }

        public new PropertyTree RemoveSelf() {
            return (PropertyTree) base.RemoveSelf();
        }

        // IXmlSerializable implementation
        XmlSchema IXmlSerializable.GetSchema() {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            throw new NotImplementedException();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer) {
            this.SaveXml(writer);
        }

        private void SaveXmlCore(PropertyTreeXmlWriter writer) {
            writer.WriteStartDocument();
            this.WriteTo(writer);
            writer.WriteEndDocument();
        }

    }
}
