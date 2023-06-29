//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeTalk.LanguageService;
using Microsoft.CodeTalk.Properties;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using static Microsoft.CodeTalk.Constants;
using Microsoft.CodeTalk.Talkpoints;
using Microsoft.CodeTalk.Profilepoints;

namespace Microsoft.CodeTalk
{
    public class VSOperations : IEnvironmentOperations, IDisposable
    {
        DTE dte;
        IVsTextManager textManager;
        DebuggerEvents debugEvents;

        List<Talkpoint> mTalkPoints;
        List<Profilepoint> listOfProfilepoints;

        public static FunctionLevelDetailsHandler functionLevelDetailsHandler;

        EnvDTE.Window focussedWindow;

        //Handlers
        _dispDebuggerEvents_OnEnterBreakModeEventHandler _breakHanlder;
        _dispDebuggerEvents_OnExceptionThrownEventHandler _expThrownHandler;
        _dispDebuggerEvents_OnExceptionNotHandledEventHandler _expNotHandledHanlder;

        public VSOperations()
        {
            //Initializing Variables

            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            //dte.Events.WindowEvents.WindowClosing += OnClose;
            textManager = (IVsTextManager)Package.GetGlobalService(typeof(SVsTextManager));
            debugEvents = dte.Events.DebuggerEvents;
            mTalkPoints = new List<Talkpoint>();

			//Setting Handlers
			SetBreakModeHandler();
			SetExceptionHandler();
			//VS Document focussed changed event
			OnDocumentFocusChanged();
		}

        public void SetBreakModeHandler()
        {
            _breakHanlder = new _dispDebuggerEvents_OnEnterBreakModeEventHandler(BreakHandler);
            debugEvents.OnEnterBreakMode += _breakHanlder;
        }

        public void RemoveBreakHandler()
        {
            if (null == _breakHanlder) { return; }
            debugEvents.OnEnterBreakMode -= _breakHanlder;
        }

        public void SetExceptionHandler()
        {
            _expThrownHandler = new _dispDebuggerEvents_OnExceptionThrownEventHandler(ExceptionHandler);
            _expNotHandledHanlder = new _dispDebuggerEvents_OnExceptionNotHandledEventHandler(ExceptionHandler);
            debugEvents.OnExceptionThrown += _expThrownHandler;
            debugEvents.OnExceptionNotHandled += _expNotHandledHanlder;
        }

        public void RemoveExceptionHandler()
        {
            if (null != _expThrownHandler)
            {
                debugEvents.OnExceptionThrown += _expThrownHandler;
            }
            if (null != _expNotHandledHanlder)
            {
                debugEvents.OnExceptionNotHandled += _expNotHandledHanlder;
            }
        }

        public void ExceptionHandler(string ExceptionType, string Name, int code, string Description, ref dbgExceptionAction ExceptionAction)
        {
            System.Windows.Forms.MessageBox.Show("Exception thrown. " +
                                                 "Reason: " + ExceptionType.ToString());
        }

        public void BreakHandler(dbgEventReason reason, ref dbgExecutionAction execAction)
        {
            try
            {
                if (reason == dbgEventReason.dbgEventReasonBreakpoint)
                {
                    //Break due to break point
                    var currentBreakpoint = dte.Debugger.BreakpointLastHit;
                    var matchedTalkpoint = MatchTalkPoint(currentBreakpoint);
                    if (null == matchedTalkpoint) { return; }
                    //Go on
                    matchedTalkpoint.Execute();

                    if (matchedTalkpoint.doesContinue)
                    {
                        execAction = dbgExecutionAction.dbgExecutionActionGo;
                    }
                }
                else if (reason == dbgEventReason.dbgEventReasonExceptionNotHandled)
                {
                    //Break due to unhandled exception
                    System.Windows.Forms.MessageBox.Show("Break due to excpetion. " +
                                                     "Reason: " + reason.ToString());
                }
            }
            catch (Exception)
            {
                //Catching exception to prevent Visual Studio crashing.
            }

        }

        public void SetBreakpointHandler(FunctionLevelDetailsHandler TalkCodefunctionLevelDetailsHandler)
        {
            functionLevelDetailsHandler = TalkCodefunctionLevelDetailsHandler;
        }

        public string RunExpressionInDebugger(string expression)
        {
			var exprResult = dte.Debugger.GetExpression(expression);
			if (exprResult.IsValidValue)
			{
				return exprResult.Value;
			}
			return string.Empty;
        }

