//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeTalk.LanguageService;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.CodeTalk.UI
{
    class UIConstants
	{
		public static Dictionary<SyntaxEntityKind, string> UIImageFileDictionary = new Dictionary<SyntaxEntityKind, string>
		{
			{ SyntaxEntityKind.Function, "../res/function.png" },
			{ SyntaxEntityKind.Class, "../res/class.png" },
			{ SyntaxEntityKind.Namespace, "../res/namespace.png" },
			{ SyntaxEntityKind.Error, "../res/error/png" },
			{ SyntaxEntityKind.CodeFile, "../res/default.png" },
			{ SyntaxEntityKind.Comment, "../res/comment.png" },
			{ SyntaxEntityKind.Delegate, "../res/delegate.png" },
			{ SyntaxEntityKind.Enum, "../res/enum.png" },
			{ SyntaxEntityKind.Interface, "../res/interface.png" },
			{ SyntaxEntityKind.Property, "../res/property.png" },
			{ SyntaxEntityKind.Struct, "../res/struct.png" },
			{ SyntaxEntityKind.Variable, "../res/default.png" },
			{ SyntaxEntityKind.Block, "../res/default.png" }
		};

		internal static string GetImageSourceFromKind(ISyntaxEntity node)
		{
			if (null == node)
			{
				return string.Empty;
			}

			if (UIConstants.UIImageFileDictionary.Keys.Contains(node.Kind))
			{
				if (node.Kind == SyntaxEntityKind.CodeFile)
				{
					var currentLang = node.CurrentCodeFile.Language;
					if (currentLang.GetType().Name.Contains("CSharp"))
					{
						return "../res/csharp.png";
					}
					else if (currentLang.GetType().Name.Contains("Python"))
					{
						return "../res/python.png";
					}
					return "../res/default.png";
				}
				else
				{
					return UIConstants.UIImageFileDictionary[node.Kind];
				}
			}
			else
			{
				return "../res/default.png";
			}

		}
	}
}
