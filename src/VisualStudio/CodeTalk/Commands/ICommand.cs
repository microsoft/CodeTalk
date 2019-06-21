//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------


namespace Microsoft.CodeTalk.Commands
{
    interface ICommand
    {
        CommandKeyConfig Keys { get; }

        void Execute();

        bool PassControl();
    }
}
