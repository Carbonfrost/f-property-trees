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
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    public class ValueSerializerFactory : AdapterFactory<IValueSerializer> {

        public static readonly ValueSerializerFactory Default
            = new ValueSerializerFactory(AdapterFactory.Default);

        protected ValueSerializerFactory(IAdapterFactory implementation)
            : base(AdapterRole.ValueSerializer, implementation) {
        }

        public IValueSerializer GetValueSerializer(Type componentType) {
            if (componentType == null) {
                throw new ArgumentNullException("componentType"); // $NON-NLS-1
            }

            Type result = GetValueSerializerType(componentType);
            if (result == null) {
                return null;
            }

            return Activation.CreateInstance<IValueSerializer>(result);
        }

        public virtual Type GetValueSerializerType(Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");
            return GetAdapterType(componentType);
        }

    }
}
