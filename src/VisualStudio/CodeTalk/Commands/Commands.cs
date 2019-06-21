//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.CodeTalk.LanguageService;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.CodeTalk.UI;
using System.Resources;
using Microsoft.CodeTalk.Properties;
using System.Globalization;

namespace Microsoft.CodeTalk.Commands
{
    public class GetFunctionsCommand : CommandBase
    {
        ResourceManager rm;

        public GetFunctionsCommand(CommandKeyConfig keys) : base(keys) { }
        
        public override void Execute()
        {
            rm = new ResourceManager(typeof(Resources));
            System.Diagnostics.Debug.WriteLine("Get Functions");

            IEnumerable<ISyntaxEntity> functions = new List<ISyntaxEntity>();
            CodeFile codeFile;

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
                MessageBox.Show(rm.GetString("CompilationErrorMessage", CultureInfo.CurrentCulture), rm.GetString("CodeTalkString", CultureInfo.CurrentCulture), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Creating a function collector for getting all the functions
            var functionCollector = new FunctionCollector();
            functionCollector.VisitCodeFile(codeFile);

            //Getting all the functions
            functions = functionCollector.FunctionsInFile;

            if (0 == functions.Count())
            {
                MessageBox.Show(rm.GetString("NoFunctionsString", CultureInfo.CurrentCulture), rm.GetString("CodeTalkString", CultureInfo.CurrentCulture), MessageBoxButtons.OK);
                return;
            }

            ToolWindowPane listFunctionsWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(AccessibilityToolWindow), 0, true);

            if ((null == listFunctionsWindow) || (null == listFunctionsWindow.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)listFunctionsWindow.Frame;
            windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, rm.GetString("FunctionsListTitle", CultureInfo.CurrentCulture));

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            (listFunctionsWindow as AccessibilityToolWindow).windowControl.SetListView(functions, rm.GetString("FunctionsListTitle", CultureInfo.CurrentCulture));
        }

        public override bool PassControl() => false;
    }

    public class GetContextCommand : CommandBase
    {
        ResourceManager rm;

        public GetContextCommand(CommandKeyConfig keys) : base(keys) {  }

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
                codeFile = lService.Parse(codeText, System.IO.Path.GetFileName(path));
            }
            catch (CodeTalkLanguageServiceException)
            {
                MessageBox.Show(rm.GetString("CompilationErrorMessage", CultureInfo.CurrentCulture), rm.GetString("CodeTalkString", CultureInfo.CurrentCulture), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                int currentLineNumber = 0;

                currentLineNumber = TalkCodePackage.vsOperations.GetCurrentCursorPosition().lineNumber;
                contextHierarchy = codeFile.GetContextAtLine(currentLineNumber) as IList<ISyntaxEntity>;

                if (0 == contextHierarchy.Count)
                {
                    MessageBox.Show(rm.GetString("NoContextString", CultureInfo.CurrentCulture) + " " + currentLineNumber.ToString(), rm.GetString("CodeTalkString", CultureInfo.CurrentCulture), MessageBoxButtons.OK);
                    return;
                }

                var entities = contextHierarchy.Reverse().ToList().ConvertAll(x => (ISyntaxEntity)x);

                ToolWindowPane listFunctionsWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(AccessibilityToolWindow), 0, true);

                if ((null == listFunctionsWindow) || (null == listFunctionsWindow.Frame))
                {
                    MessageBox.Show("Cannote create tool window", "Cannot create tool window", MessageBoxButtons.OK);
                }

                IVsWindowFrame windowFrame = (IVsWindowFrame)listFunctionsWindow.Frame;
                windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, rm.GetString("GetContextTitle", CultureInfo.CurrentCulture));

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

                (listFunctionsWindow as AccessibilityToolWindow).windowControl.SetListView(entities, rm.GetString("GetContextTitle", CultureInfo.CurrentCulture), false);
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("The cursor does not seem to be in code. please move the cursor and try again.", "CodeTalk", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class GetClassesCommand : CommandBase
    {
        public GetClassesCommand(CommandKeyConfig keys) : base(keys) { }

        public override void Execute()
        {
            System.Diagnostics.Debug.WriteLine("Get Classes");
        }

        public override bool PassControl() => false;
    }

    public class GetErrorsCommand : CommandBase
    {
        ResourceManager rm;

        public GetErrorsCommand(CommandKeyConfig keys) : base(keys) { }

        public override void Execute()
        {
            rm = new ResourceManager(typeof(Resources));
            System.Diagnostics.Debug.WriteLine("Get Errors");

            var errors = new List<ISyntaxEntity>();

            //Getting the code text from the active document
            var path = TalkCodePackage.vsOperations.GetActiveDocumentPath();
            var codeText = TalkCodePackage.vsOperations.GetActiveDocumentCode();

            //Creating a language service
            var lService = new Language(path);

                        //Parse the code and get the list of errors
            errors = lService.GetDiagnostics(codeText).ToList().ConvertAll(error => (ISyntaxEntity)error);

            if (0 == errors.Count)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    MessageBox.Show(rm.GetString("NoErrorsString", CultureInfo.CurrentCulture), rm.GetString("CodeTalkString", CultureInfo.CurrentCulture), MessageBoxButtons.OK);
                });
                return;
            }

