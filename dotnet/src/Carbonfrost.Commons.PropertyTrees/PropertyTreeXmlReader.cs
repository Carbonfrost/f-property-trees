//
// - PropertyTreeXmlReader.cs -
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
using System.IO;
using System.Text;
using System.Xml;

using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;
using XReadState = System.Xml.ReadState;
using RuntimeExpression = Carbonfrost.Commons.Core.Runtime.Expressions.Expression;

namespace Carbonfrost.Commons.PropertyTrees {

    public partial class PropertyTreeXmlReader : PropertyTreeReader, IXmlLineInfo, IXmlNamespaceResolver, IUriContext {

        private readonly XmlReader reader;
        private PTXReaderState state;
        private int _elisionPosition;
        private Uri _baseUri;
        private readonly Stack<NodeData> data = new Stack<NodeData>();
        private readonly IXmlLineInfo readerLineInfoCache;

        class NodeData {
            public PropertyNodeType _nodeType;
            public string _name;
            public string _namespace;
            public int _depth;
            public object _value;
            public int _position;
            public bool _express;
            public Uri _baseUri;

            public IDictionary<string, string> prefixMap;

            public int LineNumber;
            public int LinePosition;

            public bool IsProperty { get { return _nodeType == PropertyNodeType.Property; } }
            public bool IsEndTree { get { return _nodeType == PropertyNodeType.EndPropertyTree; } }

        }

        internal PropertyTreeXmlReader(XmlReader reader, Uri baseUri) {
            this.reader = reader;
            this.readerLineInfoCache = (reader as IXmlLineInfo) ?? Utility.NullLineInfo;
            _baseUri = baseUri;

            if (this.reader.ReadState == XReadState.Interactive) {
                this.state = PTXReaderState.ChooseState(reader);
                this.state.Accept(this, reader);

            } else {
                this.state = PTXReaderState.Initial;
            }
        }

        public static PropertyTreeXmlReader Create(string inputFileName, Encoding encoding = null, PropertyTreeXmlReaderSettings settings = null) {
            return PropertyTreeReader.CreateXml(inputFileName, encoding, settings);
        }

        public static PropertyTreeXmlReader Create(Stream inputStream, Encoding encoding = null, PropertyTreeXmlReaderSettings settings = null) {
            return PropertyTreeReader.CreateXml(inputStream, encoding, settings);
        }

        public static PropertyTreeXmlReader Create(TextReader inputReader, PropertyTreeXmlReaderSettings settings = null) {
            return PropertyTreeReader.CreateXml(inputReader, settings);
        }

        public static PropertyTreeXmlReader Create(XmlReader inputReader, PropertyTreeXmlReaderSettings settings = null) {
            return PropertyTreeReader.CreateXml(inputReader, settings);
        }

        // PropertyTreeReader override
        internal override IDictionary<string, string> PrefixMap { get { return Peek().prefixMap; } }

        public override bool Read() {
            if (this.state.IsEOF)
                return false;

            PTXReaderState newState = state.Accept(this, reader);
            this.state = newState;
            return true;
        }

        private void ExportNamespace() {
            if ("xmlns" == reader.LocalName)
                EnsurePrefixMap().Add(string.Empty, reader.Value);
            else
                EnsurePrefixMap().Add(reader.LocalName, reader.Value);
        }

        public override object Value {
            get {
                return Peek()._value;
            }
        }

        public override Uri BaseUri {
            get {
                return Peek()._baseUri;
            }
        }

        internal override bool IsExpressNamespace {
            get {
                return Peek()._express;
            }
        }

        public override PropertyNodeType NodeType {
            get {
                return Peek()._nodeType;
            }
        }

        public override string Name {
            get {
                return Peek()._name;
            }
        }

        public override string Namespace {
            get {
                return Peek()._namespace;
            }
        }

        public override int Depth {
            get {
                return Peek()._depth;
            }
        }

        public override PropertyNodeDefinition Definition {
            get {
                throw new NotImplementedException();
            }
        }

        bool SetState(PTXReaderState next) {
            this.state = next;
            return true;
        }

