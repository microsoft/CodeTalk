//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.CodeTalk.LanguageService
{
    public class Block : AbstractSyntaxEntity
    {
        protected Block(string name, FileSpan location, ISyntaxEntity parent, CodeFile currentCodeFile) 
            : base(name, location, parent, currentCodeFile)
        {
            //Edited by prvai : need not be true.
            //Debug.Assert(parent.Kind == SyntaxEntityKind.Function, "A block can only have a function as parent.");

        }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Block;
            }
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            visitor?.VisitBlock(this);
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