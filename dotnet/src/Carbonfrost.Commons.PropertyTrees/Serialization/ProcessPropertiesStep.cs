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
using System.Linq;
using System.Reflection;
using System.Xml;
using Carbonfrost.Commons.Core;

using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        class ProcessPropertiesStep : IApplyMemberStep {

            readonly bool _allowDefault;

            public ProcessPropertiesStep(bool allowDefault) {
                this._allowDefault = allowDefault;
            }

            bool IApplyMemberStep.Apply(PropertyTreeBinderImpl parent, PropertyTreeMetaObject target, PropertyTreeNavigator node) {
                PropertyDefinition prop = null;
                if (_allowDefault) {
                    var defaultProperties = target.GetDefinition().DefaultProperties;

                    if (defaultProperties.Skip(1).Any()) {
                        prop = defaultProperties.FirstOrDefault(p => p.TryGetValue(target.Component, node.QualifiedName));
                    }

                    if (prop == null) {
                        prop = defaultProperties.FirstOrDefault();
                    }

                    if (prop == null)
                        return false;

                } else {
                    var im = ImpliedName(node, target);
                    prop = target.SelectProperty(im);
                    if (prop == null || prop.IsIndexer)
                        return false;
                }

                parent.DoPropertyBind(target, node, prop);
                return true;
            }
        }

        private void DoPropertyBind(PropertyTreeMetaObject target,
                                    PropertyTreeNavigator navigator,
                                    PropertyDefinition property)
        {
            object ancestor = null;
            PropertyTreeMetaObject ancestorMeta = null;

            if (property.IsExtender) {
                var ancestorType = property.DeclaringTreeDefinition.SourceClrType;
                ancestorMeta = target.Ancestors.FirstOrDefault(
                    t => ancestorType.IsAssignableFrom(t.ComponentType));

                if (ancestorMeta != null)
                    ancestor = ancestorMeta.Component;
            }

            var component = target.Component;
            PropertyTreeMetaObject propertyTarget = target.CreateChild(property, navigator.QualifiedName, ancestorMeta);

            var services = new PropertyBindContext(
                component,
                property,
                ServiceProvider.Compose(ServiceProvider.FromValue(navigator), this))
            {
                LineNumber = navigator.LineNumber,
                LinePosition = navigator.LinePosition,
            };

            try {
                propertyTarget = Bind(propertyTarget, navigator, services);
                target.BindSetMember(property, navigator.QualifiedName, propertyTarget, ancestorMeta, services);

            } catch (NullReferenceException nre) {
                // Normally a "critical" exception, consider it a conversion error
                _errors.BinderConversionError(property.Name, target.ComponentType, nre, navigator.FileLocation);

            } catch (ArgumentException a) {
                _errors.BinderConversionError(property.Name, target.ComponentType, a, navigator.FileLocation);

            } catch (PropertyTreeException) {
                throw;
            } catch (Exception ex) {
                if (ex.IsCriticalException())
                    throw;
                _errors.BinderConversionError(property.Name, target.ComponentType, ex, navigator.FileLocation);
            }
        }

        class PropertyBindContext : IValueSerializerContext, IXmlLineInfo {

            private readonly object _component;
            private readonly PropertyDefinition _property;
            private readonly IServiceProvider _serviceProvider;

            public PropertyBindContext(object component, PropertyDefinition property, IServiceProvider serviceProvider) {
                _property = property;
                _component = component;
                _serviceProvider = serviceProvider;
            }

            public bool HasLineInfo() {
                return LineNumber > 0;
            }

            public int LineNumber { get; set; }
            public int LinePosition { get; set; }

            public object Instance {
                get {
                    return _component;
                }
            }

            public PropertyInfo Property {
                get {
                    return _property.GetUnderlyingDescriptor();
                }
            }

            public ParameterInfo Parameter {
                get {
                    return null;
                }
            }

            public object GetService(Type serviceType) {
                if (serviceType == null)
                    throw new ArgumentNullException("serviceType");

                if (typeof(IValueSerializerContext).Equals(serviceType)) {
                    return this;
                }

                if (typeof(IXmlLineInfo).Equals(serviceType)) {
                    return this;
                }

                return _serviceProvider.GetService(serviceType);
            }
        }

    }
}
