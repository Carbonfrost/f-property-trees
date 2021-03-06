//
// - FrameworkNameValueSerializer.cs -
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
using System.Runtime.Versioning;
using System.Text.RegularExpressions;


namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    sealed class FrameworkNameValueSerializer : ValueSerializer {

        static readonly Regex PATTERN = new Regex(
            @"^(?<Identifier> [-._0-9a-z]+) \s* , \s*
              Version=v(?<Version> [.0-9]+) \s* (, \s*
              Profile=(?<Profile> [^,]+))? ",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            if (text != null) {
                FrameworkName result = ParseInstance(text);
                if (result != null)
                    return result;
            }

            return base.ConvertFromString(text, destinationType, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context) {
            if (value != null) {
                FrameworkName ha = (FrameworkName) value;
                return ha.FullName;
            }
            return base.ConvertToString(value, context);
        }

        static FrameworkName ParseInstance(string s) {
            Match m = PATTERN.Match(s);
            if (m.Success) {
                string identifier = m.Groups["Identifier"].Value;
                Version version = new Version(m.Groups["Version"].Value);
                string profile = m.Groups["Profile"].Value;

                if (string.IsNullOrEmpty(profile))
                    return new FrameworkName(identifier, version);
                else
                    return new FrameworkName(identifier, version, profile);
            }

            return null;
        }

    }
}
