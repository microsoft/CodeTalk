//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;

namespace Microsoft.CodeTalk.LanguageService
{
    public class Variable : AbstractAddressableEntity
    {
        protected Variable(string name, FileSpan location, ISyntaxEntity parent, CodeFile currentCodeFile) 
            : base(name, location, parent, currentCodeFile)
        {
        }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Variable;
            }
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitVariable(this);
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