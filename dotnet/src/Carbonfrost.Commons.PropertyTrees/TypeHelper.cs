//
// Copyright 2010 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Reflection;

using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    static class TypeHelper {

        static readonly Type EnumVSType = ValueSerializer.GetValueSerializerType(typeof(BindingFlags));

        public static bool IsParameterRequired(Type type) {
            return type.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(type) == null;
        }

        public static MethodBase FindActivationConstructor(Type declaringType) {
            MethodBase ctor = declaringType.GetActivationConstructor();
            if (ctor == null) {
                ctor = declaringType.GetConstructors().FirstOrDefault();
            }

            return ctor;
        }

        public static Type GetReturnType(MethodBase method) {
            if (method.IsConstructor)
                return ((ConstructorInfo) method).DeclaringType;
            else
                return ((MethodInfo) method).ReturnType;
        }

        public static IValueSerializer GetConverter(PropertyInfo property, Type neededType) {
            if (neededType == typeof(bool)) {
                return BooleanConverterExtension.Instance;
            }

            IValueSerializer conv = null;
            if (property != null) {
                conv = NotInvalid(ValueSerializer.GetValueSerializer(property));
            }

            conv = conv ?? NotInvalid(ValueSerializer.GetValueSerializer(neededType));

            // Don't use the built-in enum value serializer
            if (EnumVSType.IsInstanceOfType(conv)) {
                return EnumValueSerializerExtension.Instance;
            }

            var colType = ListConverter.FindICollectionType(neededType);
            if (colType != null)
                return ListConverter.Instance;

            return conv ?? ValueSerializer.Invalid;
        }

        static IValueSerializer NotInvalid(IValueSerializer vs) {
            if (vs == ValueSerializer.Invalid)
                return null;
            return vs;
        }

        public static Uri ConvertToUri(object value) {
            // TODO Need a uri context?
            if (object.ReferenceEquals(value, null))
                return null;
            else
                return Utility.NewUri(value.ToString());
        }

        public static TimeSpan ConvertToTimeSpan(object value) {
            if (object.ReferenceEquals(value, null))
                return TimeSpan.Zero;
            else
                return TimeSpan.Parse(value.ToString());
        }

        public static Type TypeOf(object value, Type fallback = null) {
            if (value == null)
                return fallback ?? typeof(object);
            else
                return value.GetType();
        }

        public static Type ConvertToType(object value, IServiceProvider context) {
            if (value == null)
                return null;

            string s = value as string;
            if (s != null)
                return TypeReference.Parse(s, context).Resolve();

            return value as Type;
        }

        public static string GetNamespaceName(Type type) {
            var tt = type.GetTypeInfo();
            if (tt.IsGenericType && !tt.IsGenericTypeDefinition) {
                var def = tt.GetGenericTypeDefinition();

                if (def == typeof(IAddChild<>) || def == typeof(IEnumerable<>) || def == typeof(ICollection<>) || def == typeof(IList<>))
                    return GetNamespaceName(tt.GetGenericArguments()[0]);

                else if (def == typeof(IDictionary<,>) || def == typeof(KeyValuePair<,>))
                    return GetNamespaceName(tt.GetGenericArguments()[1]);

                else
                    return string.Empty;
            }

            if (tt.IsGenericParameter)
                return string.Empty;

            if (tt.IsArray || tt.IsByRef || tt.IsPointer)
                return GetNamespaceName(tt.GetElementType());

            return type.GetQualifiedName().NamespaceName;
        }
    }
}
