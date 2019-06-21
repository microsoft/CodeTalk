//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

namespace Microsoft.CodeTalk
{
    using Microsoft.CodeTalk.Commands;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using static Microsoft.CodeTalk.Commands.CommandConstants;

    /// <summary>
    /// Interaction logic for CodeTalkOptionsControl.
    /// </summary>
    public partial class CodeTalkOptionsControl : System.Windows.Controls.UserControl
    {

        List<string> CodeTalkKeys;
        List<string> CommandKeys;
        List<string> ModifierKeys;


        /// <summary>
        /// Initializes a new instance of the <see cref="CodeTalkOptionsControl"/> class.
        /// </summary>
        public CodeTalkOptionsControl()
        {
            this.InitializeComponent();

            this.UIInitialize();
        }

        private void UIInitialize()
        {
            //Populating combo box
            CodeTalkKeys = CommandConstants.SupportedCodeTalkKeys.Select(k => CommandConstants.KeyNamesMap[k]).ToList();
            CommandKeys = CommandConstants.SupportedCommandKeys.Select(k => CommandConstants.KeyNamesMap[k]).ToList();
            ModifierKeys = System.Enum.GetNames(typeof(CommandConstants.ModifierKeys)).ToList();

            //Code Talk Keys
            CodeTalkKeyComboBox.ItemsSource = CodeTalkKeys;
            CodeTalkModifierKeyComboBox.ItemsSource = ModifierKeys;
            var ctKey = CommandConstants.KeyNamesMap[TalkCodePackage.currentCodeTalkConfig.CodeTalkKey];
            CodeTalkKeyComboBox.SelectedIndex = CodeTalkKeys.IndexOf(ctKey);
            CodeTalkModifierKeyComboBox.SelectedIndex = ModifierKeys.IndexOf(TalkCodePackage.currentCodeTalkConfig.CodeTalkModifierKey.ToString());

            InitializeCombobox();
            

        }

        public void InitializeCombobox()
        {
            //Get Functions
            SetCommandCombobox(ref GetFunctionsKeyCombobox, ref GetFunctionsModifierKeyCombobox,
                TalkCodePackage.currentCodeTalkConfig.GetFunctionsCommandKeyConfig.CommandKey,
                TalkCodePackage.currentCodeTalkConfig.GetFunctionsCommandKeyConfig.CommandModifierKey);


            //Get Summary
            SetCommandCombobox(ref GetSummaryKeyCombobox, ref GetSummaryModifierKeyCombobox,
                TalkCodePackage.currentCodeTalkConfig.GetSummaryCommandKeyConfig.CommandKey,
                TalkCodePackage.currentCodeTalkConfig.GetSummaryCommandKeyConfig.CommandModifierKey);


            //Get Errors
            SetCommandCombobox(ref GetErrorsKeyCombobox, ref GetErrorsModifierKeyCombobox,
                TalkCodePackage.currentCodeTalkConfig.GetErrorsCommandKeyConfig.CommandKey,
                TalkCodePackage.currentCodeTalkConfig.GetErrorsCommandKeyConfig.CommandModifierKey);


            //Skip Comment
            SetCommandCombobox(ref SkipCommentKeyCombobox, ref SkipCommentModifierKeyCombobox,
                TalkCodePackage.currentCodeTalkConfig.SkipCommentCommandKeyConfig.CommandKey,
                TalkCodePackage.currentCodeTalkConfig.SkipCommentCommandKeyConfig.CommandModifierKey);


            //Get Context
            SetCommandCombobox(ref GetContextKeyCombobox, ref GetContextModifierKeyCombobox,
                TalkCodePackage.currentCodeTalkConfig.GetContextCommandKeyConfig.CommandKey,
                TalkCodePackage.currentCodeTalkConfig.GetContextCommandKeyConfig.CommandModifierKey);


            //Move To Context
            SetCommandCombobox(ref MoveToContextKeyCombobox, ref MoveToContextModifierKeyCombobox,
                TalkCodePackage.currentCodeTalkConfig.MoveToContextCommandKeyConfig.CommandKey,
                TalkCodePackage.currentCodeTalkConfig.MoveToContextCommandKeyConfig.CommandModifierKey);


            //Create Breakpoint
            SetCommandCombobox(ref CreateBreakpointKeyCombobox, ref CreateBreakpointModifierKeyCombobox,
                TalkCodePackage.currentCodeTalkConfig.CreateBreakpointCommandKeyConfig.CommandKey,
                TalkCodePackage.currentCodeTalkConfig.CreateBreakpointCommandKeyConfig.CommandModifierKey);


            //Set Profilepoints
            SetCommandCombobox(ref SetProfilepointsKeyCombobox, ref SetProfilepointsModifierKeyCombobox,
                TalkCodePackage.currentCodeTalkConfig.SetProfilepointsCommandKeyConfig.CommandKey,
                TalkCodePackage.currentCodeTalkConfig.SetProfilepointsCommandKeyConfig.CommandModifierKey);
        }

