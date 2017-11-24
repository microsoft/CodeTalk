//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeTalk.Constants;

namespace Microsoft.CodeTalk
{
    public class CursorPos
    {
        internal int lineNumber;
        internal int columnNumber;
        internal bool IsOperationFailed;

        public CursorPos() { }

        public CursorPos(int lineNumber, int columnNumber)
        {
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
        }

        public CursorPos(bool IsOperationFailed)
        {
            this.lineNumber = 0;
            this.columnNumber = 0;
            this.IsOperationFailed = IsOperationFailed;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CursorPos)) { return false; }
            var cp = obj as CursorPos;
            if (cp.lineNumber == this.lineNumber && cp.columnNumber == this.columnNumber)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (lineNumber << 16) | columnNumber;
        }

    }

    public enum TalkpointType
    {
        Tonal,
        Textual,
        Expression
    }

    public class Talkpoint
    {
        internal string filePath;
		internal CursorPos position;
		internal bool doesContinue;
		internal TalkpointType type;
		internal string statement;
		internal Tones tone;
		internal bool isCustomTone;
		internal CustomTone customTone;

        public Talkpoint(TalkpointType type, string filePath, CursorPos position, string statement = "", bool doesContinue = false, Tones tone = Tones.error1, bool isCustomTone = false, CustomTone customTonePath = null)
        {
            this.filePath = filePath;
            this.position = position;
            this.doesContinue = doesContinue;
            this.statement = statement;
            this.type = type;
			this.tone = tone;
			this.isCustomTone = isCustomTone;
			this.customTone = customTonePath;
        }

        public Talkpoint(Breakpoint breakpoint)
        {
            this.filePath = breakpoint.File;
            position = new CursorPos(breakpoint.FileLine, breakpoint.FileColumn);
            this.doesContinue = false;
            this.statement = string.Empty;
            this.type = TalkpointType.Tonal;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Talkpoint)) { return false; }
            var cb = obj as Talkpoint;
            //Currently we assume one breakpoint per line.
            if (cb.filePath.Equals(this.filePath) && cb.position.lineNumber == this.position.lineNumber)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (filePath + this.position.lineNumber.ToString()).GetHashCode();
        }
    }
}
