using static Microsoft.CodeTalk.Constants;

namespace Microsoft.CodeTalk
{
    public interface IEnvironmentOperations
    {
        /// <summary>
        /// Method to Evaluate Expression in Debugging context
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string RunExpressionInDebugger(string expression);

        /// <summary>
        /// Method to add a tone Talkpoint with existing tones
        /// </summary>
        /// <param name="tone"></param>
        /// <param name="doesContinue"></param>
        void AddTonalTalkPointToCurrentLine(Tones tone, bool doesContinue);

        /// <summary>
        /// Method to add tone Talkpoint with Custom tones
        /// </summary>
        /// <param name="customTone"></param>
        /// <param name="doesContinue"></param>
        void AddTonalTalkPointToCurrentLine(CustomTone customTone, bool doesContinue);

        /// <summary>
        /// Method to add text Talkpoint
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="doesContinue"></param>
        void AddTextualTalkpointToCurrentLine(string statement, bool doesContinue);

        /// <summary>
        /// Method to add expression Talkpoint
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="doesContinue"></param>
        void AddExpressionTalkpointToCurrentLine(string expression, bool doesContinue);

        /// <summary>
        /// Method to remove exixting breakpoints in current line
        /// </summary>
        /// <returns></returns>
        bool RemoveBreakpointInCurrentPosition();

        /// <summary>
        /// Method to get the file path of the active document
        /// </summary>
        /// <returns></returns>
        string GetActiveDocumentPath();

        /// <summary>
        /// Method to get the code text of the active document
        /// </summary>
        /// <returns></returns>
        string GetActiveDocumentCode();

        /// <summary>
        /// Method to go to the given location in the active document
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="columnNumber"></param>
        void GoToLocationInActiveDocument(int lineNumber, int columnNumber);

        /// <summary>
        /// Method to get the current cursor position
        /// </summary>
        /// <returns></returns>
        CursorPos GetCurrentCursorPosition();

        /// <summary>
        /// Method to check if active document is in focus
        /// </summary>
        /// <returns></returns>
        bool IsActiveDocumentFocussed();

        /// <summary>
        /// Method to play error tone if there is an error.
        /// </summary>
        void PlaySoundIfError();

    }
}
