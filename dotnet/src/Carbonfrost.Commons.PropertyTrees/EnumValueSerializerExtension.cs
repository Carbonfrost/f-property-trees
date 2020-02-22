//
// Copyright 2014, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Globalization;

namespace Carbonfrost.Commons.PropertyTrees {

    sealed class EnumValueSerializerExtension : ValueSerializer {

        private static readonly char[] WS =  {
            ' ',
            ',', // sic
            '\t',
            '\r',
            '\n'
        };

        public static readonly IValueSerializer Instance = new EnumValueSerializerExtension();
        static readonly IValueSerializer Base = ValueSerializer.GetValueSerializer(typeof(Enum));

        public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            if (text != null && text.IndexOfAny(WS) >= 0) {
                long num = 0;
                foreach (var item in text.Split(WS, StringSplitOptions.RemoveEmptyEntries)) {
                    string a = InflectedName(item);
                    num |= Convert.ToInt64((Enum) Enum.Parse(destinationType, a, /* ignoreCase */ true), CultureInfo.InvariantCulture);
                }

                return Enum.ToObject(destinationType, num);
            }

            return Base.ConvertFromString(InflectedName(text), destinationType, context);
        }

        static string InflectedName(string item) {
            var names = item.Split('-');
            return string.Concat(names);
        }
    }
}