        private NodeData Peek() {
            Moved();
            return this.data.Peek();
        }

        // Return true if an elision was detected
        private bool PushState(PropertyNodeType nodeType, PropertyNodeType nodeTypeIfAnElision = PropertyNodeType.PropertyTree) {
            var names = reader.LocalName.Split('-');
            var last = _elisionPosition == names.Length - 1;

            NodeData result = new NodeData();
            result._nodeType = last ? nodeType : nodeTypeIfAnElision;
            result._value = last ? ParseExpressions(nodeType, reader.Value) : null;
            result._name = names[_elisionPosition];
            result._namespace = reader.NamespaceURI;
            result._express = reader.Prefix.Length > 0;
            result.LineNumber = readerLineInfoCache.LineNumber;
            result.LinePosition = readerLineInfoCache.LinePosition;
            result._baseUri = string.IsNullOrEmpty(reader.BaseURI)
                ? _baseUri
                : Utility.NewUri(reader.BaseURI);

            int position = 0;
            if (data.Count > 0) {
                NodeData current = this.data.Peek();
                // Replacing a sibling
                if (current.IsProperty) {
                    this.data.Pop();
                    position = current._position + 1;
                }
                else if (current.IsEndTree) {
                    this.data.Pop();
                    position = current._position;
                }

                // TODO Xmlns could be explicitly reset (xmlns="") (uncommon)
                if (string.IsNullOrEmpty(result._namespace) && this.data.Count > 0)
                    result._namespace = this.data.Peek()._namespace;
            }

            result._depth = data.Count;
            result._position = position;
            this.data.Push(result);
            // TODO Merge metadata nodes; handle pt:property
            return _elisionPosition < names.Length - 1;
        }

        // IXmlLineInfo
        public override int LineNumber {
            get { return Peek().LineNumber; } }

        public override int LinePosition {
            get { return Peek().LinePosition; } }

        public bool HasLineInfo() {
            return readerLineInfoCache != null;
        }

        void Moved() {
            if (this.reader.ReadState == XReadState.Initial || this.data.Count == 0)
                throw PropertyTreesFailure.ReaderNotMoved();
        }

        public override ReadState ReadState {
            get {
                switch (this.reader.ReadState) {
                    case System.Xml.ReadState.Initial:
                        return ReadState.Initial;

                    case System.Xml.ReadState.Interactive:
                        return ReadState.Interactive;

                    case System.Xml.ReadState.Error:
                        return ReadState.Error;

                    case System.Xml.ReadState.EndOfFile:
                        return ReadState.EndOfFile;

                    case System.Xml.ReadState.Closed:
                    default:
                        return ReadState.Closed;
                }
            }
        }

        public override int Position {
            get {
                Moved();
                return Peek()._position;
            }
        }

        public override bool HasChildren {
            get {
                Moved();
                throw new NotImplementedException();
            }
        }

        // `IUriContext' implementation
        Uri IUriContext.BaseUri {
            get {
                return BaseUri;
            }
            set {

            }
        }

        // `IXmlNamespaceResolver' implementation
        IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope) {
            return ((IXmlNamespaceResolver) this.reader).GetNamespacesInScope(scope);
        }

        string IXmlNamespaceResolver.LookupNamespace(string prefix) {
            return ((IXmlNamespaceResolver) this.reader).LookupNamespace(prefix);
        }

        string IXmlNamespaceResolver.LookupPrefix(string namespaceName) {
            return ((IXmlNamespaceResolver) this.reader).LookupPrefix(namespaceName);
        }

        private IDictionary<string, string> EnsurePrefixMap() {
            var prefixMap = Peek().prefixMap;
            if (prefixMap == null)
                Peek().prefixMap = prefixMap = new SortedList<string, string>();

            return prefixMap;
        }

        private object ParseExpressions(PropertyNodeType nodeType, string value) {
            if (nodeType != PropertyNodeType.Property) {
                return value;
            }

            RuntimeExpression expr;
            if (ExpressionUtility.TryParse(value, out expr)) {
                return expr;
            }
            return value;
        }
    }
}

