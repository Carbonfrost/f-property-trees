//
// - PropertyNodeDefinition.cs -
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

using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public abstract class PropertyNodeDefinition {

        // TODO Validation, converters, value generators

        public QualifiedName QualifiedName {
            get {
                return QualifiedName.Create(this.Namespace, this.Name);
            }
        }

        internal string ShortName {
            get {
                return Name.Substring(Name.IndexOf('.') + 1);
            }
        }

        public abstract string Name { get; }
        public abstract string Namespace { get; }

        public override string ToString() {
            return this.QualifiedName.ToString();
        }

    }
}
