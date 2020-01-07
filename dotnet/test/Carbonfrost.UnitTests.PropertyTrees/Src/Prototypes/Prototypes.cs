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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Core.Runtime;

#pragma warning disable 3003
#pragma warning disable 3008

[assembly: Xmlns("https://ns.example.com", ClrNamespace = "Prototypes")]
namespace Prototypes {

    public class Alpha {

        public bool A { get; set; }
        public bool? AA { get; set; }
        public byte B { get; set; }
        public byte? BB { get; set; }
        public byte[] C { get; set; }
        public char D { get; set; }
        public char? DD { get; set; }
        public DateTime E { get; set; }
        public DateTime? EE { get; set; }
        public decimal F { get; set; }
        public decimal? FF { get; set; }
        public double G { get; set; }
        public double? GG { get; set; }
        public short H { get; set; }
        public short? HH { get; set; }
        public int I { get; set; }
        public int? II { get; set; }
        public long J { get; set; }
        public long? JJ { get; set; }
        public sbyte K { get; set; }
        public sbyte? KK { get; set; }
        public float L { get; set; }
        public float? LL { get; set; }
        public string M { get; set; }
        public ushort N { get; set; }
        public ushort? NN { get; set; }
        public uint O { get; set; }
        public uint? OO { get; set; }
        public ulong P { get; set; }
        public ulong? PP { get; set; }
        public Uri Q { get; set; }
        public TimeSpan R { get; set; }
        public TimeSpan? RR { get; set; }
        public Guid S { get; set; }
        public Guid? SS { get; set; }
        public DateTimeOffset T { get; set; }
        public DateTimeOffset? TT { get; set; }
        public Type U { get; set; }
        public DateTimeStyles V { get; set; }

    }

    public class AlphaChi : INotifyPropertyChanged, IUriContext {

        private bool a;
        private byte b;
        private DateTimeOffset t;
        private DateTimeOffset? tt;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public byte B {
            get { return b; }
            set {
                if (b != value) {
                    b = value;
                    OnPropertyChanged("B");
                }
            }
        }

        public bool A {
            get { return a; }
            set {
                if (a != value) {
                    a = value;
                    OnPropertyChanged("A");
                }
            }
        }

        public DateTimeOffset T {
            get { return t; }
            set {
                if (t != value) {
                    t = value;
                    OnPropertyChanged("T");
                }
            }
        }

        public DateTimeOffset? TT {
            get { return tt; }
            set {
                if (tt != value) {
                    tt = value;
                    OnPropertyChanged("TT");
                }
            }
        }

        public Uri BaseUri { get; set; }

    }

    // This list class contains Add(object), Add(Beta), and AddNew()
    public class BetaList : List<object>, IList<Beta> {

        [Add(Name = "beta")] // $NON-NLS-1
        public Beta AddNew(string name) {
            var target = new Beta { D = name };
            this.Add(target);

            return target;
        }

        bool ICollection<Beta>.IsReadOnly {
            get {
                return false;
            }
        }

        public int IndexOf(Beta item) {
            throw new NotImplementedException();
        }

        public void Insert(int index, Beta item) {
            throw new NotImplementedException();
        }

        public new Beta this[int index] {
            get {
                return (Beta) base[index];
            }
            set {
                base[index] = value;
            }
        }

        public void Add(Beta item) {
            base.Add(item);
        }

        public bool Contains(Beta item) {
            throw new NotImplementedException();
        }

        public void CopyTo(Beta[] array, int arrayIndex) {
            base.CopyTo(array, arrayIndex);
        }

        public bool Remove(Beta item) {
            return base.Remove(item);
        }

        public new IEnumerator<Beta> GetEnumerator() {
            throw new NotImplementedException();
        }
    }

    public class Beta {

