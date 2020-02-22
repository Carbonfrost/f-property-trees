//
// Copyright 2010 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Runtime.Serialization;
using System.Xml;

namespace Carbonfrost.Commons.PropertyTrees {

    public class PropertyTreeException : Exception, IXmlLineInfo {

        public string SourceUri {
            get {
                return FileLocation.FileName;
            }
        }

        public PropertyTreeException() : this(null, -1, -1) {}

        public PropertyTreeException(string message) : this(message, -1, -1) {}

        public PropertyTreeException(string message, int lineNumber, int linePosition)
            : this(message, null, lineNumber, linePosition) {}

        public PropertyTreeException(string message, Exception innerException) : base(message, innerException) {}

        public PropertyTreeException(string message, Exception innerException, int lineNumber, int linePosition)
            : this(message, innerException, new FileLocation(lineNumber, linePosition, null)) {
        }

        internal PropertyTreeException(string message, Exception innerException, FileLocation fileLocation)
            : base(BuildMessage(message, fileLocation, innerException as PropertyTreeException), innerException) {
            this.FileLocation = fileLocation;
        }

        static string BuildMessage(string message, FileLocation location, PropertyTreeException inner) {
            // Don't present multiple line infos
            if (location.IsEmpty || (inner != null && inner.LineNumber > 0))
                return message;
            else
                return message + Environment.NewLine + Environment.NewLine + location.ToString("h");
        }

        protected PropertyTreeException(SerializationInfo info, StreamingContext context) : base(info, context) {
            this.FileLocation = (FileLocation) info.GetValue("fileLocation", typeof(FileLocation));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);

            info.AddValue("fileLocation", FileLocation);
        }

        public int LineNumber {
            get { return FileLocation.LineNumber; }
        }

        public int LinePosition {
            get { return FileLocation.LinePosition; }
        }

        internal FileLocation FileLocation {
            get;
            private set;
        }

        bool IXmlLineInfo.HasLineInfo() {
            return LineNumber > 0 && LinePosition > 0;
        }
    }

}
