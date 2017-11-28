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

namespace Microsoft.CodeTalk.LanguageService
{
	public class CatchBlock : Block
	{
		public CatchBlock(string typeName, FileSpan fileLocation, ISyntaxEntity parent, CodeFile currentCodeFile) 
			: base(typeName, fileLocation, parent, currentCodeFile)
		{

		}

		public string CatchType { get; set; }

		public string CatchFilterClause { get; set; }

		public override SyntaxEntityKind Kind
		{
			get
			{
				return SyntaxEntityKind.Catch;
			}
		}

		public override void AcceptVisitor(ICodeVisitor visitor)
		{
			visitor?.VisitCatch(this);
		}

		public override string SpokenText()
		{
			var whenStr = (string.IsNullOrEmpty(this.CatchFilterClause)) ? string.Empty : String.Format(CultureInfo.InvariantCulture, " when ({0})", this.CatchFilterClause);
			return String.Format(CultureInfo.InvariantCulture, "{0} ({1}){2} at line {3}", this.Kind.ToString(), this.CatchType, whenStr, this.Location.StartLineNumber);
		}

		public override string DisplayText()
		{
            return SpokenText();
		}
	}
}
