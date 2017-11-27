//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public override string SpokenText()
		{
			return this.CurrentCodeFile.Language.SpokenText(this);
		}

		public override string DisplayText()
		{
			return base.DisplayText();
		}
	}
}