        public void SetCommandCombobox(ref System.Windows.Controls.ComboBox commandKeyCombobox, 
            ref System.Windows.Controls.ComboBox commandModifierKeyCombobox, 
            Keys commandKey, ModifierKeys commandModifierKey)
        {
            commandKeyCombobox.ItemsSource = CommandKeys;
            commandModifierKeyCombobox.ItemsSource = ModifierKeys;
            var ctKey = CommandConstants.KeyNamesMap[commandKey];
            commandKeyCombobox.SelectedIndex = CommandKeys.IndexOf(ctKey);
            commandModifierKeyCombobox.SelectedIndex =
                ModifierKeys.IndexOf(commandModifierKey.ToString());
        }
        
        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseFrame();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var config = new CodeTalkConfig();

            config.CodeTalkKey = CommandConstants.KeyNamesReverseMap[CodeTalkKeyComboBox.SelectedItem.ToString()];
            config.CodeTalkModifierKey = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), CodeTalkModifierKeyComboBox.SelectedItem.ToString());

            SetCommandInConfig(ref config);

            TalkCodePackage.SaveCodeTalkConfig(config);

            CloseFrame();
        }

        public void SetCommandInConfig(ref CodeTalkConfig config)
        {
            config.GetFunctionsCommandKeyConfig = new CommandKeyConfig(GetFunctionsKeyCombobox.SelectedItem.ToString(),
                GetFunctionsModifierKeyCombobox.SelectedItem.ToString());

            config.GetSummaryCommandKeyConfig = new CommandKeyConfig(GetSummaryKeyCombobox.SelectedItem.ToString(),
                GetSummaryModifierKeyCombobox.SelectedItem.ToString());

            config.GetErrorsCommandKeyConfig = new CommandKeyConfig(GetErrorsKeyCombobox.SelectedItem.ToString(),
                GetErrorsModifierKeyCombobox.SelectedItem.ToString());

            config.SkipCommentCommandKeyConfig = new CommandKeyConfig(SkipCommentKeyCombobox.SelectedItem.ToString(),
                SkipCommentModifierKeyCombobox.SelectedItem.ToString());

            config.GetContextCommandKeyConfig = new CommandKeyConfig(GetContextKeyCombobox.SelectedItem.ToString(),
                GetContextModifierKeyCombobox.SelectedItem.ToString());

            config.MoveToContextCommandKeyConfig = new CommandKeyConfig(MoveToContextKeyCombobox.SelectedItem.ToString(),
                MoveToContextModifierKeyCombobox.SelectedItem.ToString());

            config.CreateBreakpointCommandKeyConfig = new CommandKeyConfig(CreateBreakpointKeyCombobox.SelectedItem.ToString(),
                CreateBreakpointModifierKeyCombobox.SelectedItem.ToString());

            config.SetProfilepointsCommandKeyConfig = new CommandKeyConfig(SetProfilepointsKeyCombobox.SelectedItem.ToString(),
                SetProfilepointsModifierKeyCombobox.SelectedItem.ToString());
        }

        public static void CloseFrame()
        {
            ToolWindowPane talkpointWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(CodeTalkOptions), 0, true);

            if ((null == talkpointWindow) || (null == talkpointWindow.Frame))
            {
                return;
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)talkpointWindow.Frame;
            windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }
    }
}