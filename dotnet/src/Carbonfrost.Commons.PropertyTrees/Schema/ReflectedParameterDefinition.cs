//
// - ReflectedParameterDefinition.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Reflection;

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    sealed class ReflectedParameterDefinition : PropertyDefinition {

        private readonly ParameterInfo parameter;
        private readonly string ns;

        public ReflectedParameterDefinition(string ns, ParameterInfo pi) {
            this.parameter = pi;
            this.ns = ns;
        }

        public override PropertyTreeDefinition DeclaringTreeDefinition {
            get {
                return null;
            }
        }

        public override bool IsParamArray {
            get {
                return this.parameter.IsDefined(typeof(ParamArrayAttribute), false);
            }
        }

        public override object DefaultValue {
            get {
                return parameter.DefaultValue;
            }
        }

        public override bool IsOptional {
            get {
                return parameter.IsOptional;
            }
        }

        public override string Namespace {
            get {
                return this.ns;
            }
        }

        public override string Name {
            get {
                return parameter.Name;
            }
        }

        public override Type PropertyType {
            get {
                return parameter.ParameterType;
            }
        }

        public override bool IsReadOnly {
            get {
                return true;
            }
        }

        public override bool TryGetValue(object component, object ancestor, QualifiedName name, out object result) {
            result = null;
            return false;
        }

        public override void SetValue(object component, object ancestor, QualifiedName name, object value) {
        }
    }

}
