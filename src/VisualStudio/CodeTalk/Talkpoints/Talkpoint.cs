//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

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

    public abstract class Talkpoint
    {
        internal string filePath;
        internal CursorPos position;
        internal bool doesContinue;

        public Talkpoint(string filePath, CursorPos position, bool doesContinue = false)
        {
            this.filePath = filePath;
            this.position = position;
            this.doesContinue = doesContinue;
        }

        public abstract void Execute();

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