            ToolWindowPane listFunctionsWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(AccessibilityToolWindow), 0, true);

            if ((null == listFunctionsWindow) || (null == listFunctionsWindow.Frame))
            {
                MessageBox.Show("Cannote create tool window", "Cannot create tool window", MessageBoxButtons.OK);
            }
            else
            {
                IVsWindowFrame windowFrame = (IVsWindowFrame)listFunctionsWindow.Frame;
                windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, rm.GetString("ErrorListTitle", CultureInfo.CurrentCulture));

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

                (listFunctionsWindow as AccessibilityToolWindow).windowControl.SetListView(errors, rm.GetString("ErrorListTitle", CultureInfo.CurrentCulture));
            }

            
        }

        public override bool PassControl() => false;
    }

    public class GetSummaryCommand : CommandBase
    {
        ResourceManager rm;

        public GetSummaryCommand(CommandKeyConfig keys) : base(keys) { }

        public override void Execute()
        {
            rm = new ResourceManager(typeof(Resources));
            System.Diagnostics.Debug.WriteLine("Get Summary");

            //Getting the code text from the active document
            var path = TalkCodePackage.vsOperations.GetActiveDocumentPath();
            var codeText = TalkCodePackage.vsOperations.GetActiveDocumentCode();

            //Creating a language service
            var lService = new Language(path);

            try
            {
                //Parsing the code
                CodeFile codeFile = lService.Parse(codeText, System.IO.Path.GetFileName(path));

                ToolWindowPane listFunctionsWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(GetSummaryToolWindow), 0, true);

                if ((null == listFunctionsWindow) || (null == listFunctionsWindow.Frame))
                {
                    MessageBox.Show("Cannote create tool window", "Cannot create tool window", MessageBoxButtons.OK);
                }
                else
                {
                    IVsWindowFrame windowFrame = (IVsWindowFrame)listFunctionsWindow.Frame;
                    windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, rm.GetString("FileSummaryTitle", CultureInfo.CurrentCulture));
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

                    (listFunctionsWindow as GetSummaryToolWindow).windowControl.SetTreeView(codeFile, rm.GetString("FileSSummaryTitle", CultureInfo.CurrentCulture));
                }
                
            }
            catch (CodeTalkLanguageServiceException)
            {
                MessageBox.Show(rm.GetString("FileProcessError", CultureInfo.CurrentCulture));
            }
        }

        public override bool PassControl() => false;
    }


    public class CreateBreakpointCommand : CommandBase
    {
        ResourceManager rm;

        public CreateBreakpointCommand(CommandKeyConfig keys) : base(keys) { }

        public override void Execute()
        {
            rm = new ResourceManager(typeof(Resources));

            if (TalkCodePackage.vsOperations.RemoveBreakpointInCurrentPosition()) { return; }

            ToolWindowPane talkpointWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(TalkpointToolWindow), 0, true);

            if ((null == talkpointWindow) || (null == talkpointWindow.Frame))
            {
                MessageBox.Show("Cannote create breakpoint window", "Cannot create tool window", MessageBoxButtons.OK);
            }
            else
            {
                IVsWindowFrame twindowFrame = (IVsWindowFrame)talkpointWindow.Frame;
                twindowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, rm.GetString("TalkPointWindowTitle", CultureInfo.CurrentCulture));

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(twindowFrame.Show());
                ((talkpointWindow as TalkpointToolWindow).Content as TalkpointToolWindowControl).ClearAll();
            }
        }

        public override bool PassControl() => false;
    }

    public class SetProfilepointsCommand : CommandBase
    {
        ResourceManager rm;
        public static VSOperations vsOperations { get; set; }
        public SetProfilepointsCommand(CommandKeyConfig keys) : base(keys) { }

        public override void Execute()
        {
            vsOperations.AddAllProfilepoints();
        }

        public override bool PassControl() => false;
    }


}
