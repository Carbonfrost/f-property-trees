//
// Copyright 2014, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Collections;
using System.Collections.Generic;

using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class CollectionMetaObject : PropertyTreeMetaObject {

        private readonly object component;

        public CollectionMetaObject(object component) {
            this.component = component;
        }

        public override Type ComponentType {
            get { return component.GetType(); }
        }

        public override object Component {
            get {
                return component;
            }
        }

        public override void BindRemoveChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            definition.Apply(Component, null, arguments);
        }

        public override void BindClearChildren(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            definition.Apply(Component, null, arguments);
        }

        public override PropertyTreeMetaObject BindInitializeValue(string text, IServiceProvider serviceProvider) {
            object value;
            if (TryConvertFromText(text, serviceProvider, out value))
                return PropertyTreeMetaObject.Create(value);
            else
                throw new NotImplementedException();
        }

        public override PropertyTreeMetaObject BindInitializer(Expression expression, IExpressionContext context, IServiceProvider serviceProvider) {
            var values = expression.Evaluate(context);
            if (values == null) {
                return this;
            }
            if (!(values is IEnumerable)) {
                values = new [] { values } ;
            }

            var myValues = ReflectionMetaObject.GetAddonElements((IEnumerable) values);
            var addon = ReflectionMetaObject.FindAddonMethod(ComponentType, myValues);
            if (addon == null) {
                var errors = serviceProvider.GetServiceOrDefault(PropertyTreeBinderErrors.Default);
                errors.NoAddMethodSupported(component.GetType(), PropertyTreeBinderImpl.FindFileLocation(serviceProvider));
                return this;
            }
            foreach (var item in myValues) {
                addon.Invoke(Component, new object[] {
                                 item
                             });
            }
            return this;
        }

        public override PropertyTreeMetaObject BindAddChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            var result = definition.Apply(component, component, arguments);
            if (result == null)
                return PropertyTreeMetaObject.Null;
            return this.CreateChild(result);
        }

    }

}
