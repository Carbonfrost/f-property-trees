//
// - ValueSerializerContext.cs -
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
using System.Reflection;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

    public class ValueSerializerContext : IValueSerializerContext {

        public static readonly IValueSerializerContext Empty = new EmptyImpl();

        private readonly object _instance;
        private readonly IServiceProvider _serviceProvider;
        private readonly object _member;

        public ValueSerializerContext(object instance,
                                      IServiceProvider serviceProvider)
        {
            if (instance == null) {
                throw new ArgumentNullException("instance");
            }
            _instance = instance;
            _serviceProvider = serviceProvider ?? ServiceProvider.Null;
        }

        public ValueSerializerContext(object instance,
                                      ParameterInfo parameterInfo,
                                      IServiceProvider serviceProvider)
        {
            if (instance == null) {
                throw new ArgumentNullException("instance");
            }
            _instance = instance;
            _member = parameterInfo;
            _serviceProvider = serviceProvider ?? ServiceProvider.Null;
        }

        public ValueSerializerContext(object instance,
                                      PropertyInfo propertyInfo,
                                      IServiceProvider serviceProvider)
        {
            if (instance == null) {
                throw new ArgumentNullException("instance");
            }
            _instance = instance;
            _member = propertyInfo;
            _serviceProvider = serviceProvider ?? ServiceProvider.Null;
        }

        public virtual object GetService(Type serviceType) {
            if (serviceType == null) {
                throw new ArgumentNullException("serviceType");
            }
            if (serviceType.IsInstanceOfType(this)) {
                return this;
            }
            return null;
        }

        public object Instance {
            get {
                return _instance;
            }
        }

        public PropertyInfo Property {
            get {
                return _member as PropertyInfo;
            }
        }

        public ParameterInfo Parameter {
            get {
                return _member as ParameterInfo;
            }
        }

        class EmptyImpl : IValueSerializerContext {

            public object GetService(Type serviceType) {
                return null;
            }

            public object Instance {
                get {
                    return null;
                }
            }

            public PropertyInfo Property {
                get {
                    return null;
                }
            }

            public ParameterInfo Parameter {
                get {
                    return null;
                }
            }
        }
    }
}
