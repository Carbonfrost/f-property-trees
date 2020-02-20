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

    public class PropertyTreeWriterSettings {

        private bool _includeMetadata;
        private bool _isReadOnly;

        public bool IncludeMetadata {
            get {
                return _includeMetadata;
            }
            set {
                ThrowIfReadOnly();
                _includeMetadata = value;
            }
        }

        public bool IsReadOnly {
            get {
                return _isReadOnly;
            }
        }

        public PropertyTreeWriterSettings() {
        }

        public PropertyTreeWriterSettings(PropertyTreeWriterSettings other) {
            if (other != null) {
                IncludeMetadata = other.IncludeMetadata;
            }
        }

        public PropertyTreeWriterSettings Clone() {
            return CloneCore();
        }

        protected virtual PropertyTreeWriterSettings CloneCore() {
            return new PropertyTreeWriterSettings(this);
        }

        public void MakeReadOnly() {
            _isReadOnly = true;
        }

        protected void ThrowIfReadOnly() {
            if (IsReadOnly) {
                throw Failure.ReadOnlyCollection();
            }
        }
    }
}
