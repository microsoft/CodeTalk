//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.CodeTalk.LanguageService;
using System.Diagnostics;
using System.Resources;
using Microsoft.CodeTalk.Properties;
using System.Globalization;

namespace Microsoft.CodeTalk.Commands
{
    public class MoveToContextCommand: CommandBase
    {
		ResourceManager rm;

		public MoveToContextCommand(CommandKeyConfig keys): base(keys) { }

        public override bool PassControl()
        {
            return false;
        }

        public override void Execute()
        {
			rm = new ResourceManager(typeof(Resources));

			CodeFile codeFile;
            IList<ISyntaxEntity> contextHierarchy;

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
            try
            {
                int currentLineNumber = 0;
                int destinationLineNumber = 0;

                currentLineNumber = TalkCodePackage.vsOperations.GetCurrentCursorPosition().lineNumber;
                contextHierarchy = codeFile.GetContextAtLine(currentLineNumber) as IList<ISyntaxEntity>;

                if (0 == contextHierarchy.Count)
                {
                    MessageBox.Show("No context found in this line.", "CodeTalk", MessageBoxButtons.OK);
                    return;
                }
                foreach(var entity in contextHierarchy)
                {
                    destinationLineNumber = entity.Location.StartLineNumber;
                    //prvai : edited the condition check currentLineNumber+1 != destinationLineNumber
                    if (currentLineNumber != destinationLineNumber) //CurrentLineNumber+1 to make sure that cursor moves to enclosing context.
                    {
                        break;
                    }
                }
                TalkCodePackage.vsOperations.GoToLocationInActiveDocument(destinationLineNumber);
            } catch(Exception e)
            {
                Trace.TraceError("uncaught exception."+e.StackTrace);
            }

            }

    }
}
