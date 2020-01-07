
// This file was automatically generated.  DO NOT EDIT or else
// your changes could be lost!

#pragma warning disable 1570

using System;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Carbonfrost.Commons.PropertyTrees.Resources {

    /// <summary>
    /// Contains strongly-typed string resources.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("srgen", "1.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    internal static partial class SR {

        private static global::System.Resources.ResourceManager _resources;
        private static global::System.Globalization.CultureInfo _currentCulture;
        private static global::System.Func<string, string> _resourceFinder;

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(_resources, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Carbonfrost.Commons.PropertyTrees.Automation.SR", typeof(SR).GetTypeInfo().Assembly);
                    _resources = temp;
                }
                return _resources;
            }
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return _currentCulture;
            }
            set {
                _currentCulture = value;
            }
        }

        private static global::System.Func<string, string> ResourceFinder {
            get {
                if (object.ReferenceEquals(_resourceFinder, null)) {
                    try {
                        global::System.Resources.ResourceManager rm = ResourceManager;
                        _resourceFinder = delegate (string s) {
                            return rm.GetString(s);
                        };
                    } catch (global::System.Exception ex) {
                        _resourceFinder = delegate (string s) {
                            return string.Format("localization error! {0}: {1} ({2})", s, ex.GetType(), ex.Message);
                        };
                    }
                }
                return _resourceFinder;
            }
        }


  /// <summary>Cannot append child to instance of type ${parentType}.</summary>
    internal static string BadAddChild(
    object @parentType
    ) {
        return string.Format(Culture, ResourceFinder("BadAddChild") , @parentType);
    }

  /// <summary>Invalid `source' directive: an invalid or unrecognized URI format.</summary>
    internal static string BadSourceDirective(
    
    ) {
        return string.Format(Culture, ResourceFinder("BadSourceDirective") );
    }

  /// <summary>Invalid `provider' directive: unknown or unresolved provider.</summary>
    internal static string BadTargetProviderDirective(
    
    ) {
        return string.Format(Culture, ResourceFinder("BadTargetProviderDirective") );
    }

  /// <summary>Invalid `type' directive: unknown or unresolved type.</summary>
    internal static string BadTargetTypeDirective(
    
    ) {
        return string.Format(Culture, ResourceFinder("BadTargetTypeDirective") );
    }

  /// <summary>Invalid `type' directive: couldn't resolve specified type or its dependencies.</summary>
    internal static string BadTargetTypeDirectiveUnresolvable(
    
    ) {
        return string.Format(Culture, ResourceFinder("BadTargetTypeDirectiveUnresolvable") );
    }

  /// <summary>Cannot parse text for the property `${property}' (${type}).</summary>
    internal static string BinderConversionError(
    object @property, object @type
    ) {
        return string.Format(Culture, ResourceFinder("BinderConversionError") , @property, @type);
    }

  /// <summary>Member name `${propertyName}' could not be found for the type `${type}'.</summary>
    internal static string BinderMissingProperty(
    object @propertyName, object @type
    ) {
        return string.Format(Culture, ResourceFinder("BinderMissingProperty") , @propertyName, @type);
    }

  /// <summary>Member name `${propertyName}' could not be found.</summary>
    internal static string BinderMissingPropertyNoType(
    object @propertyName
    ) {
        return string.Format(Culture, ResourceFinder("BinderMissingPropertyNoType") , @propertyName);
    }

  /// <summary>Cannot append child nodes to this instance.</summary>
    internal static string CannotAppendChild(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotAppendChild") );
    }

  /// <summary>Cannot delete the property tree root.</summary>
    internal static string CannotDeleteRoot(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotDeleteRoot") );
    }

  /// <summary>A public instance constructor is required for the type: ${typeName}.</summary>
    internal static string ConstructorRequired(
    object @typeName
    ) {
        return string.Format(Culture, ResourceFinder("ConstructorRequired") , @typeName);
    }

  /// <summary>"An error occurred when converting the property value"</summary>
    internal static string ConversionGenericMessage(
    
    ) {
        return string.Format(Culture, ResourceFinder("ConversionGenericMessage") );
    }

  /// <summary>Could not determine the generic parameters for type ${type}.</summary>
    internal static string CouldNotBindGenericParameters(
    object @type
    ) {
        return string.Format(Culture, ResourceFinder("CouldNotBindGenericParameters") , @type);
    }

  /// <summary>Could not initialize an instance of type ${type} from a stream.</summary>
    internal static string CouldNotBindStreamingSource(
    object @type
    ) {
        return string.Format(Culture, ResourceFinder("CouldNotBindStreamingSource") , @type);
    }

  /// <summary>A node with the specified name already exists: `${name}'.</summary>
    internal static string DuplicateProperty(
    object @name
    ) {
        return string.Format(Culture, ResourceFinder("DuplicateProperty") , @name);
    }

  /// <summary>One or more property names have duplicates: ${names}</summary>
    internal static string DuplicatePropertyName(
    object @names
    ) {
        return string.Format(Culture, ResourceFinder("DuplicatePropertyName") , @names);
    }

  /// <summary>Expected ')' in function expression</summary>
    internal static string ExpectedClosingBrace(
    
    ) {
        return string.Format(Culture, ResourceFinder("ExpectedClosingBrace") );
    }

  /// <summary>Failed to load an instance from `${uri}'.</summary>
    internal static string FailedToLoadFromSource(
    object @uri
    ) {
        return string.Format(Culture, ResourceFinder("FailedToLoadFromSource") , @uri);
    }

  /// <summary>The specified type cannot be used as a property tree factory type: `${fullName}'.</summary>
    internal static string InvalidFactoryType(
    object @fullName
    ) {
        return string.Format(Culture, ResourceFinder("InvalidFactoryType") , @fullName);
    }

  /// <summary>The property for the late bound type must be specified.</summary>
    internal static string LateBoundTypeMissing(
    
    ) {
        return string.Format(Culture, ResourceFinder("LateBoundTypeMissing") );
    }

  /// <summary>Specified required late-bound type could not be found: ${typeName}</summary>
    internal static string LateBoundTypeNotFound(
    object @typeName
    ) {
        return string.Format(Culture, ResourceFinder("LateBoundTypeNotFound") , @typeName);
    }

  /// <summary>Could not load the late bound type or its dependencies.</summary>
    internal static string LateBoundTypeNotFoundError(
    
    ) {
        return string.Format(Culture, ResourceFinder("LateBoundTypeNotFoundError") );
    }

  /// <summary>Nodes must have same qualified names to be merged.</summary>
    internal static string MergeNamedMustMatch(
    
    ) {
        return string.Format(Culture, ResourceFinder("MergeNamedMustMatch") );
    }

  /// <summary>Type `${type}' cannot be initialized like a collection because no best `Add' method exists.</summary>
    internal static string NoAddMethodSupported(
    object @type
    ) {
        return string.Format(Culture, ResourceFinder("NoAddMethodSupported") , @type);
    }

  /// <summary>The given property type is not one of the acceptible property types.</summary>
    internal static string NotAcceptiblePropertyType(
    
    ) {
        return string.Format(Culture, ResourceFinder("NotAcceptiblePropertyType") );
    }

  /// <summary>No provider of type `${componentType}' matches the specified criteria.</summary>
    internal static string NoTargetProviderMatches(
    object @componentType
    ) {
        return string.Format(Culture, ResourceFinder("NoTargetProviderMatches") , @componentType);
    }

  /// <summary>The specified property tree is not connected to this client.</summary>
    internal static string NotFromThisClient(
    
    ) {
        return string.Format(Culture, ResourceFinder("NotFromThisClient") );
    }

  /// <summary>An error occurred when invoking property getter or indexer `${name}' (type: ${hint}).</summary>
    internal static string ProblemAccessingProperty(
    object @name, object @hint
    ) {
        return string.Format(Culture, ResourceFinder("ProblemAccessingProperty") , @name, @hint);
    }

  /// <summary>Component or component type or both must be specified.</summary>
    internal static string PropertyTreeMetaObjectComponentNull(
    
    ) {
        return string.Format(Culture, ResourceFinder("PropertyTreeMetaObjectComponentNull") );
    }

  /// <summary>Reader is currently positioned before the first node of the document.</summary>
    internal static string ReaderNotMoved(
    
    ) {
        return string.Format(Culture, ResourceFinder("ReaderNotMoved") );
    }

  /// <summary>Expected reader to be positioned on a `${nodeKind}' node (actual position: `${actualNodeKind}').</summary>
    internal static string ReaderWrongPosition(
    object @nodeKind, object @actualNodeKind
    ) {
        return string.Format(Culture, ResourceFinder("ReaderWrongPosition") , @nodeKind, @actualNodeKind);
    }

  /// <summary>One or more required properties must be specified: ${names} (${hint})</summary>
    internal static string RequiredPropertiesMissing(
    object @names, object @hint
    ) {
        return string.Format(Culture, ResourceFinder("RequiredPropertiesMissing") , @names, @hint);
    }

  /// <summary>The template type must have a public, default constructor: ${type}</summary>
    internal static string TemplateTypeConstructorMustBeNiladic(
    object @type
    ) {
        return string.Format(Culture, ResourceFinder("TemplateTypeConstructorMustBeNiladic") , @type);
    }

  /// <summary>The name ${name} is ambiguous: ${list}</summary>
    internal static string UnableToMatchTypeNameAmbiguous(
    object @name, object @list
    ) {
        return string.Format(Culture, ResourceFinder("UnableToMatchTypeNameAmbiguous") , @name, @list);
    }

  /// <summary>No type could be loaded that matches the name ${name}.</summary>
    internal static string UnableToMatchTypeNameZero(
    object @name
    ) {
        return string.Format(Culture, ResourceFinder("UnableToMatchTypeNameZero") , @name);
    }

  /// <summary>Operation would lead to an invalid document.</summary>
    internal static string WouldCreateMalformedDocument(
    
    ) {
        return string.Format(Culture, ResourceFinder("WouldCreateMalformedDocument") );
    }

  /// <summary>Operation would lead to an malformed document.  A tree is required.</summary>
    internal static string WouldCreateMalformedDocumentRootRequired(
    
    ) {
        return string.Format(Culture, ResourceFinder("WouldCreateMalformedDocumentRootRequired") );
    }

  /// <summary>Operation is not valid for the current writer state, `${state}'.</summary>
    internal static string WriterIncorrectState(
    object @state
    ) {
        return string.Format(Culture, ResourceFinder("WriterIncorrectState") , @state);
    }

  /// <summary>Tree must specify the property trees metadata namespace when used within a property.</summary>
    internal static string WriterOnlyPropertyTreesNamespace(
    
    ) {
        return string.Format(Culture, ResourceFinder("WriterOnlyPropertyTreesNamespace") );
    }

  /// <summary>Specified property or tree name would create a malformed document because the given name has already been written: `${propertyName}'.</summary>
    internal static string WriterPropertyOrTreeExists(
    object @propertyName
    ) {
        return string.Format(Culture, ResourceFinder("WriterPropertyOrTreeExists") , @propertyName);
    }

  /// <summary>Cannot convert from a string to the specified type `${destinationType}'.</summary>
    internal static string CannotConvertFromString(
    object @destinationType
    ) {
        return string.Format(Culture, ResourceFinder("CannotConvertFromString") , @destinationType);
    }

  /// <summary>Cannot convert from the specified instance to a string.</summary>
    internal static string CannotConvertToString(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotConvertToString") );
    }

    }
}
