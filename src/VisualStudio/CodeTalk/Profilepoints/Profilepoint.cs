using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeTalk.Constants;

namespace Microsoft.CodeTalk.Profilepoints
{
    public class Profilepoint : Talkpoint
    {
        FunctionLevelDetailsHandler functionLevelDetailsHandler;
        readonly int id;

        public Profilepoint(string filePath, CursorPos position, bool doesContinue, int lineNumber, FunctionLevelDetailsHandler TalkCodePackageBreakpointHandler) : base(filePath, position, doesContinue)
        {
            this.functionLevelDetailsHandler = TalkCodePackageBreakpointHandler;
            this.id = this.functionLevelDetailsHandler.AddNewProfilePoint(lineNumber);
        }

        public override void Execute()

        {
            long timesec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.functionLevelDetailsHandler.AddEnd(id, timesec);
        }
    }
}


