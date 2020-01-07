//
// - IndexerUsingIPropertiesPropertyDefinition.cs -
//
// Copyright 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    sealed class IndexerUsingIPropertiesPropertyDefinition : PropertyDefinition {

        private bool _readWrite;

        public IndexerUsingIPropertiesPropertyDefinition(bool readWrite) {
            _readWrite = readWrite;
        }

        public override PropertyTreeDefinition DeclaringTreeDefinition {
            get {
                return null;
            }
        }

        public override object DefaultValue {
            get {
                return null;
            }
        }

        public override bool IsParamArray {
            get {
                return false;
            }
        }

        public override Type PropertyType {
            get {
                return typeof(object);
            }
        }

        public override bool IsIndexer {
            get {
                return true;
            }
        }

        public override bool IsOptional {
            get {
                return true;
            }
        }

        public override string Namespace {
            get {
                return string.Empty;
            }
        }

        public override string Name {
            get {
                return "GetProperty";
            }
        }

        public override bool IsReadOnly {
            get {
                return !_readWrite;
            }
        }

        // TODO Define behavior of qualified names
        public override bool TryGetValue(object component, object ancestor, QualifiedName name, out object result) {
            return ((IPropertyProvider) ActualProperties(component)).TryGetProperty(name.LocalName, typeof(object), out result);
        }

        public override void SetValue(object component, object ancestor, QualifiedName name, object value) {
            ((IProperties) ActualProperties(component)).SetProperty(name.LocalName, value);
        }

        private static object ActualProperties(object component) {
            if (component == null) {
                throw new ArgumentNullException("component");
            }
            var container = component as IPropertiesContainer;
            if (container != null) {
                return container.Properties;
            }
            return component;
        }
    }
}
