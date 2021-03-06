//
// Copyright 2010, 2016, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees {

    internal static class Mixins {

        public static void All<T>(this IEnumerable<T> items) {
            var disp = items.GetEnumerator();
            while (disp.MoveNext()) {}
        }

        public static T SingleOrThrow<T>(this IEnumerable<T> items, Func<Exception> error) {
            bool flag = false;

            T result = default(T);
            foreach (var e in items) {
                if (flag) {
                    throw error();
                }

                flag = true;
                result = e;
            }

            return result;
        }

        public static bool IsHiddenUX(this Type type) {
            // Prevent confusion in the error message by concealing internal type
            // names from error messages

            if (type.Namespace == "Carbonfrost.Commons.PropertyTrees.Serialization") {
                return true;
            }
            return false;
        }

        public static bool IsCriticalException(this Exception ex) {
            // NRE, access violation, etc. that occur in this assembly are critical
            var thisFrame = new StackTrace(ex, false).GetFrames()[0];
            if (thisFrame.GetMethod().DeclaringType.GetTypeInfo().Assembly == typeof(Mixins).GetTypeInfo().Assembly) {
                return Failure.IsCriticalException(ex);
            }

            // State-corrupting and serious exceptions in user code (but not
            // common debuggable errors like NRE)
            return ex is OutOfMemoryException
                || ex is StackOverflowException
                || ex is SEHException
                || ex is SecurityException;
        }

        public static TValue GetValueOrCache<TKey, TValue>(
            this IDictionary<TKey, TValue> source, TKey key,
            Func<TKey, TValue> factory = null)
        {
            TValue result;
            if (!source.TryGetValue(key, out result)) {
                if (factory == null) {
                    result = default(TValue);
                }
                else {
                    result = factory(key);
                }

                source[key] = result;
            }

            return result;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> source, TKey key,
            TValue defaultValue = default(TValue))
        {
            TValue result = defaultValue;
            if (source.TryGetValue(key, out result)) {
                return result;
            }

            return defaultValue;
        }

        public static bool IsExtension(this ICustomAttributeProvider any) {
            return any.IsDefined(typeof(ExtensionAttribute), false)
                || any.IsDefined(typeof(ExtenderAttribute), false);
        }

        public static IEnumerable<T> ByLocalName<T>(this IEnumerable<T> items, string name, bool attached = false)
            where T : PropertyNodeDefinition
        {
            if (attached) {
                return items.Where(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(t.ShortName, name, StringComparison.OrdinalIgnoreCase));
            } else {
                return items.Where(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
            }
        }

        public static IEnumerable<TypeInfo> GetTypesHelper(this Assembly a) {
            try {
                return a.DefinedTypes;
            }
            catch (ReflectionTypeLoadException e) {
                return e.Types.Where(t => t != null).Select(t => t.GetTypeInfo());
            }
        }

    }
}
