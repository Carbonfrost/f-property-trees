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

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees {

    static class PropertyAttribute {

        static readonly NamespaceUri pt = Xmlns.PropertyTrees2010;

        public static readonly QualifiedName Value = pt + "value";
        public static readonly QualifiedName Position = pt + "position";
        public static readonly QualifiedName Browsable = pt + "browsable";
        public static readonly QualifiedName Category = pt + "category";
        public static readonly QualifiedName DefaultValue = pt + "defaultValue";
        public static readonly QualifiedName Enabled = pt + "enabled";
        public static readonly QualifiedName FullName = pt + "fullName";
        public static readonly QualifiedName IsReadOnly = pt + "readOnly";
        public static readonly QualifiedName PropertyType = pt + "propertyType";
        public static readonly QualifiedName Source = pt + "source";
    }
}
