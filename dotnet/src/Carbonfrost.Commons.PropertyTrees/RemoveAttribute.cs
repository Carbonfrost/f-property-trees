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
using System.Reflection;

using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class RemoveAttribute : Attribute, IRoleAttribute {

        internal static readonly RemoveAttribute Default = new RemoveAttribute();

        public string Name { get; set; }

        string IRoleAttribute.ComputeName(MethodBase method) {
            return string.IsNullOrEmpty(Name) ? "Remove" : Name;
        }

        void IRoleAttribute.ProcessExtensionMethod(MethodInfo mi) {
        }

        OperatorDefinition IRoleAttribute.BuildInstance(MethodInfo method) {
            return new ReflectedRemoveDefinition(this, method);
        }
    }
}