        // TODO Should ALpha be implicitly created like in XRMS?
        private readonly IDictionary<string, Alpha> g = new Dictionary<string, Alpha>();
        private readonly Alpha h = new Alpha();
        private readonly Eta i = new Eta(0, TimeSpan.Zero, 0, 0);
        private readonly AlphaChi j = new AlphaChi();
        private readonly Omicron k = new Omicron();

        public Alpha A { get; set; }
        public Gamma B { get; set; }
        public Uri C { get; set; }
        public string D { get; set; }

        public Gamma E { get; set; }
        public Properties F { get; set; }
        public IDictionary<string, Alpha> G { get { return g; } }
        public Alpha H { get { return h; } }
        public Eta I { get { return i; } }
        public AlphaChi J { get { return j; } }
        public Omicron K { get { return k; } }

        // Should be treated as a property and not a streaming source
        public string Source { get; set; }

        public Beta() {
            this.A = new Alpha();
            this.B = new Gamma();
            this.E = new GammaExtension();
        }
    }

    public class Gamma {

        public CultureInfo A { get; set; }

        [FileLengthValue]
        public double B { get; set; }

        public Regex C { get; set; }

        public Beta Source { get; set; }
    }

    sealed class FileLengthValueAttribute : ValueSerializerAttribute, IValueSerializer {
        public FileLengthValueAttribute()
            : base(typeof(FileLengthValueAttribute)) {}

        public object ConvertFromString(string text, Type destinationType, IValueSerializerContext context) {
            if (text.EndsWith("MB")) {
                return 1000 * 1000 * double.Parse(text.Substring(0, text.Length - 2));
            }
            throw new NotImplementedException();
        }

        public string ConvertToString(object value, IValueSerializerContext context) {
            throw new NotImplementedException();
        }
    }

    public class GammaExtension : Gamma {

        // Shadowed property should bind; not
        public new DateTime B { get; set; }
    }


    public class Delta {

        private readonly IList<string> _d;

        public IList<Alpha> A { get; private set; }
        public BetaList B { get; private set; }
        public AlphaList C { get; private set; }
        public string Name { get; set; }
        public IList<string> D { get { return _d; } }

        public Delta() {
            this.A = new List<Alpha>();
            this.B = new BetaList();
            this.C = new AlphaList();
            _d = new List<string>();
        }

    }

    public class AlphaList : List<Alpha> {}

    public class Eta : IFileLocationConsumer {

        public int A { get; private set; }
        public TimeSpan B { get; private set; }
        public double C { get; private set; }
        public long D { get; private set; }
        public Uri E { get; set; }

