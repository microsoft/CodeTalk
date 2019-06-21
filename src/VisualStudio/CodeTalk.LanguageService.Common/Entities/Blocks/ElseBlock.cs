//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------


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
	}
}
