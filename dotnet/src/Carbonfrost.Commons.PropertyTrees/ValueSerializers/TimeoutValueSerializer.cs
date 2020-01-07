//
// - TimeoutValueSerializer.cs -
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
using System.Globalization;


namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    sealed class TimeoutValueSerializer : ValueSerializer {

        public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            string s = (text ?? string.Empty).Trim();
            if (s.Length > 0) {

                int k;
                if (Int32.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out k) && k == -1)
                    return -1;

                TimeSpan ts = TimeSpanValueSerializer.ParseTimeSpan(s);
                if (ts >= TimeSpan.Zero)
                    return (int) Math.Ceiling(ts.TotalMilliseconds);
            }

            return base.ConvertFromString(text, destinationType, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context) {
            if (value == null)
                throw new ArgumentNullException("value"); // $NON-NLS-1

            if (value is TimeSpan || value is long || value is int) {
                return value.ToString();
            }

            return base.ConvertToString(value, context);
        }
    }
}