        public Eta(int a, TimeSpan b, double c, long d) {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public int LineNumber { get; set; }
        public int LinePosition { get; set; }
        public bool SetFileLocationCalled { get; set; }

        void IFileLocationConsumer.SetFileLocation(int lineNumber, int linePosition) {
            SetFileLocationCalled = true;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

    }

    public class IotaChi {

        public StreamingSource A { get; set; }
        public ITemplate Template { get; set; }
        public IotaChi B { get; set; }
        [Add]
        public IotaChi Add(ITemplate template, StreamingSource a) {
            return B = new IotaChi { Template = template , A = a };
        }
    }

    public class Iota {

        public int A { get; private set; }
        public TimeSpan B { get; private set; }
        public double C { get; private set; }
        public long D { get; private set; }
        public Eta E { get; private set; }
        public string F { get; set; }
        public Uri G { get; set; }
        public NestedClass H { get; set; }

        public Iota(int a, TimeSpan b, double c, long d, Eta e) {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
            this.E = e;
        }

        public class NestedClass {
            public string A { get; set; }
        }
    }

    [Builder(typeof(EpsilonBuilder))]
    public abstract class Epsilon {

        public int A { get; private set; }
        public TimeSpan B { get; private set; }
        public double C { get; private set; }
        public long D { get; private set; }
        public Epsilon E { get; private set; }

        protected Epsilon(int a, TimeSpan b, double c, long d, Epsilon e) {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
            this.E = e;
        }

    }

    public class EpsilonAlpha : Epsilon {

        public EpsilonAlpha(int a, TimeSpan b, double c, long d, Epsilon e)
            : base(a, b, c, d, e) {}
    }

    [Builder(typeof(EpsilonExtendedBuilder))]
    public class EpsilonExtended : Epsilon {

        public DateTime F { get; private set; }

        public EpsilonExtended(int a, TimeSpan b, double c, long d, Epsilon e, DateTime f)
            : base(a, b, c, d, e)
        {
            this.F = f;
        }
    }

    public class EpsilonExtendedBuilder : EpsilonBuilder {

        public DateTime F { get; set; }

        public override Epsilon Build(IServiceProvider serviceProvider) {
            Epsilon e = null;
            if (E != null)
                e = E.Build(serviceProvider);

            return new EpsilonExtended(A, B, C, D, e, F);
        }
    }

    public class EpsilonBuilder : Builder<Epsilon> {

        public int A { get; set; }
        public TimeSpan B { get; set; }
        public double C { get; set; }
        public long D { get; set; }

        public EpsilonBuilder E { get; set; }

        public EpsilonBuilder() {}

        public override Epsilon Build(IServiceProvider serviceProvider) {
            return new EpsilonAlpha(A, B, C, D, E.Build(serviceProvider));
        }
    }

    public class Phi : IAddChild<Gamma> {

        public Gamma G { get; internal set; }

        public void AddChild(Gamma item) {
            this.G = item;
        }

        public void AddChild(object item) {}
        public void AddText(string text) {}
    }

    public class Psi {

        public IList<Alpha> A { get; private set; }
        public IList<string> B { get; private set; }

        public Psi() {
            this.A = new List<Alpha> {
                new Alpha { B = 47 }, new Alpha { B = 39 }, new Alpha { B = 47 }
            };
            this.B = new List<string>();
        }

    }
    public class Upsilon {

        public int A { get; set; }
        public string B { get; set; }
        public EpsilonChi C { get; set; }

        public Upsilon(string b, int a = 45) {
            this.A = a;
            this.B = b;
        }
    }

    public static class AddGammaExensions {

        [AddAttribute(Name = "gammaChild")]
        public static Gamma AddNewGamma(this IAddChild<Gamma> source, int a) {
            if (source == null)
                throw new ArgumentNullException("source");

            Gamma g = new Gamma();
            source.AddChild(g);
            return g;
        }

        // TODO Check for covariance on the interface  -- IADdChild<GammaSupertype>
    }

    public static class OmicronExtensions {


        [AddAttribute(Name = "gamma")]
        public static Gamma AddNewGamma(this Omicron omicron,
                                        CultureInfo a) {
            if (omicron == null)
                throw new ArgumentNullException("omicron");

            return omicron.G = new Gamma { A = a };
        }

        [AddAttribute(Name = "gammaOptional")]
        public static Gamma AddNewGamma(this Omicron omicron,
                                        CultureInfo a = null,
                                        double b = 6.7) {
            if (omicron == null)
                throw new ArgumentNullException("omicron");

            return omicron.G = new Gamma { A = a, B = b };
        }

        [AddAttribute(Name = "gammaNoargs")]
        public static Gamma AddNewGamma(this Omicron omicron) {
            if (omicron == null)
                throw new ArgumentNullException("omicron");

            return omicron.G = new Gamma { A = new CultureInfo("fr-FR") };
        }

        [AddAttribute(Name = "gammaLatebound")]
        public static Gamma AddNewGammaLateBound(this Omicron omicron) {
            if (omicron == null)
                throw new ArgumentNullException("omicron");

            return omicron.G = new GammaExtension { A = new CultureInfo("es-ES") };
        }

        [Add]
        public static Tau AddTau(this Omicron omicron) {
            if (omicron == null)
                throw new ArgumentNullException("omicron");

            return omicron.T = new Tau { };
        }

        [Add]
        public static Beta AddSource(this Omicron omicron) {
            if (omicron == null)
                throw new ArgumentNullException("omicron");

            return omicron.B = new Beta();
        }
    }

    public class OmicronTheta {
        public Alpha Alpha { get; set; }
        public Beta Beta { get; set; }
        public Delta Delta { get; set; }
    }

    public class OmicronThetaWithScope : INameScope {

        private readonly INameScope _scope = new NameScope();

        public Alpha Alpha { get; set; }
        public Beta Beta { get; set; }
        public Delta Delta { get; set; }

        public object FindName(string name) {
            return _scope.FindName(name);
        }

        public void RegisterName(string name, object element) {
            _scope.RegisterName(name, element);
        }

        public void UnregisterName(string name) {
            _scope.UnregisterName(name);
        }
    }

    public class Omicron {

        internal Alpha A { get; private set; }
        internal Beta B { get; set; }
        internal Gamma G { get; set; }
        internal Tau T { get; set; }

        // TODO What happens when there are multiple add methods
        // with the same name? (even if the name is implicit?)
        // What happens if there is a prop and add method with the
        // same name?

        [AddAttribute(Name = "alpha")]
        public Alpha AddNewAlpha() {
            return A = new Alpha();
        }

        [AddAttribute(Name = "beta")]
        public Beta AddNewBeta(Uri c) {
            return B = new Beta { C = c };
        }

        [AddAttribute(Name = "epsilon")]
        public static void AddNewEpsilon(Epsilon e) {
        }
    }

    // Demonstrates using dictionary access using indexer
    public class OmicronAlpha {

        private Alpha a = new Alpha();

        public Alpha A_ { get { return a; } }

        public bool B { get; set; }
        public int C_ { get; set; }

        public object this[string key] {
            get {
                switch (key) {
                    case "a":
                        return a;
                    default:
                        throw new KeyNotFoundException(key);
                }
            }
        }

        [AddAttribute(Name = "C")]
        public void Count() {
            this.C_++;
        }
    }

    // Demonstrates derived classes with ConcreteClassAttribute
    [ConcreteClass(typeof(MuAlpha))]
    public abstract class Mu {
    }

    public class MuAlpha : Mu {
        public int B { get; set; }
    }

    // Demonstrates composable providers
    public class PiChi {

        public Condition Preconditions { get; set; }
        public Control Controls { get; set; }
    }

    // Demonstrates derived classes with builders
    public abstract class EpsilonChi { // TODO IObjectWithType convention
        public int A { get; set; }

        public Type Type {
            get {
                return GetType();
            }
        }
    }

    public class EpsilonChiAlphaBuilder : Builder<EpsilonChiAlpha> {

        public char M { get; set; }

        public override EpsilonChiAlpha Build(IServiceProvider serviceProvider = null) {
            var result = base.Build(serviceProvider);
            result._M = M;

            return result;
        }
    }

    [Builder(typeof(EpsilonChiAlphaBuilder))]
    public class EpsilonChiAlpha : EpsilonChi {
        public char _M { get; set; }
    }

    // TODO Demonstrates the use of service activation using tau
    public class Tau {

    }

    // public class Bravo {

    //     readonly ComponentCollection components = new ComponentCollection();

    //     public ComponentCollection Components {
    //         get {
    //             return components;
    //         }
    //     }
    // }

    [Extender]
    public static class Charlie {

        [Extender]
        public static void SetValueExtender(Alpha alpha, int value) {
            alpha.I = value;
        }

        [Extender]
        public static int GetValueExtender(Alpha alpha) {
            return alpha.I;
        }
    }

    public class Echo {

        public NameValueCollection V { get; private set; }

        public Echo() {
            V = new NameValueCollection();
        }
    }

    public class Foxtrot : PropertiesObject {
        public AlphaChi A { get; private set; }
        public Uri BaseUri { get; set; }

        public Foxtrot() {
            A = new AlphaChi();
        }
    }
}
