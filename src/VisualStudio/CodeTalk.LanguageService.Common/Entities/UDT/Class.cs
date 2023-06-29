//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------


namespace Microsoft.CodeTalk.LanguageService.Entities.UDT
{
    public class ClassDefinition : InheritableUserDefinedType
    {
        public ClassDefinition(string text, FileSpan fileSpan, ISyntaxEntity parent, CodeFile currentCodeFile) : base(text, fileSpan, parent, currentCodeFile)
        {
        }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Class;
            }
        }
    }
}
