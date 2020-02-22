//
// Copyright 2014, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
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

using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl : PropertyTreeMetaObjectBinder, IServiceProvider {

        private readonly IPropertyTreeBinderErrors _errors;
        private readonly INameScope _globalNameScope = new NameScope();
        private readonly PropertyTreeBinderOptions _options;

        private readonly PropertyTreeBinderStep[] Pipeline = {
            new ApplyGenericParametersStep(),
            new ApplyLateBoundTypeStep(),
            new ApplyProviderTypeStep(),
            new ApplyConstructorStep(),
            new ApplyUriContextStep(),
            new ApplyFileLocationStep(),
            new ApplyStreamingSourcesStep(),
            new ApplyDefaultConstructorStep(),
            new ApplyIdStep(),
            new ProcessMembersStep
            (
                new ProcessPropertiesStep(false),
                new ProcessOperatorsStep(),
                new ProcessPropertiesStep(true)
            ),
            new ErrorUnmatchedMembersStep(),
            new EndObjectStep(),
        };

        private readonly SerializerDirectiveFactory directiveFactory;

        public ExpressionContext CurrentExpressionContext {
            get {
                return _options.ExpressionContext;
            }
        }

        public SerializerDirectiveFactory DirectiveFactory {
            get {
                return directiveFactory;
            }
        }

        public override StreamingSource CreateStreamingSource(PropertyTreeMetaObject target, StreamContext input, IServiceProvider serviceProvider) {
            Type componentType = target.ComponentType;
            var uri = serviceProvider.GetRequiredService<IUriContext>();

            if (input.Uri == uri.BaseUri) {
                string name = input.Uri.Fragment.TrimStart('#');
                var obj = FindNameScope(target).FindName(name);
                if (obj == null) {
                    throw new NotImplementedException();
                }
                return new ObjectCacheStreamingSource(input.Uri, obj);
            }

            // TODO Should cache streaming sources so that the output objects can immediately be retrieved
            return StreamingSource.Create(componentType, input.ContentType) ?? new PropertyTreeSource();
        }

        private static QualifiedName ImpliedName(PropertyTreeNavigator nav, PropertyTreeMetaObject target) {
            QualifiedName qualifiedName = nav.QualifiedName;
            if (nav.IsExpressNamespace)
                return qualifiedName;

            NamespaceUri impliedNS = Utility.GetXmlnsNamespaceSafe(target.ComponentType);
            if (impliedNS == null)
                return qualifiedName;
            else
                return qualifiedName.ChangeNamespace(impliedNS);
        }

        private INameScope FindNameScope(PropertyTreeMetaObject target) {
            return (target.AncestorComponents.OfType<INameScope>().FirstOrDefault()) ?? _globalNameScope;
        }

        internal IServiceProvider GetBasicServices(PropertyTreeNavigator nav) {
            return ServiceProvider.Compose(this,
                                           ServiceProvider.FromValue(nav));
        }

        internal static FileLocation FindFileLocation(IServiceProvider serviceProvider) {
            var loc = serviceProvider.GetServiceOrDefault(Utility.NullLineInfo);
            string uri = Convert.ToString(serviceProvider.GetServiceOrDefault(Utility.NullUriContext).BaseUri);
            return new FileLocation(loc.LineNumber, loc.LinePosition, uri);
        }

        public PropertyTreeBinderImpl(IPropertyTreeBinderErrors errors,
                                      PropertyTreeBinderOptions options) {
            _errors = errors;
            this.directiveFactory = new SerializerDirectiveFactory(this);
            _options = options;
        }

        internal static Predicate<PropertyTreeNavigator> ImplicitDirective(PropertyTreeMetaObject target, string name) {
            var defaultNS = NamespaceUri.Default + name;
            var langNS = Xmlns.PropertyTrees2010Uri + name;
            var existing = (PropertyNodeDefinition) target.SelectProperty(defaultNS, allowDefaultProperties: false)
                ?? target.SelectOperator(defaultNS);

            if (existing == null) {
                return t => t.Name == name;

            } else {
                return t => t.QualifiedName == langNS;
            }
        }

        private static IEnumerable<PropertyTreeNavigator> SelectChildren(PropertyTreeNavigator navigator) {
            navigator = navigator.Clone();

            if (navigator.MoveToFirstChild()) {
                do {
                    yield return navigator.Clone();
                } while (navigator.MoveToNext());
            }
        }

        internal PropertyTreeMetaObject BindChildNodes(PropertyTreeMetaObject target,
                                                       PropertyTreeNavigator self,
                                                       NodeList children) {
            foreach (var step in Pipeline) {
                target = step.Process(this, target, self, children);
            }

            return target;
        }

        public override PropertyTreeMetaObject Bind(PropertyTreeMetaObject target,
                                                    PropertyTreeNavigator navigator,
                                                    IServiceProvider serviceProvider) {
            if (target == null)
                throw new ArgumentNullException("target");
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            if (navigator.IsProperty) {
                var textValue = navigator.Value as string;
                if (textValue != null) {
                    return BindInitializeValue(
                        target,
                        navigator,
                        serviceProvider,
                        () => target.BindInitializeValue(textValue, serviceProvider));

                }

                var exprValue = navigator.Value as Expression;
                if (exprValue != null) {
                    var exprContext = ExpressionContext.Compose(
                        CurrentExpressionContext,
                        ExpressionContext.FromNameScope(FindNameScope(target))
                    );
                    exprValue = ExpressionUtility.LiftToCall(exprValue, exprContext);
                    return BindInitializeValue(
                        target,
                        navigator,
                        serviceProvider,
                        () => target.BindInitializer(exprValue, exprContext, serviceProvider));
                }

                throw new NotImplementedException();
            }

            var children = SelectChildren(navigator);
            return BindChildNodes(target, navigator, NodeList.Create(children));
        }

        private PropertyTreeMetaObject BindInitializeValue(PropertyTreeMetaObject target,
                                                           PropertyTreeNavigator navigator,
                                                           IServiceProvider serviceProvider,
                                                           Func<PropertyTreeMetaObject> thunk) {
            try {
                return thunk();

            } catch (PropertyTreeException) {
                throw;

            } catch (Exception ex) {
                var sp = ServiceProvider.Compose(ServiceProvider.FromValue(navigator), serviceProvider, this);
                // Throw critical exceptions if they originate within PT; otherwise, allow
                // callback to decide how to handle them.
                if (ex.IsCriticalException())
                    throw;

                var descriptor = serviceProvider.GetServiceOrDefault<IValueSerializerContext>();
                string property = navigator.QualifiedName.ToString();
                Type componentType;

                if (descriptor == null || descriptor.Instance == null) {
                    componentType = target.ComponentType;
                } else if (descriptor.Property == null) {
                    componentType = descriptor.Instance.GetType();
                } else {
                    property = descriptor.Property.Name;
                    componentType = descriptor.Property.DeclaringType;
                }

                FileLocation loc = navigator.FileLocation;
                object value = navigator.Value;

                _errors.BinderConversionError(property, componentType, ex, loc);
            }

            return target;
        }

        public object GetService(Type serviceType) {
            if (serviceType == typeof(IPropertyTreeBinderErrors)) {
                return _errors;
            }
            if (serviceType == typeof(PropertyTreeMetaObjectBinder)) {
                return this;
            }
            return null;
        }
    }
}
