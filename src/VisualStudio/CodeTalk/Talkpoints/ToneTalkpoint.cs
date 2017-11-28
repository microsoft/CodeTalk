using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeTalk.Constants;

namespace Microsoft.CodeTalk.Talkpoints
{
    public class ToneTalkpoint : Talkpoint
    {
        Tones talkpointTone;
        bool isCustomTone;
        CustomTone customTalkpointTone; 

        public ToneTalkpoint(string filePath, CursorPos position, bool doesContinue, Tones tone) : base(filePath, position, doesContinue)
        {
            this.talkpointTone = tone;
            this.isCustomTone = false;
        }

        public ToneTalkpoint(string filePath, CursorPos position, bool doesContinue, CustomTone customTone) : base(filePath, position, doesContinue)
        {
            this.customTalkpointTone = customTone;
            this.isCustomTone = true;
        }

        public override void Execute()
        {
            if (isCustomTone)
            {
                if (null != customTalkpointTone)
                {
                    VSOperations.PlaySound(customTalkpointTone);
                }
            }
            else
            {
                VSOperations.PlaySound(talkpointTone);
            }
        }
    }
}
