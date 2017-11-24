//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;

namespace Microsoft.CodeTalk.LanguageService
{
    public class Comment : AbstractSyntaxEntity
    {
        public Comment(string text, FileSpan span, ISyntaxEntity belongsTo, CodeFile currentCodeFile)
            : base(text, span, belongsTo, currentCodeFile)
        {
        }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Comment;
            }
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitComment(this);
        }

        public override string SpokenText()
        {
            return base.SpokenText();
        }

        public override string DisplayText()
        {
            return base.DisplayText();
        }
    }
}