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
using System.Text;

namespace Carbonfrost.Commons.PropertyTrees {

    interface IFileLocationConsumer {
        void SetFileLocation(int lineNumber, int linePosition);
    }

    struct FileLocation : IEquatable<FileLocation> {

        private readonly int linePosition;
        private readonly int lineNumber;
        private readonly string fileName;

        public bool IsEmpty {
            get {
                return string.IsNullOrEmpty(FileName)
                    && LineNumber < 1 && LinePosition < 1;
            }
        }

        public string FileName {
            get { return fileName; }
        }

        public FileLocation(int lineNumber, int linePosition, string fileName) {
            this.linePosition = linePosition;
            this.lineNumber = lineNumber;
            this.fileName = fileName;
        }

        public int LinePosition {
            get { return linePosition; }
        }

        public int LineNumber {
            get { return lineNumber; }
        }

        // 'IEquatable<FileLocation> implementation.

        bool IEquatable<FileLocation>.Equals(FileLocation other) {
            return Equals(other);
        }

        // 'Object' overrides.
        public override int GetHashCode() {
            int hashCode = linePosition.GetHashCode() ^ lineNumber.GetHashCode();
            if (fileName != null) { hashCode ^= fileName.GetHashCode(); }

            return hashCode;
        }

        public override bool Equals(object obj) {
            if (obj is FileLocation) {
                FileLocation other = (FileLocation) obj;
                return (other.LinePosition == this.LinePosition)
                    && (other.LineNumber == this.LineNumber)
                    && (other.FileName == this.FileName);
            } else {
                return false;
            }
        }

        public override string ToString() {
            if (LinePosition > 0 && LineNumber > 0) {
                return string.Format("{0}:{1}:{2}", FileName, LineNumber, LinePosition);
            } else {
                return FileName ?? string.Empty;
            }
        }

        public string ToString(string format) {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider) {
            if (string.IsNullOrEmpty(format))
                return this.ToString();

            if (format.Length == 1)
                return this.ToStringFormat(format[0]);

            throw new FormatException();
        }

        private string ToStringFormat(char d) {
            switch (d) {
                case 'g':
                    return ToString();

                case 'h':
                    return ToLineString();

                default:
                    throw new FormatException();
            }
        }

        private string ToLineString() {
            if (LineNumber > 0 && LinePosition > 1) {
                return string.Format("line {0}, pos {1}", LineNumber, LinePosition);
            }
            if (LineNumber > 0) {
                return string.Format("line {0}", LineNumber);
            }
            if (LinePosition > 0) {
                return string.Format("pos {0}", LinePosition);
            }
            return string.Empty;
        }
    }
}
