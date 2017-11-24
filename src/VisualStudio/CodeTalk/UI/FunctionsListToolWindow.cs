//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

namespace Microsoft.CodeTalk
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("f3b26d37-88e1-47b8-9eea-04e83684bc97")]
    public class AccessibilityToolWindow : ToolWindowPane
    {

        internal AccessibilityToolbarWindowControl windowControl;
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessibilityToolWindow"/> class.
        /// </summary>
        public AccessibilityToolWindow() : base(null)
        {
            this.Caption = "AccessibilityToolWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.windowControl = new AccessibilityToolbarWindowControl();
            this.Content = this.windowControl;
        }
    }
}
