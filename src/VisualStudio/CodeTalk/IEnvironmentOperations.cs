using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeTalk.Constants;

namespace Microsoft.CodeTalk
{
	public interface IEnvironmentOperations
	{
		string RunExpressionInDebugger(string expression);

		void AddTonalTalkPointToCurrentLine(Tones tone, bool doesContinue);

		void AddTonalTalkPointToCurrentLine(CustomTone customTone, bool doesContinue);

		void AddTextualTalkpointToCurrentLine(string statement, bool doesContinue);

		void AddExpressionTalkpointToCurrentLine(string expression, bool doesContinue);

		bool RemoveBreakpointInCurrentPosition();

		string GetActiveDocumentPath();

		string GetActiveDocumentCode();

		void GoToLocationInActiveDocument(int lineNumber, int columnNumber = 0);

		int GetCursorLineNumber();

		CursorPos GetCurrentCursorPosition();

		bool IsActiveDocumentFocussed();

		void PlaySoundIfError();

	}
}
