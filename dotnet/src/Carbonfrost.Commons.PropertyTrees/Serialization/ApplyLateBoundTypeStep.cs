//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;

using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        class ApplyLateBoundTypeStep : PropertyTreeBinderStep {

            public override PropertyTreeMetaObject Process(PropertyTreeBinderImpl parent, PropertyTreeMetaObject target, PropertyTreeNavigator self, NodeList children) {
                Type type = target.ComponentType ?? typeof(object);
                Type concreteClass = type.GetConcreteClass();
                QualifiedName name = self.QualifiedName;
                IServiceProvider serviceProvider = parent;

                if (concreteClass != null && type != concreteClass) {
                    target.BindTargetType(TypeReference.FromType(concreteClass), serviceProvider);

                } else if (type.IsComposable()) {
                    target.BindTargetProvider(name, null, serviceProvider);
                }

                if (target.Component == null && !target.ComponentType.GetTypeInfo().IsSealed) {
                    // TODO This predicate is probably too loose
                    Predicate<PropertyTreeNavigator> predicate = t => t.Name == "type";
                    var node = children.FindAndRemove(predicate).FirstOrDefault();
                    ApplyType(parent, target, node);
                }
                return PickBuilderTypeIfAvailable(target);
            }

            private void ApplyType(PropertyTreeBinderImpl parent,
                                   PropertyTreeMetaObject target,
                                   PropertyTreeNavigator node) {

                if (node == null)
                    return;

                var serviceProvider = parent.GetBasicServices(node);
                target.BindTargetType(parent.DirectiveFactory.CreateTargetType(node), serviceProvider);
            }

            private PropertyTreeMetaObject PickBuilderTypeIfAvailable(PropertyTreeMetaObject target) {
                if (target.Component == null && target.ComponentType != null) {

                    Type builderType = target.ComponentType.GetBuilderType();
                    if (builderType != null)
                        return new BuilderMetaObject(new PreactivationMetaObject(builderType));
                }

                return target;
            }
        }

    }

}
