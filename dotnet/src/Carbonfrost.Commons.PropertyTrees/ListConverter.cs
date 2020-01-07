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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Carbonfrost.Commons.PropertyTrees {

    struct ListConverter : IValueSerializer {

        static readonly MethodInfo GenericAddItemsMethod = typeof(ListConverter)
            .GetMethod("GenericAddItems", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly char[] WS = {
            ' ',
            '\t',
            '\r',
            '\n'
        };

        public static IValueSerializer Instance {
            get {
                return new ListConverter();
            }
        }

        public object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            Type collectionType = FindICollectionType(destinationType);
            Type itemType = collectionType.GetGenericArguments()[0];
            Array array = SplitItems(text, itemType);
            object result;

            if (destinationType.GetTypeInfo().IsInterface) {
                destinationType = typeof(List<>).MakeGenericType(itemType);
                result = Activator.CreateInstance(destinationType, array);

            } else {
                result = Activator.CreateInstance(destinationType);
                GenericAddItemsMethod.MakeGenericMethod(itemType)
                    .Invoke(null, new[] { result, array });
            }

            return result;
        }

        private static void GenericAddItems<T>(ICollection<T> target, Array items) {
            foreach (object o in items) {
                target.Add((T) o);
            }
        }

        internal static Type FindICollectionType(Type type) {
            // TODO We call this twice (once from TypeHelper, then in ConvertFromString) - cache (perf)
            // TODO Could be ambiguous if type implements ICollection<> multiple times
            var tt = type.GetTypeInfo();
            var resultTypeInfo = tt.GetInterfaces().Select(m => m.GetTypeInfo()).FirstOrDefault(t => t.IsGenericType && !t.IsGenericTypeDefinition && t.IsInterface
                                            && t.GetGenericTypeDefinition() == typeof(ICollection<>));
            return resultTypeInfo == null ? null : resultTypeInfo.AsType();
        }

        static Array SplitItems(string text, Type itemType) {
            // TODO Support correct tokenization (probably similar to Properties.ParseKeyValuePairs())
            string[] items = text.Split(WS, StringSplitOptions.RemoveEmptyEntries);
            Array array = Array.CreateInstance(itemType, items.Length);

            int index = 0;
            foreach (string m in items) {
                var vs = ValueSerializer.GetValueSerializer(itemType);
                var item = vs.ConvertFromString(m, itemType, null);
                array.SetValue(item, index++);
            }
            return array;
        }

        public string ConvertToString(object value, IValueSerializerContext context) {
            throw new NotImplementedException();
        }

    }
}
