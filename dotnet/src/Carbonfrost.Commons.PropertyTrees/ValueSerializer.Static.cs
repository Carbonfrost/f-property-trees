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
using System.Reflection;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.PropertyTrees.ValueSerializers;

namespace Carbonfrost.Commons.PropertyTrees {

    [Providers]
    public partial class ValueSerializer {

        public static readonly IValueSerializer Default = new TransparentValueSerializer();
        public static readonly IValueSerializer Invalid = new InvalidValueSerializer();

        public static readonly IValueSerializer Percentage = new PercentageValueSerializer();
        public static readonly IValueSerializer PercentageRange = new PercentageValueSerializer();
        public static readonly IValueSerializer Timeout = new TimeoutValueSerializer();

        [ValueSerializerUsage(Name = "hex")]
        public static readonly IValueSerializer Base16 = new Base16ValueSerializer();
        public static readonly IValueSerializer Base64 = new Base64ValueSerializer();

        public static IValueSerializer GetValueSerializer(ParameterInfo parameter) {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            return CreateInstance(GetValueSerializerType(parameter));
        }

        public static IValueSerializer GetValueSerializer(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");

            return CreateInstance(GetValueSerializerType(property));
        }

        public static Type GetValueSerializerType(ParameterInfo parameter) {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            var gen = (ValueSerializerAttribute) parameter.GetCustomAttribute(typeof(ValueSerializerAttribute));
            return gen == null ? GetValueSerializerType(parameter.ParameterType) : gen.ValueSerializerType;
        }

        public static Type GetValueSerializerType(PropertyInfo property) {
            if (property == null) {
                throw new ArgumentNullException("property");
            }

            var gen = property.GetCustomAttribute<ValueSerializerAttribute>();
            return gen == null ? GetValueSerializerType(property.PropertyType) : gen.ValueSerializerType;
        }

        public static IValueSerializer GetValueSerializer(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            return ValueSerializerFactory.Default.GetValueSerializer(type) ?? Invalid;
        }

        public static Type GetValueSerializerType(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            return ValueSerializerFactory.Default.GetValueSerializerType(type);
        }

        public static IValueSerializer FromName(string name) {
            return App.GetProvider<IValueSerializer>(name);
        }

        static IValueSerializer GetValueSerializerFromAttributes(ValueSerializerAttribute gen) {
            Type type = gen.ValueSerializerType;
            return type == null ? null : Activation.CreateInstance<IValueSerializer>(type);
        }

        static IValueSerializer CreateInstance(Type type) {
            if (type == null) {
                return Invalid;
            }
            return (IValueSerializer) Activation.CreateInstance(type);
        }
    }
}
