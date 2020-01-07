//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.PropertyTrees {

    public sealed class AttachedPropertyID : IEquatable<AttachedPropertyID> {

        private readonly Type _declaringType;
        private readonly string _propertyName;

        public Type DeclaringType {
            get {
                return _declaringType;
            }
        }

        public string PropertyName {
            get {
                return _propertyName;
            }
        }

        public AttachedPropertyID(Type declaringType, string propertyName) {
            if (declaringType == null) {
                throw new ArgumentNullException("declaringType");
            }
            if (propertyName == null) {
                throw new ArgumentNullException("propertyName");
            }
            if (string.IsNullOrEmpty(propertyName)) {
                throw Failure.EmptyString("propertyName");
            }

            _declaringType = declaringType;
            _propertyName = propertyName;
        }

        public override bool Equals(object obj) {
            return Equals(obj as AttachedPropertyID);
        }

        public bool Equals(AttachedPropertyID other) {
            if (other == null) {
                return false;
            }

            return _declaringType == other._declaringType
                && _propertyName == other._propertyName;
        }

        public override int GetHashCode() {
            int hashCode = 0;
            unchecked {
                hashCode += 37 * _declaringType.GetHashCode();
                hashCode += 7 * _propertyName.GetHashCode();
            }
            return hashCode;
        }

        public static bool operator ==(AttachedPropertyID left, AttachedPropertyID right) {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }

        public static bool operator !=(AttachedPropertyID left, AttachedPropertyID right) {
            return !(left == right);
        }

        public override string ToString() {
            return string.Concat(_declaringType, ".", _propertyName);
        }
    }

}
