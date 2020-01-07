//
// - ObjectCacheStreamingSource.cs -
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
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees {

    class ObjectCacheStreamingSource : StreamingSource {

        private readonly Uri _expectedUri;
        private readonly object _cache;

        public ObjectCacheStreamingSource(Uri _expectedUri, object cache) {
            this._expectedUri = _expectedUri;
            _cache = cache;
        }

        public override void Load(StreamContext inputSource, object instance) {
            if (IsCompatibleUri(inputSource.Uri)) {
                Template.Copy(_cache, instance);
                // TODO Could clobber?
                return;
            }
            throw new NotImplementedException();
        }

        public override object Load(StreamContext inputSource, Type instanceType) {
            if (IsCompatibleUri(inputSource.Uri)) {
                return _cache;
            }
            throw new NotImplementedException();
        }

        public override void Save(StreamContext outputTarget, object value) {
            throw new NotImplementedException();
        }

        private bool IsCompatibleUri(Uri uri) {
            return _expectedUri == uri;
        }
    }
}
