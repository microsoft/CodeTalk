//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

namespace Microsoft.CodeTalk.UI
{
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using static Microsoft.CodeTalk.Constants;

    /// <summary>
    /// Interaction logic for TalkpointToolWindowControl.
    /// </summary>
    public partial class TalkpointToolWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TalkpointToolWindowControl"/> class.
        /// </summary>
        public TalkpointToolWindowControl()
        {
            this.InitializeComponent();

            TalkpointTypeCombobox.ItemsSource = Enum.GetValues(typeof(TalkpointType)).Cast<TalkpointType>();
            TalkpointTypeCombobox.SelectedIndex = 0;

			// get all wav files
			var defaultTones = Enum.GetValues(typeof(Tones)).Cast<Tones>().Select(t => (object)t).ToList();

			defaultTones.AddRange(GetAllCustomTones());

			ToneSelectComboBox.ItemsSource = defaultTones; //Enum.GetValues(typeof(Tones)).Cast<Tones>();
            
        }

		private List<CustomTone> GetAllCustomTones()
		{
			DirectoryInfo dInfo = new DirectoryInfo(TalkCodePackage.TonesPath);
			var files = dInfo.GetFiles("*.wav");
			return files.Select(f => new CustomTone(f)).ToList();
		}

        void SetTonePanelVisible()
        {
            ToneSelectGrid.Visibility = Visibility.Visible;
            StatementGrid.Visibility = Visibility.Hidden;
        }

        void SetStatementPanelVisible()
        {
            StatementGrid.Visibility = Visibility.Visible;
            ToneSelectGrid.Visibility = Visibility.Hidden;
        }

        private void ToneSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox)) { return; }
            if ((sender as ComboBox).SelectedIndex < 0) { return; }
			if ((sender as ComboBox).SelectedItem is Tones tone)
			{
				VSOperations.PlaySound(tone);
			}
			else
			{
				VSOperations.PlaySound((sender as ComboBox).SelectedItem as CustomTone);
			}
		}

        private void TalkpointTypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!(sender is ComboBox)) { return; }
            if((sender as ComboBox).SelectedIndex < 0) { return; }
            var type = (TalkpointType)(sender as ComboBox).SelectedItem;
            switch (type)
            {
                case TalkpointType.Tonal:
                default:
                    SetTonePanelVisible();
                    break;
                case TalkpointType.Textual:
                case TalkpointType.Expression:
                    SetStatementPanelVisible();
                    break;

            }
        }

        public void ClearAll()
        {
            TalkpointTypeCombobox.SelectedIndex = 0;
            ToneSelectComboBox.SelectedIndex = -1;
            StatementTextBox.Text = "";
            ContinueCheckBox.IsChecked = false;
        }

        private void CreateTalkpointButton_Click(object sender, RoutedEventArgs e)
        {
            //On create talkpoint click
            var type = (TalkpointType)TalkpointTypeCombobox.SelectedItem;
            var doesContinue = ContinueCheckBox.IsChecked ?? false;
            switch (type)
            {
                case TalkpointType.Tonal:
                    if(ToneSelectComboBox.SelectedIndex < 0)
                    {
                        //report error
                        CloseFrame();
                        return;
                    }
					if(ToneSelectComboBox.SelectedItem is Tones tone)
					{
						TalkCodePackage.vsOperations.AddTonalTalkPointToCurrentLine(tone, doesContinue);
					}
					else
					{
						TalkCodePackage.vsOperations.AddTonalTalkPointToCurrentLine(ToneSelectComboBox.SelectedItem as CustomTone, doesContinue);
					}
                    
                    break;
                case TalkpointType.Textual:
                    if (string.IsNullOrEmpty(StatementTextBox.Text))
                    {
						//report error
						CloseFrame();
                        return;
                    }
                    var textualStatment = StatementTextBox.Text;
                    TalkCodePackage.vsOperations.AddTextualTalkpointToCurrentLine(textualStatment, doesContinue);
                    break;
                case TalkpointType.Expression:
                    if (string.IsNullOrEmpty(StatementTextBox.Text))
                    {
						//report error
						CloseFrame();
                        return;
                    }
                    var expressionStatment = StatementTextBox.Text;
                    TalkCodePackage.vsOperations.AddExpressionTalkpointToCurrentLine(expressionStatment, doesContinue);
                    break;
            }
			CloseFrame();
        }


        public static void CloseFrame()
        {
            ToolWindowPane talkpointWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(TalkpointToolWindow), 0, true);

            if ((null == talkpointWindow) || (null == talkpointWindow.Frame))
            {
				return;
            }
			var control = (TalkpointToolWindowControl)talkpointWindow.Content;
			if(null != control)
			{
				control.ClearAll();
			}
            IVsWindowFrame windowFrame = (IVsWindowFrame)talkpointWindow.Frame;
            windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
			ClearAll();
			CloseFrame();
        }
    }
}