//
// Copyright 2010, 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    sealed class TimeSpanValueSerializer : ValueSerializer {

        public override string ConvertToString(object value, IValueSerializerContext context) {
            if (value is TimeSpan)
                return value.ToString();

                return base.ConvertToString(value, context);
        }

        public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            string s = text;
            if (s != null)
                return ParseTimeSpan(s);
            else
                return base.ConvertFromString(text, destinationType, context);
        }

        internal static TimeSpan ParseTimeSpan(string text) {
            if (string.IsNullOrWhiteSpace(text))
                return TimeSpan.Zero;

            text = text.Trim();

            if (text == "max" || text == "Infinite")
                return TimeSpan.MaxValue;

            else if (text == "min")
                return TimeSpan.MinValue;

            TimeSpan result;
            if (Utility.ConvertUnits(text, UNITS, CONVERSIONS, out result)) {
                return result;
            }

            return TimeSpan.Parse(text);
        }

        static readonly string[] UNITS = new string[] {
            "d",
            "h",
            "ms",
            "min",
            "s",
        };

        static readonly Func<Double, TimeSpan>[] CONVERSIONS =
            new Func<Double, TimeSpan>[] {
            TimeSpan.FromDays,
            TimeSpan.FromHours,
            TimeSpan.FromMilliseconds,
            TimeSpan.FromMinutes,
            TimeSpan.FromSeconds,
        };
    }
}
