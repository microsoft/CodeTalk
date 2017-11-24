//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.CodeTalk.Commands
{
    public abstract class CommandBase : ICommand
    {
        public CommandKeyConfig Keys { get; }
        public abstract void Execute();
        public abstract bool PassControl();

        protected CommandBase(CommandKeyConfig keys)
        {
            this.Keys = keys;
        }

    }
}
