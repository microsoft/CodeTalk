//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

namespace Microsoft.CodeTalk.UI
{
    using Microsoft.CodeTalk.LanguageService;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for GetSummaryToolWindowControl.
    /// </summary>
    public partial class GetSummaryToolWindowControl : UserControl
	{
		//Tree Items binded to the WPF
		public ObservableCollection<MenuItemViewModel> TreeItems { get; }
		MenuItemViewModel treeRootViewModel = null;

		FunctionTypes functionTypesToDisplay = FunctionTypes.MemberFunction |
												FunctionTypes.Constructor |
												FunctionTypes.Destructor |
												FunctionTypes.External |
												FunctionTypes.GlobalFunction |
												FunctionTypes.Operator |
												FunctionTypes.ConversionOperator;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetSummaryToolWindowControl"/> class.
		/// </summary>
		public GetSummaryToolWindowControl()
		{
			this.InitializeComponent();
			TreeItems = new ObservableCollection<MenuItemViewModel>();
			treeView.ItemsSource = TreeItems;
		}


		/// <summary>
		/// Builds the tree for MenuItem tree view model
		/// </summary>
		/// <param name="node"></param>
		/// <param name="menuItemViewModel"></param>
		void BuildTreeViewModel(ISyntaxEntity node, MenuItemViewModel menuItemViewModel)
		{
			MenuItemViewModel item = new MenuItemViewModel()
			{
				DisplayText = node.DisplayText(),
				SpokenText = node.SpokenText(),
				Name = node.DisplayText(),
				LineNumber = node.Location.StartLineNumber + CodeTalk.Constants.GoToLineOffsite,
				Children = new ObservableCollection<MenuItemViewModel>(),
				IsExpanded = (node is Block || node is FunctionDefinition) ? false : true,
				ImageSource = UIConstants.GetImageSourceFromKind(node)
			};
			if (null == menuItemViewModel)
			{
				treeRootViewModel = item;
			}
			else
			{
				menuItemViewModel.Children.Add(item);
			}

			menuItemViewModel = item;

			if (null == node.Children)
				return;

			foreach (var child in node.Children)
			{
				if (child.Kind == SyntaxEntityKind.Function)
				{
					if (((child as FunctionDefinition).TypeOfFunction & functionTypesToDisplay) == 0)
					{
						continue;
					}
				}
				BuildTreeViewModel(child, menuItemViewModel);
			}
		}

		public void SetTreeView(ISyntaxEntity node, string label)
		{
			//Cleaning UI
			TreeItems.Clear();

			treeRootViewModel = null;

			//Modifed by prvai
			//buildTree(node, null);
			BuildTreeViewModel(node, null);

			//modified by prvai
			TreeItems.Add(treeRootViewModel);

			treeView.Focus();

			if (treeView.HasItems)
			{
				var selectedItem = treeView.Items[0] as MenuItemViewModel;
				selectedItem.IsSelected = true;
				//TreeViewItem item = treeView.ItemContainerGenerator.Items[0] as TreeViewItem;
				//item.IsSelected = true;
			}

		}

		public static void CloseFrame()
		{
			ToolWindowPane listFunctionsWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(GetSummaryToolWindow), 0, true);

			if ((null == listFunctionsWindow) || (null == listFunctionsWindow.Frame))
			{
				return;
			}

			IVsWindowFrame windowFrame = (IVsWindowFrame)listFunctionsWindow.Frame;
			windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
		}

		public void GoToEntity()
		{
			MoveCursorToSelectedFunction();

			CloseFrame();
		}

		void MoveCursorToSelectedFunction()
		{
			MenuItemViewModel destinationFunction = (MenuItemViewModel)treeView.SelectedItem;
			TalkCodePackage.vsOperations.GoToLocationInActiveDocument(destinationFunction.LineNumber);
		}

		private void FileSummary_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				GoToEntity();
			}
			else if (e.Key == Key.Escape)
			{
				Window.GetWindow(this).Close();
			}
		}

		private void treeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			GoToEntity();
		}
	}
}