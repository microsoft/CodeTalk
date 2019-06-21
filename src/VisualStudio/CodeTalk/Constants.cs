//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System.ComponentModel;
using System.IO;

namespace Microsoft.CodeTalk
{
    public class Constants
    {
        public static int GoToLineOffsite = 0;

        public static int CommandModeWaitTimeMilliseconds = 2000;

        public static int ErrorDetectWaitTimeMilliseconds = 2000;

        public static bool AutomaticErrorDetectEnabled = true;

        public static string RequestFunctionDetails = "RequestFunctionDetails";

        public static string Pause = "Pause";

        public static string Resume = "Resume";

        public enum Tones
        {
            [Description("Error Tone 1")]
            error1,
            [Description("Error Tone 2")]
            error2
        }

		public class CustomTone
		{
			FileInfo toneInfo;

			public CustomTone(FileInfo toneInfo)
			{
				this.toneInfo = toneInfo;
			}

			public override string ToString()
			{
				return toneInfo.Name;
			}

			public string GetTonePath()
			{
				return toneInfo.FullName;
			}
		}
    }
}
