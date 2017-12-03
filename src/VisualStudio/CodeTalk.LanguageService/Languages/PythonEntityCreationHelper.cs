//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IronPython.Compiler.Ast;
using Microsoft.CodeTalk.LanguageService.Entities.UDT;

namespace Microsoft.CodeTalk.LanguageService
{
    /// <summary>
    /// <summary>
    /// This is a helper class used to create ILanguage entities for the python language service. these entities are used to populate the CodeFile for PythonLanguageService.
    /// </summary>
    internal static class PythonEntityCreationHelper
    {

        /// <summary>
        /// This helper function creates a UDT of udtTypes.class
        /// </summary>
        /// <param name="node"> The ClassDefinition node to create the class UDT</param>
        /// <param name="currentCodeFile">The CodeFile</param>
        /// <param name="parent">the parent</param>
        /// <returns>the UDT from the ClassDefinition node</returns>
        internal static UserDefinedType createClassUDT(IronPython.Compiler.Ast.ClassDefinition node, CodeFile currentCodeFile,ISyntaxEntity parent)
        {

			UserDefinedType classObject = new Entities.UDT.ClassDefinition(node.Name, new FileSpan(node.Start.Line, node.Start.Column, node.End.Line, node.End.Column), parent, currentCodeFile);
            return classObject;
        }

        /// <summary>
        /// <param name="node">The FunctionDefinition node</param>
        /// <param name="currentCodeFile">The current CodeFile</param>
        /// <param name="parent">The parent of the current CodeFile</param>
        /// <returns>The function object.</returns>
        internal static FunctionDefinition createFunction(IronPython.Compiler.Ast.FunctionDefinition node, CodeFile currentCodeFile, ISyntaxEntity parent)
        {
            FunctionDefinition fn = new FunctionDefinition(node.Name, new FileSpan(node.Start.Line, node.Start.Column, node.End.Line, node.End.Column), parent, currentCodeFile);
            if (parent.Kind == SyntaxEntityKind.Class)
            {
                fn.TypeOfFunction = FunctionTypes.MemberFunction;
            } else if(parent.Kind == SyntaxEntityKind.Function)
            {
                fn.TypeOfFunction = FunctionTypes.AnonymousFunction;
            } else
            {
                fn.TypeOfFunction = FunctionTypes.GlobalFunction;
            }
            
            return fn;
            
        }

        internal static ForBlock createForBlock(IronPython.Compiler.Ast.ForStatement node, CodeFile currentCodeFile, ISyntaxEntity parent)
        {
            ForBlock forBlock = new ForBlock("", new FileSpan(node.Start.Line, node.Start.Column, node.End.Line, node.End.Column), parent, currentCodeFile);
            return forBlock;
        }

        internal static ForBlock createComprehensionForBlock(IronPython.Compiler.Ast.ComprehensionFor node, CodeFile currentCodeFile, ISyntaxEntity parent)
        {
            ForBlock forBlock = new ForBlock("", new FileSpan(node.Start.Line, node.Start.Column, node.End.Line, node.End.Column), parent, currentCodeFile);
            return forBlock;
        }

        internal static WhileBlock createWhileBlock(IronPython.Compiler.Ast.WhileStatement node, CodeFile currentCodeFile, ISyntaxEntity parent)
        {
            WhileBlock whileBlock = new WhileBlock("", new FileSpan(node.Start.Line, node.Start.Column, node.End.Line, node.End.Column), parent, currentCodeFile);
            return whileBlock;
        }

        internal static IfBlock createIfBlock(IronPython.Compiler.Ast.IfStatement node, CodeFile currentCodeFile, ISyntaxEntity parent)
        {
            IfBlock ifBlock = new IfBlock("", new FileSpan(node.Start.Line, node.Start.Column, node.End.Line, node.End.Column), parent, currentCodeFile);
            return ifBlock;
        }

        internal static IfBlock createComprehensionIfBlock(IronPython.Compiler.Ast.ComprehensionIf node, CodeFile currentCodeFile, ISyntaxEntity parent)
        {
            IfBlock ifBlock = new IfBlock("", new FileSpan(node.Start.Line, node.Start.Column, node.End.Line, node.End.Column), parent, currentCodeFile);
            return ifBlock;
        }

        internal static TryBlock createTryBlock(IronPython.Compiler.Ast.TryStatement node, CodeFile currentCodeFile, ISyntaxEntity parent)
        {
            TryBlock tryBlock = new TryBlock("", new FileSpan(node.Start.Line, node.Start.Column, node.End.Line, node.End.Column), parent, currentCodeFile);
            return tryBlock;
        }
    }
}
