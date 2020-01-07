// //
// // - FileLengthValueSerializer.cs -
// //
// // Copyright 2010 Carbonfrost Systems, Inc. (http://carbonfrost.com)
// //
// // Licensed under the Apache License, Version 2.0 (the "License");
// // you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at
// //
// //     http://www.apache.org/licenses/LICENSE-2.0
// //
// // Unless required by applicable law or agreed to in writing, software
// // distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and
// // limitations under the License.
// //
// using System;
// using Carbonfrost.Commons.Core;


// namespace Carbonfrost.Commons.PropertyTrees.ValueSerializers {

//     sealed class FileLengthValueSerializer : ValueSerializer {

//         public override string ConvertToString(object value, IValueSerializerContext context) {
//             if (value != null && IsValidType(value.GetType()))
//                 return value + "B";
//             else
//                 return base.ConvertToString(value, context);
//         }

//         public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
//             if (text != null) {
//                 long bytes = FileLength.ParseFileLength(text);

//                 if (destinationType == typeof(FileLength))
//                     return new FileLength(bytes);

//                 return bytes;
//             }

//             return base.ConvertFromString(text, destinationType, context);
//         }

//         static bool IsValidType(Type value) {
//             Type nt = Nullable.GetUnderlyingType(value) ?? value;

//             switch (Type.GetTypeCode(nt)) {
//                 case TypeCode.Int16:
//                 case TypeCode.Int32:
//                 case TypeCode.Int64:
//                 case TypeCode.Decimal:
//                 case TypeCode.Double:
//                 case TypeCode.Single:
//                 case TypeCode.UInt16:
//                 case TypeCode.UInt32:
//                 case TypeCode.UInt64:
//                     return true;
//                 default:
//                     return false;
//             }
//         }
//     }
// }
