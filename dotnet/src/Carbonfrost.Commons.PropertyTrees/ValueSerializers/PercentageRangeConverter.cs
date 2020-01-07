//
// - PercentageRangeConverter.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Runtime.Versioning;

using Carbonfrost.Commons.Core;


namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    sealed class PercentageRangeConverter : ValueSerializer {

        public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            if (text != null) {
                Type type = null;
                if (context != null && context.Property != null) {
                    type = context.Property.PropertyType;
                }
                return ConvertCore(type, text);
            }

            return base.ConvertFromString(text, destinationType, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context) {
            throw new NotImplementedException();
        }

        static object ConvertCore(Type type, string text) {
            if (text == null)
                throw new ArgumentNullException("text");
            if (text.Length == 0)
                throw Failure.EmptyString("text");

            text = text.Trim();
            bool percent = false;
            if (text.EndsWith("%", StringComparison.Ordinal)) {
                text = text.Substring(0, text.Length - 1);
                percent = true;
            } else if (text.EndsWith("pc", StringComparison.Ordinal)) {
                text = text.Substring(0, text.Length - 2);
                percent = true;
            }

            if (typeof(float).Equals(type))
                return float.Parse(text) / (percent ? 100.0f : 1.0f);
            else if (typeof(decimal).Equals(type))
                return decimal.Parse(text) / (percent ? 100.0m : 1.0m);
            else
                return double.Parse(text) / (percent ? 100.0d : 1.0d);
        }

        public static double ConvertToDouble(string text) {
            return (double) ConvertCore(typeof(double), text);
        }

        public static float ConvertToSingle(string text) {
            return (float) ConvertCore(typeof(float), text);
        }

        public static decimal ConvertToDecimal(string text) {
            return (decimal) ConvertCore(typeof(decimal), text);
        }
    }
}
