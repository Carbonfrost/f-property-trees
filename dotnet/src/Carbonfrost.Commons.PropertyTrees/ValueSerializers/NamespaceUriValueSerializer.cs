//
// Copyright 2005, 2006, 2010, 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    sealed class NamespaceUriValueSerializer : ValueSerializer {

        public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            var s = text;
            if (s != null)
                return NamespaceUri.Parse(s);

            return base.ConvertFromString(text, destinationType, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context) {
            if (value is NamespaceUri)
                return value.ToString();

            return base.ConvertToString(value, context);
        }
    }
}

