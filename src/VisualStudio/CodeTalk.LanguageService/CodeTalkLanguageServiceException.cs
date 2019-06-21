//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;

namespace Microsoft.CodeTalk.LanguageService
{
    public class CodeTalkLanguageServiceException : Exception
	{
		private Exception _internalException;
		public Exception InternalException { get; }
		public CodeTalkLanguageServiceException() : base() { }
		public CodeTalkLanguageServiceException(string message) : base(message) { }
		public CodeTalkLanguageServiceException(Exception internalException)
		{
			this._internalException = internalException;
		}
	}
}
