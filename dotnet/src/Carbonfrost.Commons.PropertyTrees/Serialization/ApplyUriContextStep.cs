//
// - ApplyUriContextStep.cs -
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
using System.Linq;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        class ApplyUriContextStep : PropertyTreeBinderStep {

            public override PropertyTreeMetaObject Process(PropertyTreeBinderImpl parent, PropertyTreeMetaObject target, PropertyTreeNavigator self, NodeList children) {
                Predicate<PropertyTreeNavigator> predicate = ImplicitDirective(target, "base");

                var node = children.FindAndRemove(predicate).FirstOrDefault();

                Uri baseUri;
                IServiceProvider serviceProvider;

                if (node == null) {
                    baseUri = self.BaseUri;
                    serviceProvider = parent;
                } else {
                    baseUri = Utility.NewUri(Convert.ToString(node.Value));
                    serviceProvider = parent.GetBasicServices(node);
                }
                return target.BindBaseUri(baseUri, serviceProvider);
            }

        }
    }
}
