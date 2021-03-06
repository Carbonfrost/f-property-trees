//
// Copyright 2014, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class PropertyTreeBinderErrors : IPropertyTreeBinderErrors {

        public static readonly IPropertyTreeBinderErrors Default = new PropertyTreeBinderErrors();

        public void BadAddChild(Type parentType, Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BadAddChild(parentType, ex, loc);
        }

        public void BinderConversionError(string name, Type componentType, Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BinderConversionError(name, componentType, ex, loc);
        }
        public void BindUnmatchedProperty(QualifiedName qualifiedName, Type componentType, FileLocation loc) {
            throw PropertyTreesFailure.UnmatchedMembersGenericError(qualifiedName, componentType, loc);
        }

        public void FailedToLoadFromSource(Uri uri, Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.FailedToLoadFromSource(uri, ex, loc);
        }

        public void IdCannotBeBlank(FileLocation loc) {
        }

        public void IdAlreadyRegistered(string id, FileLocation loc) {
        }

        public void CouldNotBindGenericParameters(Type componentType, Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.CouldNotBindGenericParameters(componentType, ex, loc);
        }

        public void CouldNotBindStreamingSource(Type componentType, FileLocation loc) {
            throw PropertyTreesFailure.CouldNotBindStreamingSource(componentType, loc);
        }

        public void DuplicatePropertyName(IEnumerable<QualifiedName> duplicates, FileLocation loc) {
            throw PropertyTreesFailure.DuplicatePropertyName(duplicates, loc);
        }

        public void RequiredPropertiesMissing(IEnumerable<string> requiredMissing, OperatorDefinition op, FileLocation loc) {
            throw PropertyTreesFailure.RequiredPropertiesMissing(requiredMissing, op.ToString(), loc.LineNumber, loc.LinePosition);
        }

        public void NoAddMethodSupported(Type type, FileLocation loc) {
            throw PropertyTreesFailure.NoAddMethodSupported(type, loc);
        }

        public void NoTargetProviderMatches(Type componentType, FileLocation loc) {
            throw PropertyTreesFailure.NoTargetProviderMatches(componentType, loc);
        }

        public void BadTargetTypeDirective(Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BadTargetTypeDirective(ex, loc);
        }

        public void BadTargetProviderDirective(Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BadTargetProviderDirective(ex, loc);
        }

        public void BadSourceDirective(Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BadSourceDirective(ex, loc);
        }
    }
}
