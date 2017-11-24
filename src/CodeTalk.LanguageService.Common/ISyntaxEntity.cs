//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService
{
    public enum SyntaxEntityKind
    {
        CodeFile,
        Namespace,
        Class,
        Interface,
        Function,
        Variable,
        Property,
        Block,
        Comment,
        Error, /* Is this correct? Keeping it for the build demo. */
        Delegate,
        Struct, 
        Enum,
        If,
        Else,
        Try,
        Catch,
        For,
        ForEach,
        While,
        Switch,
        Case
    }

    [Flags]
    public enum AccessSpecifiers { Public = 1, Private = 2, Internal = 4, Package = 8, Protected = 16, None = 0 };

    [Flags] //Because static extern is possible.
    public enum StorageSpecifiers { None = 0, Static = 1, Instance = 2, Global = 4, Extern = 8 }; 


    /// <summary>
    /// Why are we simply not using Roslyn's classes? Because they are more complex
    /// and offer a very detailed representation of a program. We want something 
    /// simpler. 
    /// </summary>
    public interface ISyntaxEntity
    {
        /// <summary>
        /// The name is simply the name of the entity without 
        /// any 'decoration'. Like "ISyntaxEntity".
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Location in the file. 
        /// </summary>
        FileSpan Location { get; }

        /// <summary>
        /// Any parent - for instance, parent of a function
        /// could be the containing class. 
        /// </summary>
        ISyntaxEntity Parent { get; }

        /// <summary>
        /// The associated comment is typically the comment for the 
        /// entity like a class or function as opposed to a standalone
        /// comment inside a function or elsewhere. 
        /// </summary>
        Comment AssociatedComment { get; }

        /// <summary>
        /// Mimicing what Roslyn does with its SyntaxNodes. The kind
        /// specifies the type of the entity. Useful for iterators to
        /// hunt for specific types of entities.
        /// </summary>
        SyntaxEntityKind Kind { get; }

        /// <summary>
        /// The current code file object that contains this entity. 
        /// This object can be used to access language-specific 
        /// information through the Language object of the CodeFile
        /// class. So, things like the names of the d'tor and c'tor, 
        /// and case-sensitivity of the language can all be found using
        /// this pointer. 
        /// </summary>
        CodeFile CurrentCodeFile { get;  }

        /// <summary>
        /// Children is a loose definition. For classes, namespaces, e.t.c
        /// Children are any members they contain, including static functions/
        /// variables. For functions, children are the function signature 
        /// and function body. For variables, children are null. Comments can
        /// also be children of FunctionBody.
        /// </summary>
        IEnumerable<ISyntaxEntity> Children { get; }

        /// <summary>
        /// Set of errors that have occurred at a particular entity. 
        /// </summary>
        IEnumerable<CompileError> ErrorsAtEntity { get;  }

        /// <summary>
        /// Add a child to the current entity. Implementation can choose to 
        /// check for the type of the child, e.t.c
        /// </summary>
        /// <param name="child"></param>
        void AddChild(ISyntaxEntity child);

        /// <summary>
        /// Standard implementation of the visitor pattern. We are only offering
        /// the implementation where the visitor maintains STATE. i.e. there are 
        /// no return values. 
        /// </summary>
        /// <param name="visitor"></param>
        void AcceptVisitor(ICodeVisitor visitor);

        string SpokenText();

        string DisplayText();
    }

}
