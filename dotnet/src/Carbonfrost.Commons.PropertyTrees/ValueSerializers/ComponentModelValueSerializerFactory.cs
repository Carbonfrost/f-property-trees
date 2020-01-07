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
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.ValueSerializers;

[assembly: ValueSerializerFactory(typeof(ComponentModelValueSerializerFactory))]

namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    sealed class ComponentModelValueSerializerFactory : AdapterFactory {

        private static IDictionary<Type, Type> _intrinsics;
        private static readonly IDictionary<Type, Type> _nullables = new Dictionary<Type, Type>();

        internal static IDictionary<Type, Type> Intrinsics {
            get {
                return _intrinsics ?? (_intrinsics = CreateIntrinsics());
            }
        }

        public override Type GetAdapterType(Type adapteeType, string adapterRoleName) {
            if (!string.Equals(adapterRoleName, AdapterRole.ValueSerializer, StringComparison.OrdinalIgnoreCase)) {
                return null;
            }

            var nullableType = Nullable.GetUnderlyingType(adapteeType);
            if (nullableType != null) {
                // TODO We could end up caching nullable instances which have no backing value serializer (performance)
                // : Switch to a weak reference dictionary and do more validation
                return _nullables.GetValueOrCache(adapteeType,
                                                  _ => typeof(NullableValueSerializer<>).MakeGenericType(nullableType));
            }

            if (adapteeType.GetTypeInfo().IsEnum) {
                adapteeType = typeof(Enum);
            }

            return Intrinsics.GetValueOrDefault(adapteeType);
        }

        static IDictionary<Type, Type> CreateIntrinsics() {
            var dict = new Dictionary<Type, Type> {
                { typeof(bool), typeof(BooleanValueSerializer) },
                { typeof(byte), typeof(ByteValueSerializer) },
                { typeof(SByte), typeof(SByteValueSerializer) },
                { typeof(char), typeof(CharValueSerializer) },
                { typeof(double), typeof(DoubleValueSerializer) },
                { typeof(string), typeof(StringValueSerializer) },
                { typeof(int), typeof(Int32ValueSerializer) },
                { typeof(short), typeof(Int16ValueSerializer) },
                { typeof(long), typeof(Int64ValueSerializer) },
                { typeof(float), typeof(SingleValueSerializer) },
                { typeof(UInt16), typeof(UInt16ValueSerializer) },
                { typeof(UInt32), typeof(UInt32ValueSerializer) },
                { typeof(UInt64), typeof(UInt64ValueSerializer) },
                { typeof(byte[]), typeof(Base16ValueSerializer) },
                { typeof(DateTime), typeof(DateTimeValueSerializer) },
                { typeof(DateTimeOffset), typeof(DateTimeOffsetValueSerializer) },
                { typeof(Decimal), typeof(DecimalValueSerializer) },
                { typeof(TimeSpan), typeof(TimeSpanValueSerializer) },
                { typeof(Guid), typeof(GuidValueSerializer) },
                { typeof(Type), typeof(TypeValueSerializer) },
                { typeof(Uri), typeof(UriValueSerializer) },
                { typeof(CultureInfo), typeof(CultureInfoValueSerializer) },
                { typeof(Version), typeof(VersionValueSerializer) },
                { typeof(Enum), typeof(EnumValueSerializer) },
                { typeof(FrameworkName), typeof(FrameworkNameValueSerializer) },
                { typeof(Encoding), typeof(EncodingValueSerializer) },
                { typeof(HashAlgorithm), typeof(HashAlgorithmValueSerializer) },
                { typeof(SymmetricAlgorithm), typeof(SymmetricAlgorithmValueSerializer) },
                { typeof(Regex), typeof(RegexValueSerializer) },

                { typeof(ContentType), typeof(ContentTypeValueSerializer) },
                { typeof(Glob), typeof(GlobValueSerializer) },
                { typeof(NamespaceUri), typeof(NamespaceUriValueSerializer) },
                { typeof(Properties), typeof(PropertiesValueSerializer) },
                { typeof(QualifiedName), typeof(QualifiedNameValueSerializer) },
                { typeof(StreamContext), typeof(StreamContextValueSerializer) },
                { typeof(TypeReference), typeof(TypeReferenceValueSerializer) },
                { typeof(GlobTemplate), typeof(GlobTemplateValueSerializer) },
            };

            foreach (var type in typeof(ComponentModelValueSerializerFactory).GetTypeInfo().Assembly.GetTypes()) {
                // TODO Initialize the dictionary without reflection (performance)
                foreach (ValueSerializerAttribute attr in type.GetCustomAttributes(typeof(ValueSerializerAttribute), false)) {
                    dict[type] = attr.ValueSerializerType;
                }
            }
            return dict;
        }
    }
}

