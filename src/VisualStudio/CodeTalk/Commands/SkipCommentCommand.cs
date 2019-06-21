//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeTalk.LanguageService;
using System.Windows.Forms;
using System.Resources;
using Microsoft.CodeTalk.Properties;
using System.Globalization;

namespace Microsoft.CodeTalk.Commands
{
    class SkipCommentCommand : CommandBase
    {
		ResourceManager rm;

		int lineNumber = 0;

        public SkipCommentCommand(CommandKeyConfig keys) : base(keys) { }

        public string keys { get; }

        public override bool PassControl()
        {
            return false;
        }

        public override void Execute()
        {
			rm = new ResourceManager(typeof(Resources));

			CodeFile codeFile;
            Comment commentAtLine = null;
            //Getting the code text from the active document
            var path = TalkCodePackage.vsOperations.GetActiveDocumentPath();
            var codeText = TalkCodePackage.vsOperations.GetActiveDocumentCode();

            //Creating a language service
            var lService = new Language(path);
            try
            {
                //Parsing the code
                codeFile = lService.Parse(codeText, path);
            }
            catch (CodeTalkLanguageServiceException)
            {
				MessageBox.Show(rm.GetString("CompilationErrorString", CultureInfo.CurrentCulture), rm.GetString("CodeTalkText", CultureInfo.CurrentCulture), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
            }
            lineNumber = TalkCodePackage.vsOperations.GetCurrentCursorPosition().lineNumber;
            commentAtLine = codeFile.GetCommentAtLine(lineNumber);
            if (commentAtLine != null)
            {
                if (commentAtLine.Parent != null)
                {
                    TalkCodePackage.vsOperations.GoToLocationInActiveDocument(commentAtLine.Parent.Location.StartLineNumber);
                }
                else
                {
                }
            }
            else
            {
            }
        }

    }
}