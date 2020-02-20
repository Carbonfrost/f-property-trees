//
// Copyright 2010, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

namespace Carbonfrost.Commons.PropertyTrees {

    public class PropertyTreeReaderSettings {
        private bool _allowExternals;
        private bool _isReadOnly;

        public bool AllowExternals {
            get {
                return _allowExternals;
            }
            set {
                ThrowIfReadOnly();
                _allowExternals = value;
            }
        }

        public bool IsReadOnly {
            get {
                return _isReadOnly;
            }
        }

        public PropertyTreeReaderSettings() { }

        public PropertyTreeReaderSettings(PropertyTreeReaderSettings other) {
            if (other != null) {
                AllowExternals = other.AllowExternals;
            }
        }

        public void MakeReadOnly() {
            _isReadOnly = true;
        }

        public PropertyTreeReaderSettings Clone() {
            return CloneCore();
        }

        protected virtual PropertyTreeReaderSettings CloneCore() {
            return new PropertyTreeReaderSettings(this);
        }

        protected void ThrowIfReadOnly() {
            if (IsReadOnly) {
                throw Failure.ReadOnlyCollection();
            }
        }

    }
}

