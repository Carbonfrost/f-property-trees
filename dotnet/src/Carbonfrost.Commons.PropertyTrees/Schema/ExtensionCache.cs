//
// - ExtensionCache.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    static class ExtensionCache {

        private static IEnumerable<Assembly> extensionHelper;

        public static void Init() {
            EnsureCache();
        }

        private static void EnsureCache() {
            if (extensionHelper != null)
                return;

            extensionHelper = App.DescribeAssemblies(
                assembly => {
                    if (!assembly.IsExtension() || assembly.GetPropertyTreeOptions().SkipExtensionScanning)
                        return (IEnumerable<Assembly>) null;

                    foreach (TypeInfo type in assembly.GetTypesHelper()) {
                        if (!type.IsExtension())
                            continue;

                        foreach (MethodInfo mi in type.GetMethods()) {
                            if (mi.IsExtension()) {

                                foreach (RoleAttribute ra in mi.GetCustomAttributes(typeof(RoleAttribute))) {
                                    ra.ProcessExtensionMethod(mi);
                                }
                            }
                        }
                    }

                    return new [] { assembly };
                });
            extensionHelper.ToArray();
        }

    }
}
