//
// - TransparentValueSerializer.cs -
//
// Copyright 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;


namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    class TransparentValueSerializer : IValueSerializer {

        public object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            var vs = GetSerializer(destinationType, context);
            return vs.ConvertFromString(text, destinationType, context);
        }

        public string ConvertToString(object value, IValueSerializerContext context) {
            var vs = GetSerializer(value.GetType(), context);
            return vs.ConvertToString(value, context);
        }

        private IValueSerializer GetSerializer(Type destinationType, IValueSerializerContext context) {
            IValueSerializer result = null;
            if (context != null) {
                if (context.Parameter != null) {
                    result = ValueSerializer.GetValueSerializer(context.Parameter);
                } else  if (context.Property != null) {
                    result = ValueSerializer.GetValueSerializer(context.Property);
                }
            }
            return result ?? ValueSerializer.GetValueSerializer(destinationType) ?? ValueSerializer.Invalid;
        }
    }
}
