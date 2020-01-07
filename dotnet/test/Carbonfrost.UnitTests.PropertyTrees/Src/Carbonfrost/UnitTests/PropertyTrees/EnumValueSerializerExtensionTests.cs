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
using System.Net.Sockets;
using Carbonfrost.Commons.PropertyTrees.ValueSerializers;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public class EnumValueSerializerExtensionTests {

        readonly IValueSerializer conv = EnumValueSerializerExtension.Instance;

        [Fact]
        public void test_convert_builtins() {
            Assert.Equal(SocketFlags.Broadcast | SocketFlags.ControlDataTruncated | SocketFlags.Multicast,
                         conv.ConvertFromString("Broadcast,ControlDataTruncated,Multicast", typeof(SocketFlags), null));
        }

        [Fact]
        public void test_convert_whitespace_delimited() {
            Assert.Equal(SocketFlags.Broadcast | SocketFlags.ControlDataTruncated | SocketFlags.Multicast,
                         conv.ConvertFromString("Broadcast , \t \t \n ControlDataTruncated Multicast", typeof(SocketFlags), null));
        }

        [Fact]
        public void ConvertFromString_should_inflect_hyphenated_names() {
            Assert.Equal(SocketFlags.Broadcast | SocketFlags.ControlDataTruncated,
                         conv.ConvertFromString("broadcast control-data-truncated", typeof(SocketFlags), null));
            Assert.Equal(SocketFlags.ControlDataTruncated,
                         conv.ConvertFromString("control-data-truncated", typeof(SocketFlags), null));
        }
    }
}
