// //
// // Copyright 2010, 2015 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

//     sealed class BitRateValueSerializer : ValueSerializer {

//         public override string ConvertToString(object value, IValueSerializerContext context) {
//             if (Utility.IsFloatingNumeric(value))
//                 return value + "bits/s";

//             return base.ConvertToString(value, context);
//         }

//         public override object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
//             if (text != null) {
//                 double result;
//                 if (Utility.ConvertUnits(text, Shared.BitRate.UNITS, Shared.BitRate.CONVERSIONS, out result))
//                     return result;
//             }

//             return base.ConvertFromString(text, destinationType, context);
//         }
//     }
// }
