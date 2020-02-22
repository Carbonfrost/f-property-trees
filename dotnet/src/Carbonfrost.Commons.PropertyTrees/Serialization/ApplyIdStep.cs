//
// Copyright 2015, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        class ApplyIdStep : PropertyTreeBinderStep {

            public override PropertyTreeMetaObject Process(PropertyTreeBinderImpl parent, PropertyTreeMetaObject target, PropertyTreeNavigator self, NodeList children) {
                Predicate<PropertyTreeNavigator> predicate = ImplicitDirective(target, "id");

                var node = children.FindAndRemove(predicate).FirstOrDefault();
                if (node == null) {
                    node = children.FindAndRemove("name").FirstOrDefault();
                }
                if (node != null) {
                    // TODO Handle when a name is duplicated or contains whitespace
                    var ns = parent.FindNameScope(target);
                    string id = Convert.ToString(node.Value);

                    if (string.IsNullOrEmpty(id)) {
                        parent._errors.IdCannotBeBlank(node.FileLocation);
                    } else if (ns.FindName(id) == null) {
                        ns.RegisterName(id, target.Component);
                    } else {
                        parent._errors.IdAlreadyRegistered(id, node.FileLocation);
                    }

                    var nameProperty = target.SelectProperty(NamespaceUri.Default + "name");
                    if (nameProperty != null) {
                        parent.DoPropertyBind(target, node, nameProperty);
                    }
                }

                return target;
            }

        }
    }
}
