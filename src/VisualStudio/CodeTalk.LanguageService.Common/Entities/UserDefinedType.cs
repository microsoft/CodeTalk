//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------


namespace Microsoft.CodeTalk.LanguageService
{
    public abstract class UserDefinedType : AbstractAddressableEntity
    {
        public UserDefinedType(string text, FileSpan fileSpan, ISyntaxEntity parent, CodeFile currentCodeFile)
            : base(text, fileSpan, parent, currentCodeFile)
        {
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitUserDefinedType(this);
        }
    }
}
