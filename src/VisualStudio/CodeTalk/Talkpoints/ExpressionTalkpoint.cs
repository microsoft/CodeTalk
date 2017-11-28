using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.Talkpoints
{
    class ExpressionTalkpoint : Talkpoint
    {
        string expression;

        public ExpressionTalkpoint(string filePath, CursorPos position, bool doesContinue, string expression) : base(filePath, position, doesContinue)
        {
            this.expression = expression;
        }

        public override void Execute()
        {
            if (string.IsNullOrEmpty(expression)) { return; }
            var exp = TalkCodePackage.vsOperations.RunExpressionInDebugger(expression);
            if (exp.IsValidValue)
            {
                TextToSpeech.SpeakText(exp.Value);
            }
        }
    }
}
