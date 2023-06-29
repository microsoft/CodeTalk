//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------



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
        /// <summary>
        /// </summary>
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
    }
}
