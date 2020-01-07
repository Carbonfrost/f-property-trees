//
// - TargetSourceDirective.cs -
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
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class TargetSourceDirective {

        private readonly Uri source;

        public Uri Uri {
            get {
                return source;
            }
        }

        // TODO Consider criteria

        public ContentType Type { get; set; }
        public string Provider { get; set; }

        public StreamContext GetStreamingContext() {
            return StreamContext.FromSource(source);
        }

        public TargetSourceDirective(Uri source) {
            this.source = source;
        }

    }

}


