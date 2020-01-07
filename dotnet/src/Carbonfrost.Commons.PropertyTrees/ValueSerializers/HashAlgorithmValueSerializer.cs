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
using System.Reflection;
using System.Security.Cryptography;

namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    sealed class HashAlgorithmValueSerializer : ValueSerializer {

        // MD5
        // HMAC:MD5:00000000000000000000

        public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            string s = text;
            if (s != null)
                return ParseHashAlgorithm(s);
            else
                return base.ConvertFromString(text, destinationType, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context) {
            HashAlgorithm ha = (HashAlgorithm) value;
            KeyedHashAlgorithm k = ha as KeyedHashAlgorithm;

            string name = MapToName(ha);
            if (k == null)
                return name;
            else
                return string.Concat(name, ':', Utility.Hex(k.Key));
        }

        static string MapToName(HashAlgorithm ha) {
            HMAC hmac = ha as HMAC;
            if (hmac != null)
                return "HMAC:" + hmac.HashName;

            // Look for the first descendent of HashAlgorithm or KeyedHashAlgorithm
            TypeInfo type = ha.GetType().GetTypeInfo();
            while (type.BaseType != typeof(KeyedHashAlgorithm)
                   && type.BaseType != typeof(HashAlgorithm)) {
                type = type.BaseType.GetTypeInfo();
            }

            return type.Name;
        }

        // UNDONE Implement parse hash algorithm
        static HashAlgorithm ParseHashAlgorithm(string s) {
            string[] parts = s.Split(':');
            switch (parts.Length) {
                case 1:
                    return CreateHashAlgorithm(parts[0]);
                case 2:
                case 3:
                    return CreateHMAC(parts[0]);
            }
            throw new NotImplementedException();
        }

        static HashAlgorithm CreateHashAlgorithm(string name) {
            // TODO The name can also be qualified with the namespace (compatibility)
            return HashAlgorithm.Create(name);
        }

        static HMAC CreateHMAC(string name) {
            return HMAC.Create(name);
        }

    }
}