        Talkpoint MatchTalkPoint(Breakpoint breakpoint)
        {
            return mTalkPoints.Where(b => (breakpoint.File.Equals(b.filePath) && breakpoint.FileLine == b.position.lineNumber)).FirstOrDefault();
        }

        public void AddProfilePointToCurrentLine(int lineNumber)
        {
            try
            {
                var cursorPos = new CursorPos(lineNumber, 15);
                var filePath = GetActiveDocumentPath();
                //Toggling Talkpoint
                //RemoveIfTalkpointsExists(filePath, cursorPos);
                //if (CheckIfBreakpointExists(filePath, cursorPos))
                //{
                //    RemoveBreakpoints(filePath, cursorPos);
                //    return;
                //}
                Talkpoint profilepoint = new Profilepoint(filePath, cursorPos, true, lineNumber, functionLevelDetailsHandler);
                AddTalkPoint(profilepoint);
            }
            catch (Exception exp)   //We have to catch the exception here, or the IDE can crash
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        public void AddTonalTalkPointToCurrentLine(Tones tone, bool doesContinue)
        {
            try
            {
                var cursorPos = GetCurrentCursorPosition();
                var filePath = GetActiveDocumentPath();
                //Toggling Talkpoint
                RemoveIfTalkpointsExists(filePath, cursorPos);
                if (CheckIfBreakpointExists(filePath, cursorPos))
                {
                    RemoveBreakpoints(filePath, cursorPos);
                    return;
                }
                Talkpoint talkpoint = new ToneTalkpoint(filePath, cursorPos, doesContinue, tone);
                AddTalkPoint(talkpoint);
            }
            catch (Exception exp)   //We have to catch the exception here, or the IDE can crash
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        public void AddTonalTalkPointToCurrentLine(CustomTone customTone, bool doesContinue)
        {
            try
            {
                var cursorPos = GetCurrentCursorPosition();
                var filePath = GetActiveDocumentPath();
                //Toggling Talkpoint
                RemoveIfTalkpointsExists(filePath, cursorPos);
                if (CheckIfBreakpointExists(filePath, cursorPos))
                {
                    RemoveBreakpoints(filePath, cursorPos);
                    return;
                }
                Talkpoint talkpoint = new ToneTalkpoint(filePath, cursorPos, doesContinue, customTone);
                AddTalkPoint(talkpoint);
            }
            catch (Exception exp)   //We have to catch the exception here, or the IDE can crash
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        public void AddTextualTalkpointToCurrentLine(string statement, bool doesContinue)
        {
            try
            {
                var cursorPos = GetCurrentCursorPosition();
                var filePath = GetActiveDocumentPath();
                //Toggling Talkpoint
                RemoveIfTalkpointsExists(filePath, cursorPos);
                if (CheckIfBreakpointExists(filePath, cursorPos))
                {
                    RemoveBreakpoints(filePath, cursorPos);
                    return;
                }
                Talkpoint talkpoint = new TextTalkpoint(filePath, cursorPos, doesContinue, statement);
                AddTalkPoint(talkpoint);
            }
            catch (Exception exp)   //We have to catch the exception here, or the IDE can crash
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        public void AddExpressionTalkpointToCurrentLine(string expression, bool doesContinue)
        {
            try
            {
                var cursorPos = GetCurrentCursorPosition();
                var filePath = GetActiveDocumentPath();
                //Toggling Talkpoint
                RemoveIfTalkpointsExists(filePath, cursorPos);
                if (CheckIfBreakpointExists(filePath, cursorPos))
                {
                    RemoveBreakpoints(filePath, cursorPos);
                    return;
                }
                Talkpoint talkpoint = new ExpressionTalkpoint(filePath, cursorPos, doesContinue, expression);
                AddTalkPoint(talkpoint);
            }
            catch (Exception exp)   //We have to catch the exception here, or the IDE can crash
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        public bool RemoveBreakpointInCurrentPosition()
        {
            var cursorPos = GetCurrentCursorPosition();
            var filePath = GetActiveDocumentPath();
            var flag = CheckIfBreakpointExists(filePath, cursorPos);
            if (flag)
            {
                RemoveBreakpoints(filePath, cursorPos);
            }
            return flag;
        }

        public void AddTalkPoint(Talkpoint talkpoint)
        {
            try
            {
                var Breakpoints = dte.Debugger.Breakpoints;
                // Currently we are only considering one breakpoint, therefore we ignore the column.
                Breakpoints.Add(File: talkpoint.filePath, Line: talkpoint.position.lineNumber);
                mTalkPoints.Add(talkpoint);
            }
            catch (Exception exp)   //We have to catch the exception here, or the IDE can crash
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        public void AddAllProfilepoints()
        {
            //foreach
            IEnumerable<ISyntaxEntity> functions = functionLevelDetailsHandler.GetFunctions();
            List<int> startLineNumbers = functions.Select(func => (func.Location.StartLineNumber + 1)).ToList();
            List<int> endLineNumbers = functions.Select(func => func.Location.EndLineNumber).ToList();
            List<int> profilePointsLineNumbers = new List<int>();
            foreach (var functionSpan in startLineNumbers.Zip(endLineNumbers, Tuple.Create))
            {
                profilePointsLineNumbers.AddRange(Enumerable.Range(functionSpan.Item1, functionSpan.Item2).ToList());
            }
            profilePointsLineNumbers.ForEach(lineNumber => this.AddProfilePointToCurrentLine(lineNumber));
        }

        public void AddAllBreakpoints()
        {
            //foreach
        }

        public bool CheckIfBreakpointExists(string filePath, CursorPos position)
        {
            foreach (Breakpoint breakpoint in dte.Debugger.Breakpoints)
            {
                if (breakpoint.File.Equals(filePath) && breakpoint.FileLine == position.lineNumber)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveIfTalkpointsExists(string filePath, CursorPos position)
        {
            mTalkPoints.RemoveAll(t => (t.filePath.Equals(filePath) && t.position.lineNumber == position.lineNumber));
        }

        public void RemoveBreakpoints(string filePath, CursorPos position)
        {

            mTalkPoints.RemoveAll(b => (b.filePath.Equals(filePath) && b.position.lineNumber == position.lineNumber));

            foreach (Breakpoint breakpoint in dte.Debugger.Breakpoints)
            {
                if (breakpoint.File.Equals(filePath) && breakpoint.FileLine == position.lineNumber)
                {
                    breakpoint.Delete();
                }
            }
        }

        public string GetActiveDocumentPath()
        {
            if (null == dte.ActiveDocument) { return string.Empty; }
            return GetDocumentPath(dte.ActiveDocument);
        }

        public string GetDocumentPath(Document document)
        {
            if (null == document) { return string.Empty; }
            var path = document.Path + document.Name;
            return path;
        }

        public string GetActiveDocumentCode()
        {
            if (null == dte.ActiveDocument) { return string.Empty; }
            return GetDocumentCode(dte.ActiveDocument);
        }

        public string GetDocumentCode(Document document)
        {
            if (null == document) { return string.Empty; }
            TextDocument textDocument = document.Object() as TextDocument;
            var codeText = textDocument.CreateEditPoint(textDocument.StartPoint).GetText(textDocument.EndPoint);
            return codeText;
        }

        public void GoToLocationInActiveDocument(int lineNumber, int columnNumber = 0)
        {
            if (null == dte.ActiveDocument) { return; }
            TextDocument activeDocument = dte.ActiveDocument.Object() as TextDocument;
            dte.ActiveDocument.Activate();
            activeDocument.Selection.GotoLine(lineNumber);
        }

        public bool IsActiveDocumentPresent()
        {
            if (null == dte) { return false; }
            if (null == dte.ActiveDocument) { return false; }
            return true;
        }

        public int GetCursorLineNumber()
        {
            return GetCurrentCursorPosition().lineNumber;
        }

        public int GetCursorColumnNumber()
        {
            return GetCurrentCursorPosition().columnNumber;
        }

        public CursorPos GetCurrentCursorPosition()
        {
            int lineNumber = 0, columnNumber = 0;

            try
            {
                IVsTextView currentTextView = null;
                var doc = dte.ActiveDocument;
                textManager.GetActiveView(1, null, out currentTextView);
                var status = currentTextView.GetCaretPos(out lineNumber, out columnNumber);
                lineNumber++;
            }
            catch (Exception e) //We have to catch the exception here, or the IDE can crash
            {
                Trace.TraceInformation("caught exception of type " + e.GetType() + " message: " + e.Message);
                return new CursorPos(true);
            }
            return new CursorPos(lineNumber, columnNumber);
        }

        public static void PlaySound(Tones tone)
        {
            try
            {
                SoundPlayer audio;
                switch (tone)
                {
                    case Tones.error1:
                        audio = new SoundPlayer(AudioResource.error);
                        break;
                    case Tones.error2:
                        audio = new SoundPlayer(AudioResource.errorBeep2);
                        break;
                    default:
                        audio = new SoundPlayer(AudioResource.error);
                        break;
                }

                audio.Play();
                audio.Dispose();
                //SoundPlayer soundPlayer = new SoundPlayer(stream);
                //soundPlayer.PlaySync();
            }
            catch (Exception e) //We have to catch the exception here, or the IDE can crash
            {
                System.Windows.Forms.MessageBox.Show("error finding resource. " + e.StackTrace);
                Console.WriteLine(e.Message);
            }
        }

        public int GetProcessId()
        {
            Debug.WriteLine("Getting into GetProcessId");
            EnvDTE.Process process = dte.Debugger.CurrentProcess;
            if (process == null)
                return -1;
            else
            {
                Debug.WriteLine("Currently Debugging ID: " + process.ProcessID + " name: " + process.Name);
                return process.ProcessID;
            }
        }

        public static void PlaySound(CustomTone customTone)
        {
            try
            {
                if (string.IsNullOrEmpty(customTone.GetTonePath())) { return; }
                var audio = new SoundPlayer(customTone.GetTonePath());
                audio.Play();
            }
            catch (Exception exp)   //We have to catch the exception here, or the IDE can crash
            {
                System.Diagnostics.Debug.WriteLine(exp.StackTrace);
            }
        }

        public void OnDocumentFocusChanged()
        {
            dte.Events.WindowEvents.WindowActivated += OnWindowActivated;
        }

        private void OnWindowActivated(EnvDTE.Window gotFocus, EnvDTE.Window lostFocus)
        {
            focussedWindow = gotFocus;
            if (null != gotFocus)
            {
                System.Diagnostics.Debug.WriteLine("Got Focus " + gotFocus);
                if (null != gotFocus.Document)
                {
                    System.Diagnostics.Debug.WriteLine(gotFocus.Document.Name);
                }
            }
            if (null != lostFocus)
            {
                System.Diagnostics.Debug.WriteLine("Lost Focus " + lostFocus);
                if (null != lostFocus.Document)
                {
                    System.Diagnostics.Debug.WriteLine(lostFocus.Document.Name);
                }
            }

        }

        public bool IsActiveDocumentFocussed()
        {
            try
            {
                if (null != focussedWindow && null != focussedWindow.Document)
                {
                    //If current focussed and has a document, compare the path
                    var focussedDocumentPath = GetDocumentPath(focussedWindow.Document);
                    var activeDocumentPath = GetActiveDocumentPath();
                    if (focussedDocumentPath.Equals(activeDocumentPath))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                //The active document can be disposed
            }
            return false;
        }

        public void PlaySoundIfError()
        {
            if (CodeContainsErrors() == true)
            {
                PlaySound(Tones.error2);
            }
        }

        private bool CodeContainsErrors()
        {
            List<ISyntaxEntity> errors = new List<ISyntaxEntity>();
            try
            {


                //Getting the code text from the active document
                var path = GetActiveDocumentPath();
                var codeText = GetActiveDocumentCode();
                if (string.IsNullOrEmpty(path)) { return false; }
                //Creating a language service
                var lService = new Microsoft.CodeTalk.LanguageService.Language(path);

                //Parse the code and get the list of errors
                errors = lService.GetDiagnostics(codeText).ToList().ConvertAll(error => (ISyntaxEntity)error);

            }
            catch (Exception e) //We have to catch the exception here, or the IDE can crash
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
            if (0 != errors.Count)
            {
                return true;
            }
            else
            {
                Trace.TraceInformation("returning false.");
                return false;
            }

        }

        //private void OnClose(EnvDTE.Window window)
        //{
        //   if(true == CodeContainsErrors())
        //    {
        //        DialogResult userChoice = System.Windows.Forms.MessageBox.Show("The file contains syntax errors. Are you sure you want to exit?", "CodeTalk", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //        if(userChoice == DialogResult.Yes)
        //        {
        //            window.Close(vsSaveChanges.vsSaveChangesPrompt);
        //        }
        //    }
        //}

        static class SafeNativeMethods
        {
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern bool PostMessage(HandleRef hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        }

        public void Dispose()
        {
            RemoveBreakHandler();
            RemoveExceptionHandler();
        }
    }
}
