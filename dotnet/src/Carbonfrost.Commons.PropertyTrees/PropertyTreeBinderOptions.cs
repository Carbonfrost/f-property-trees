//
// Copyright 2015, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime.Expressions;

namespace Carbonfrost.Commons.PropertyTrees {

    public class PropertyTreeBinderOptions {

        private ExpressionContext _expressionContext;

        public ExpressionContext ExpressionContext {
            get {
                if (_expressionContext == null) {
                    _expressionContext = new ExpressionContext();
                }
                return _expressionContext;
            }
            set {
                ThrowIfReadOnly();
                _expressionContext = value;
            }
        }

        public PropertyTreeBinderOptions(PropertyTreeBinderOptions other) {
            if (other == null) {
                return;
            }
            _expressionContext = other.ExpressionContext;
        }

        public PropertyTreeBinderOptions() : this(null) {
        }

        public PropertyTreeBinderOptions Clone() {
            return new PropertyTreeBinderOptions(this);
        }

        public static PropertyTreeBinderOptions ReadOnly(PropertyTreeBinderOptions source) {
            var result = source.Clone();
            result.IsReadOnly = true;
            return result;
        }

        public bool IsReadOnly {
            get;
            private set;
        }

        private void ThrowIfReadOnly() {
            if (IsReadOnly) {
                throw Failure.Sealed();
            }
        }
    }
}


