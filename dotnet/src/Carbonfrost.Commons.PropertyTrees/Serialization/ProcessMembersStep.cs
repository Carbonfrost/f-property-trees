//
// - ProcessMembersStep.cs -
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
using System.Linq;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        interface IApplyMemberStep {
            bool Apply(PropertyTreeBinderImpl parent, PropertyTreeMetaObject target, PropertyTreeNavigator node);
        }

        class ProcessMembersStep : PropertyTreeBinderStep {

            readonly IApplyMemberStep[] _items;

            public ProcessMembersStep(params IApplyMemberStep[] items) {
                this._items = items;
            }

            public override PropertyTreeMetaObject Process(PropertyTreeBinderImpl parent, PropertyTreeMetaObject target, PropertyTreeNavigator self, NodeList children) {
                target.ApplyUriContextToProperties(self.BaseUri);

                children.FindAndRemove(t => Apply(parent, target, t)).All();
                return target;
            }

            static void CopyContextFromParent(object component, IUriContext parent) {
                var uc = component as IUriContext;
                if (uc != null) {
                    uc.BaseUri = parent.BaseUri;
                }
            }

            // TODO Implement property ordering based on [DependsOn]
            private bool Apply(PropertyTreeBinderImpl parent, PropertyTreeMetaObject target, PropertyTreeNavigator node) {
                return _items.Any(t => t.Apply(parent, target, node));
            }
        }
    }
}
