using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.Talkpoints
{
    class TextTalkpoint : Talkpoint
    {
        string statement;

        public TextTalkpoint(string filePath, CursorPos position, bool doesContinue, string statement) : base(filePath, position, doesContinue)
        {
            this.statement = statement;
        }

        public override void Execute()
        {
            if (string.IsNullOrEmpty(statement)) { return; }
            TextToSpeech.SpeakText(statement);
        }
    }
}
