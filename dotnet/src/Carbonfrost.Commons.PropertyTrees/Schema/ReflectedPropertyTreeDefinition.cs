//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;


using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    sealed class ReflectedPropertyTreeDefinition : PropertyTreeDefinition {

        private readonly Type type;
        private readonly OperatorDefinitionCollection operators;
        private readonly List<PropertyDefinition> _defaultProperties = new List<PropertyDefinition>();
        private OperatorDefinition activationConstructor;
        private readonly PropertyDefinitionCollection properties;
        private bool scanAdd;
        private bool _scanProperties;

        public override PropertyTreeSchema Schema {
            get {
                return PropertyTreeSchema.FromAssembly(type.GetTypeInfo().Assembly);
            }
        }

        public override Type SourceClrType {
            get { return this.type; }
        }

        public override PropertyDefinitionCollection Properties {
            get {
                EnsureProperties();
                return properties;
            }
        }

        public override IEnumerable<PropertyDefinition> DefaultProperties {
            get {
                EnsureProperties();
                return _defaultProperties;
            }
        }

        public override OperatorDefinition Constructor {
            get {
                EnsureFactoryDefinitions();
                return activationConstructor;
            }
        }

        public ReflectedPropertyTreeDefinition(Type type) {
            this.type = type;
            this.operators = new OperatorDefinitionCollection();
            this.properties = new PropertyDefinitionCollection();
            ApplyFilters(this);
        }

        public override OperatorDefinitionCollection Operators {
            get {
                EnsureFactoryDefinitions();
                return this.operators;
            }
        }

        public override string Namespace {
            get {
                var nn = Utility.GetXmlnsNamespaceSafe(type);
                return nn == null ? string.Empty : nn.NamespaceName;
            }
        }

        public override string Name {
            get {
                return type.Name.Replace('`', '-');
            }
        }

        public override PropertyDefinition GetProperty(QualifiedName name, GetPropertyOptions options) {
            PropertyDefinition result = this.Properties[name];
            if (result != null)
                return result;

            bool declaredOnly = options.HasFlag(GetPropertyOptions.DeclaredOnly);
            if (declaredOnly)
                return null;

            // TODO This seems backwards
            var comparer = options.HasFlag(GetPropertyOptions.IncludeExtenders)
                ? Utility.OrdinalIgnoreCaseQualifiedName
                : Utility.AttachedOrdinalIgnoreCaseQualifiedName;
            return EnumerateProperties().FirstOrDefault(t => comparer.Equals(t.QualifiedName, name));
        }

        public override OperatorDefinition GetOperator(QualifiedName name, bool declaredOnly = false) {
            if (name == null)
                throw new ArgumentNullException("name");

            OperatorDefinition result = this.Operators[name];
            if (result != null)
                return result;

            if (declaredOnly)
                return null;

            return EnumerateOperators().FirstOrDefault(t => Utility.OrdinalIgnoreCaseQualifiedName.Equals(t.QualifiedName, name));
        }

        private static void ApplyFilters(ReflectedPropertyTreeDefinition def) {
            // TODO This should be structured instead
            if (typeof(NameValueCollection) == def.SourceClrType) {
                // Make sure that Add(string,string) is the addon, and not Add(NameValueCollection) which isn't useful
                var mi = typeof(NameValueCollection).GetMethod("Add", new[] { typeof(string), typeof(string) });
                def.Operators.RemoveInternal(NamespaceUri.Default + "Add");
                def.Operators.Add(ReflectedPropertyTreeFactoryDefinition.Create(null, mi));
            }
        }

        private void FindAllOperators() {
            HashSet<MethodInfo> explicitOperators = new HashSet<MethodInfo>();

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
                bool match = false;
                // TODO Check signature of these methods because they could be illegal
                foreach (RoleAttribute attribute in FindRoleAttributes(method)) {
                    var result = attribute.BuildInstance(method);
                    if (result != null) {
                        this.operators.Add(result);
                        match = true;
                    }
                }

                if (match)
                    explicitOperators.Add(method);
            }

            foreach (var mi in type.GetMethods().Except(explicitOperators)) {
                if (!mi.IsPublic || mi.IsStatic)
                    continue;
                ParameterInfo[] parameters = mi.GetParameters();

                switch (mi.Name) {
                    case "Add":
                        if (IsValidAddon(mi, parameters)) {
                            // TODO We're consuming duplicated names (here, below); they should be errors
                            this.operators.TryAdd(ReflectedPropertyTreeFactoryDefinition.FromListAddMethod(mi));
                            var natural = ReflectedPropertyTreeFactoryDefinition.FromListAddMethod(mi, true);
                            if (natural != null && !this.operators.ContainsKey(natural.QualifiedName))
                                this.operators.Add(natural);
                        }
                        break;

                    case "Clear":
                        this.operators.Add(new ReflectedClearDefinition(null, mi));
                        break;

                    case "RemoveAt":
                        this.operators.TryAdd(new ReflectedRemoveDefinition(null, mi));
                        break;

                    case "Remove":
                        break;
                }
            }
        }

        static bool IsValidAddon(MethodInfo mi, ParameterInfo[] parameters)
        {
            if (parameters.Length == 1) {
                var param = parameters[0];

                return mi.ReturnType == typeof(void)
                    && !mi.IsDefined(typeof(AddAttribute), false)
                    && !param.IsOut
                    && !param.ParameterType.IsByRef;
            }

            return false;
        }

        static bool IsValidIndexer(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Property)
                return false;

            PropertyInfo pi = (PropertyInfo) member;
            MethodInfo mi = pi.GetGetMethod();
            if (mi == null)
                return false;

            ParameterInfo[] parameters = mi.GetParameters();
            if (parameters.Length == 1) {
                var param = parameters[0];

                return (param.ParameterType == typeof(string) || param.ParameterType == typeof(QualifiedName));
            }

            return false;
        }

        IEnumerable<RoleAttribute> FindRoleAttributes(MethodInfo method) {
            var result = method.GetCustomAttributes(typeof(RoleAttribute)).Cast<RoleAttribute>();

            foreach (var data in method.CustomAttributes) {
                if (data.AttributeType.GetTypeInfo().Assembly.Equals(typeof(ReflectedPropertyTreeDefinition).GetTypeInfo().Assembly)) {
                    continue;
                }

                if (data.AttributeType.FullName == typeof(AddAttribute).FullName) {
                    return new [] { AddAttribute.Default };
                }
            }
            return result;
        }

        internal void AddFactoryDefinition(ReflectedPropertyTreeFactoryDefinition definition) {
            this.operators.TryAdd(definition);
        }

        internal void AddPropertyDefinition(PropertyDefinition definition) {
            this.properties.TryAdd(definition);
        }

        void EnsureProperties() {
            if (_scanProperties) {
                return;
            }
            _scanProperties = true;
            var items = ReflectGetProperties(SourceClrType)
                .Select(t => new ReflectedPropertyDefinition(t));
            foreach (var item in items) {
                this.properties.Add(item);
            }

            if (typeof(IPropertiesContainer).GetTypeInfo().IsAssignableFrom(type)) {
                var one = new IndexerUsingIPropertiesPropertyDefinition(true);
                Properties.AddInternal(one);
                _defaultProperties.Add(one);
            }
            else if (typeof(IPropertyProvider).GetTypeInfo().IsAssignableFrom(type)) {
                // Try using IPropertyProvider
                var one = new IndexerUsingIPropertiesPropertyDefinition(typeof(IProperties).IsAssignableFrom(type));
                Properties.AddInternal(one);
                _defaultProperties.Add(one);
            }

            var defaultMember = (PropertyInfo) this.type.GetTypeInfo().GetDefaultMembers().FirstOrDefault(IsValidIndexer);
            if (defaultMember != null) {
                var one = new ReflectedIndexerPropertyDefinition(defaultMember);
                Properties.AddInternal(one);
                _defaultProperties.Add(one);
            }

            var methods = type.GetTypeInfo().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            FindExtensionProperties(methods);

            // Could define extender properties
            ExtensionCache.Init();
            this.properties.MakeReadOnly();
        }


        private static IEnumerable<PropertyInfo> ReflectGetProperties(Type sourceClrType) {
            var baseType = sourceClrType;
            var names = new HashSet<string>();
            do {
                foreach (var prop in baseType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)) {
                    if (names.Add(prop.Name) && prop.GetIndexParameters().Length == 0) {
                        yield return prop;
                    }
                }
                baseType = baseType.GetTypeInfo().BaseType;
            } while (baseType != null && baseType != typeof(object));
        }

        void FindExtensionProperties(IEnumerable<MethodInfo> methods) {
            var results = new Dictionary<AttachedPropertyID, ReflectedExtenderPropertyDefinition>();

            foreach (var method in methods) {
                foreach (var epa in method.GetCustomAttributes(typeof(ExtenderAttribute)).Cast<ExtenderAttribute>()) {
                    var pid = epa.GetAttachedPropertyID(method);
                    var current = results.GetValueOrCache(
                        pid, _ => new ReflectedExtenderPropertyDefinition(pid));

                    current.AddMethod(method);
                }
            }

            foreach (var item in results.Values) {
                this.properties.AddInternal(item);
            }
        }

        private void EnsureFactoryDefinitions() {
            if (!this.scanAdd) {
                this.scanAdd = true;
                FindAllOperators();
                FindProviderAddChildOperators();
                FindActivationConstructor();
            }

            ExtensionCache.Init();
        }

        void FindActivationConstructor() {
            var tt = type.GetTypeInfo();
            if (tt.IsAbstract) {

                // Composable providers can be abstract
                if (type.IsProviderType() && tt.IsDefined(typeof(ComposableAttribute), false)) {

                    var composeMember = App.GetProviderMember(type, "compose");
                    if (composeMember == null)
                        throw new NotImplementedException();

                    activationConstructor = ReflectedProviderFactoryDefinitionBase.Create(type, composeMember);
                }

            } else {
                MethodBase ctor = TypeHelper.FindActivationConstructor(type);
                if (ctor != null) {
                    activationConstructor = ReflectedPropertyTreeFactoryDefinition.Create(null, ctor);
                }
            }
        }

        private void FindProviderAddChildOperators() {
            Type arg;
            var tt = type.GetTypeInfo();

            if (tt.IsGenericType
                && !tt.IsGenericTypeDefinition
                && tt.GetGenericTypeDefinition() == typeof(IAddChild<>)
                && (arg = tt.GetGenericArguments()[0]).IsProviderType()) {

                foreach (var pi in App.DescribeProviders().GetProviderInfos(arg)) {
                    var m = pi.Member;
                    var name = pi.Name;

                    var item = ReflectedProviderFactoryDefinitionBase.Create(arg, m, name);
                    if (item == null || this.operators.ContainsKey(item.QualifiedName)) {
                        // TODO If two operators are defined with the same name, trace an error (StatusAppender.ForType(PropertyTreeSchema))
                        continue;
                    }
                    this.operators.Add(item);
                }
            }
        }

        public override IEnumerable<PropertyDefinition> EnumerateProperties(bool declaredOnly = false) {
            IEnumerable<PropertyDefinition> result = this.Properties;
            foreach (var t in BaseTypes) {
                var ops = t.Properties;
                result = result.Concat(ops);
            }

            return result;
        }

        public override IEnumerable<OperatorDefinition> EnumerateOperators(bool declaredOnly = false) {
            IEnumerable<OperatorDefinition> result = this.Operators;
            foreach (var t in BaseTypes) {
                var ops = t.Operators;
                result = result.Concat(ops);
            }

            return result;
        }

        public override PropertyDefinition GetProperty(string name, GetPropertyOptions options) {
            // TODO Probably treat as ambiguous
            bool declaredOnly = options.HasFlag(GetPropertyOptions.DeclaredOnly);
            bool attached = options.HasFlag(GetPropertyOptions.IncludeExtenders);
            var props = declaredOnly ? this.Properties : EnumerateProperties();
            return props.ByLocalName(name, attached).FirstOrDefault();
        }

        public override OperatorDefinition GetOperator(string name, bool declaredOnly) {
            var props = declaredOnly ? this.Operators : EnumerateOperators();
            return props.ByLocalName(name).FirstOrDefault();
        }
    }
}
