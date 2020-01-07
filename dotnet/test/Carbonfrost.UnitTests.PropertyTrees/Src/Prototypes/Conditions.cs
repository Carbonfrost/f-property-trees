//
// Copyright 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.PropertyTrees;

[assembly: Provides(typeof(Prototypes.Condition))]

namespace Prototypes {

    // A kind of composable provider
    [Composable, Providers]
    public abstract class Condition {

        [Provider(typeof(Condition), Name = "compose")]
        public static Condition Compose(params Condition[] items) {
            return new CompositeCondition(items);
        }

        public static Condition Parse(string text) {
            return (Condition) Composable.Parse(typeof(Condition), text);
        }
    }

    [Provider(typeof(Condition), Name = "platform")]
    public class PlatformCondition : Condition {
        public override string ToString() {
            return "platform()";
        }
    }

    [Provider(typeof(Condition), Name = "environment")]
    public class EnvironmentCondition : Condition {

        public string Variable { get; private set; }
        public string Pattern { get; private set; }

        public EnvironmentCondition(string variable,
                                    string pattern) {
            Variable = variable;
            Pattern = pattern;
        }

        public override string ToString() {
            return string.Format("environment(\"{0}\", \"{1}\")", Variable, Pattern);
        }
    }

    public class CompositeCondition : Condition {
        private readonly Condition[] _items;

        public CompositeCondition(Condition[] items) {
            _items = items;
        }

        public override string ToString() {
            return "compose(" + string.Join<Condition>(", ", _items) + ")";
        }
    }
}
