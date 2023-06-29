//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------


namespace Microsoft.CodeTalk.LanguageService
{
    public class ForBlock : Block
	{
		public ForBlock(string typeName, FileSpan fileLocation, ISyntaxEntity parent, CodeFile currentCodeFile) 
			: base(typeName, fileLocation, parent, currentCodeFile)
		{

		}

		public override SyntaxEntityKind Kind
		{
			get
			{
				return SyntaxEntityKind.For;
			}
		}

		public override void AcceptVisitor(ICodeVisitor visitor)
		{
			visitor?.VisitFor(this);
		}
	}
}
