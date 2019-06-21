//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

namespace Microsoft.CodeTalk
{
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AboutControl.
    /// </summary>
    public partial class AboutControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutControl"/> class.
        /// </summary>
        public AboutControl()
        {
            this.InitializeComponent();
        }

        private void HackLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("https://aka.ms/codetalk");
        }

        private void MailLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        public static void CloseFrame()
        {
            ToolWindowPane listFunctionsWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(About), 0, true);

            if ((null == listFunctionsWindow) || (null == listFunctionsWindow.Frame))
            {
                return;
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)listFunctionsWindow.Frame;
            windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }

        private void PrivacyButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://go.microsoft.com/fwlink/?LinkId=521839");
        }
    }
}