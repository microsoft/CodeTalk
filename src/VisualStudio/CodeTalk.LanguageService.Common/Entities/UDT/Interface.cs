//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------


namespace Microsoft.CodeTalk.LanguageService.Entities.UDT
{
    public class InterfaceDefinition : InheritableUserDefinedType
    {
        public InterfaceDefinition(string text, FileSpan fileSpan, ISyntaxEntity parent, CodeFile currentCodeFile) : base(text, fileSpan, parent, currentCodeFile)
        {
        }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Interface;
            }
        }
    }
}
