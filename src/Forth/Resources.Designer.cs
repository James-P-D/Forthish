﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Forth {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Forth.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is already defined.
        /// </summary>
        internal static string AlreadyDefined {
            get {
                return ResourceManager.GetString("AlreadyDefined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to fractional
        ///3.14 constant pi
        ///
        ///decimal
        ///42 constant meaning-of-life
        ///0 constant true
        ///-1 constant false.
        /// </summary>
        internal static string Constants {
            get {
                return ResourceManager.GetString("Constants", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (Push &apos;cell&apos; size to stack, multiplies top two items of stack, leaving result on stack)
        ///: cells cell * ;
        ///
        ///(Common shorthand for getting the displaying the value of a variable)
        ///: ? @ . ;
        ///
        ///(Increment)
        ///: +1 1 + ;
        ///
        ///(Decrement)
        ///: -1 1 - ;
        ///
        ///(Is zero)
        ///: ?0 0 = ;
        ///
        ///.
        /// </summary>
        internal static string Definitions {
            get {
                return ResourceManager.GetString("Definitions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected &apos;{0}.
        /// </summary>
        internal static string Expected_ {
            get {
                return ResourceManager.GetString("Expected_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected a name.
        /// </summary>
        internal static string ExpectedAName {
            get {
                return ResourceManager.GetString("ExpectedAName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected &apos;;&apos; at end of definition.
        /// </summary>
        internal static string ExpectedEndDefinition {
            get {
                return ResourceManager.GetString("ExpectedEndDefinition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a valid character.
        /// </summary>
        internal static string InvalidCharacter {
            get {
                return ResourceManager.GetString("InvalidCharacter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a valid float.
        /// </summary>
        internal static string InvalidFloat {
            get {
                return ResourceManager.GetString("InvalidFloat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a valid hex integer.
        /// </summary>
        internal static string InvalidHexInteger {
            get {
                return ResourceManager.GetString("InvalidHexInteger", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a valid integer.
        /// </summary>
        internal static string InvalidInteger {
            get {
                return ResourceManager.GetString("InvalidInteger", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Memory overflow.
        /// </summary>
        internal static string MemoryOverflow {
            get {
                return ResourceManager.GetString("MemoryOverflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Memory underflow.
        /// </summary>
        internal static string MemoryUnderflow {
            get {
                return ResourceManager.GetString("MemoryUnderflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a Value.
        /// </summary>
        internal static string NameIsNotAValue {
            get {
                return ResourceManager.GetString("NameIsNotAValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not an &apos;if&apos; command.
        /// </summary>
        internal static string NotAnIfCommand {
            get {
                return ResourceManager.GetString("NotAnIfCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Problem loading resource: &apos;{0}&apos;.
        /// </summary>
        internal static string ProblemLoadingResource {
            get {
                return ResourceManager.GetString("ProblemLoadingResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Problem loading source file: &apos;{0}&apos;.
        /// </summary>
        internal static string ProblemLoadingSourceFile {
            get {
                return ResourceManager.GetString("ProblemLoadingSourceFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing stack overflow.
        /// </summary>
        internal static string ProcessingStackOverflow {
            get {
                return ResourceManager.GetString("ProcessingStackOverflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stack overflow.
        /// </summary>
        internal static string StackOverflow {
            get {
                return ResourceManager.GetString("StackOverflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stack underflow.
        /// </summary>
        internal static string StackUnderflow {
            get {
                return ResourceManager.GetString("StackUnderflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown item: &apos;{0}&apos;.
        /// </summary>
        internal static string UnknownItem {
            get {
                return ResourceManager.GetString("UnknownItem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect Cell size.
        /// </summary>
        internal static string WrongCellSize {
            get {
                return ResourceManager.GetString("WrongCellSize", resourceCulture);
            }
        }
    }
}