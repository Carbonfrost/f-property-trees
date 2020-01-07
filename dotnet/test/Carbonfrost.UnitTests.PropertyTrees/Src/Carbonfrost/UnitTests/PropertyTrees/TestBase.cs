//
// Copyright 2010, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.IO;
using System.Xml;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.PropertyTrees {

    public abstract class TestBase : TestClass {

        protected PropertyTree LoadTree(string fileName) {
            return PropertyTree.FromFile(GetContentPath(fileName));
        }

        protected PropertyTreeReader LoadContent(string fileName) {
            if (fileName.EndsWith(".xml", StringComparison.Ordinal))
                return PropertyTreeReader.CreateXml(GetContentPath(fileName));
            else
                throw new NotImplementedException();
        }

        protected string GetContentPath(string fileName) {
            return TestContext.GetFullPath(fileName);
        }

        protected Uri GetContentUri(string fileName) {
            return new Uri("file://" + GetContentPath(fileName));
        }

        protected XmlReader GetXmlReader(string fileName) {
            return XmlReader.Create(GetContentPath(fileName));
        }

        protected string GetContent(string fileName) {
            return TestContext.OpenText(GetContentPath(fileName)).ReadToEnd();
        }

    }
}
