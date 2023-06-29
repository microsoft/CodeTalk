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
            var expResult = TalkCodePackage.vsOperations.RunExpressionInDebugger(expression);
            if (!string.IsNullOrEmpty(expResult))
            {
                TextToSpeech.SpeakText(expResult);
            }
        }
    }
}
