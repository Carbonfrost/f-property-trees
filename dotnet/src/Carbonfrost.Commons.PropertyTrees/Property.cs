//
// Copyright 2010, 2012, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees {

    public partial class Property : PropertyNode {

        private object _value;

        public event EventHandler ValueChanged;

        public Property(QualifiedName name) : base(name) {
        }

        public Property(string localName) : base(localName) {
        }

        public Property(string localName, string ns) : base(localName, ns) {
        }

        // PropertyNode overrides
        public override object Value {
            get { return _value; }
            set {
                // UNDONE Check the type
                if (_value != value) {
                    _value = value;
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnValueChanged(EventArgs e) {
            if (ValueChanged != null) {
                ValueChanged(this, e);
            }
        }

        public override PropertyNodeCollection Children {
            get {
                return EmptyPropertyNodeCollection.Instance;
            }
        }

        public override void CopyTo(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.IsPropertyTree) {
                PropertyTreeWriter w = new PropertyTreeNodeWriter((PropertyTree) node);
                WriteTo(w);

            } else {
                // TODO Possible that Value isn't appropriate for Property
                Property p = (Property) node;
                p.QualifiedName = QualifiedName;
                p.Value = Value;
            }
        }

        public new Property Clone() {
            return new Property(QualifiedName) { Value = Value };
        }

        protected override PropertyNode CloneCore() {
            return Clone();
        }

        public new Property RemoveSelf() {
            return (Property) base.RemoveSelf();
        }

        public override PropertyNodeType NodeType {
            get { return PropertyNodeType.Property; } }

        public override PropertyNodeDefinition Definition {
            get {
                throw new NotImplementedException();
            }
        }

        protected internal override void AcceptVisitor(PropertyTreeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitProperty(this);
        }

        protected internal override TResult AcceptVisitor<TArgument, TResult>(PropertyTreeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitProperty(this, argument);
        }

    }
}
