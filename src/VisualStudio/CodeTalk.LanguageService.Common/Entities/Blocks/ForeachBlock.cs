//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------


namespace Microsoft.CodeTalk.LanguageService
{
    public class ForEachBlock : Block
	{
		public ForEachBlock(string typeName, FileSpan fileLocation, ISyntaxEntity parent, CodeFile currentCodeFile) 
			: base(typeName, fileLocation, parent, currentCodeFile)
		{

		}

		public override SyntaxEntityKind Kind
		{
			get
			{
				return SyntaxEntityKind.ForEach;
			}
		}

		public override void AcceptVisitor(ICodeVisitor visitor)
		{
			visitor?.VisitForEach(this);
		}
	}
}
