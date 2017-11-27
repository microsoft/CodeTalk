//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.LanguageService
{
	public class ElseBlock : Block
	{
		public ElseBlock(string typeName, FileSpan fileLocation, ISyntaxEntity parent, CodeFile currentCodeFile) 
			: base(typeName, fileLocation, parent, currentCodeFile)
		{

		}

		public override SyntaxEntityKind Kind
		{
			get
			{
				return SyntaxEntityKind.Else;
			}
		}

		public override void AcceptVisitor(ICodeVisitor visitor)
		{
			visitor?.VisitElse(this);
		}

		public override string SpokenText()
		{
			return this.Kind.ToString() + " at line " + this.Location.StartLineNumber;
		}

		public override string DisplayText()
		{
			return this.Kind.ToString() + " at line " + this.Location.StartLineNumber;
		}
	}
}
