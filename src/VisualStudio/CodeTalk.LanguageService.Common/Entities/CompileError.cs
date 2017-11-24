//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.CodeTalk.LanguageService
{
    public class CompileError : AbstractSyntaxEntity
    {
        //public FileSpan Location { get; }

        public string ErrorText { get; }

        public IEnumerable<string> SuggestedRemedies { get; }
        public CompileError(string errorText, FileSpan location)
        {
            this.ErrorText = errorText;
            this.Location = location;
            this.Name = errorText;
        }

        public override SyntaxEntityKind Kind
        {
            get
            {
                return SyntaxEntityKind.Error;
            }
        }

        public override void AcceptVisitor(ICodeVisitor visitor)
        {
            // no implementation here
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