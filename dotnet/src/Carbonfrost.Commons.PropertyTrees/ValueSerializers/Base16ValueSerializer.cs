//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Text.RegularExpressions;

namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    sealed class Base16ValueSerializer : ValueSerializer {

        public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            if (destinationType == typeof(byte[])) {
                return Unhex(text);
            }
            return base.ConvertFromString(text, destinationType, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context) {
            var bytes = value as byte[];
            if (bytes != null) {
                return Utility.Hex(bytes);
            }
            return base.ConvertToString(value, context);
        }

        static byte[] Unhex(string hexstring) {
            if (string.IsNullOrWhiteSpace(hexstring))
                return new byte[0];

            hexstring = hexstring.Replace('\n', ' ').Replace('\r', ' ');
            hexstring = Regex.Replace(hexstring, @"\s+", "");
            if (0 != (hexstring.Length % 2))
                throw new ArgumentException();

            byte[] result = new byte[hexstring.Length / 2];
            int j = 0;
            for (int i = 0; i < hexstring.Length; i += 2) {
                result[j++] = Convert.ToByte(hexstring.Substring(i, 2), 16);
            }
            return result;
        }
    }
}

