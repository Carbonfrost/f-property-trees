//
// - PropertyTreeWriter.cs -
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
using System.Xml;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract partial class PropertyTreeWriter : DisposableObject {

        public abstract WriteState WriteState { get; }
        public abstract PropertyTreeWriterSettings Settings { get; }

        public void Close() {
            Dispose();
        }

        public void CopyFrom(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            node.WriteTo(this);
        }

        public void CopyContentsFrom(PropertyTree tree) {
            if (tree == null)
                throw new ArgumentNullException("tree");

            tree.WriteContentsTo(this);
        }

        public void CopyFrom(PropertyTreeReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            reader.CopyTo(this);
        }

        public void CopySubtreeFrom(PropertyTreeReader reader) { // CopyContentsFrom?
            if (reader == null)
                throw new ArgumentNullException("reader");

            reader.CopySubtreeTo(this);
        }

        public void ReadToEnd(PropertyTreeReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader"); // $NON-NLS-1

            _ReadSubtree(reader, -1);
        }

        public void ReadSubtree(PropertyTreeReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader"); // $NON-NLS-1

            _ReadSubtree(reader, reader.Depth);
        }

        void _ReadSubtree(PropertyTreeReader reader, int depth) {
            while (reader.Read()) {
                CopyCurrent(reader); // TODO Exceptions generated by the write here (invalid local name) should be detected earlier and have line numbers

                if (reader.NodeType == PropertyNodeType.EndPropertyTree
                    || reader.NodeType == PropertyNodeType.Property) {

                    if (reader.Depth == depth)
                        break;
                }
            }
        }

        public void WriteObject(object value) {
            if (value == null)
                throw new ArgumentNullException("value"); // $NON-NLS-1

            ReadToEnd(new PropertyTreeObjectReader(value));
        }

        public void WriteNode(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node"); // $NON-NLS-1

            var reader = node.ReadNode();
            reader.Read();
            WriteNode(reader);
        }

        public void WriteNode(PropertyTreeReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader"); // $NON-NLS-1

            int initialDepth = reader.Depth;
            do {
                switch (reader.NodeType) {
                    case PropertyNodeType.EndPropertyTree:
                        this.WriteEndTree();
                        break;

                    case PropertyNodeType.Property:
                        this.WriteProperty(reader.Name, reader.Namespace, Convert.ToString(reader.Value));

                        break;

                    case PropertyNodeType.PropertyTree:
                        this.WriteStartTree(reader.Name, reader.Namespace);
                        break;
                }

            } while (reader.Read()
                     && ((initialDepth < reader.Depth)
                         || (initialDepth == reader.Depth && reader.NodeType == PropertyNodeType.EndPropertyTree)));
        }

        public void WriteStartTree(string localName) {
            WriteStartTree(localName, string.Empty);
        }

        public void WriteStartProperty(string localName) {
            WriteStartProperty(localName, string.Empty);
        }

        public void WriteProperty(string localName, string value) {
            WriteProperty(localName, string.Empty, value);
        }

        public abstract void WriteStartTree(string localName, string ns);
        public abstract void WriteStartProperty(string localName, string ns);

        public void WriteProperty(string localName,
                                  string ns,
                                  object value) {
            WriteStartProperty(localName, ns);
            WritePropertyValue(value);
            WriteEndProperty();
        }

        public abstract void WritePropertyValue(object value);
        public abstract void WriteEndProperty();
        public abstract void WriteEndTree();
        public abstract void WriteComment(string comment);
        public abstract void WriteStartDocument();
        public abstract void WriteEndDocument();
        public abstract void Flush();

        internal virtual void SetLineInfo(IXmlLineInfo lineInfo, IDictionary<string, string> prefixMap, IUriContext uriContext) {}
        internal virtual void SetExpressNamespace(bool isExpressNamespace) {}

        internal bool CopyCurrent(PropertyTreeReader reader) {
            if (reader.MoveToContent()) {
                SetLineInfo(reader as IXmlLineInfo ?? Utility.NullLineInfo, reader.PrefixMap, reader as IUriContext);
                SetExpressNamespace(reader.IsExpressNamespace);

                switch (reader.NodeType) {
                    case PropertyNodeType.Property:
                        WriteProperty(reader.Name, reader.Namespace, reader.Value);
                        return true;

                    case PropertyNodeType.PropertyTree:
                        this.WriteStartTree(reader.Name, reader.Namespace);
                        return true;

                    case PropertyNodeType.EndPropertyTree:
                        this.WriteEndTree();
                        return true;
                }
            }

            return false;
        }
    }

}
