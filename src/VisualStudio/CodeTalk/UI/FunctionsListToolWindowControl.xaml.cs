//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

namespace Microsoft.CodeTalk
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.CodeTalk.LanguageService;
    using System.Collections.ObjectModel;
    using System;
    using System.ComponentModel;
    using Microsoft.CodeTalk.UI;


    // <summary>
    /// <summary>
    /// class containing data to populate the GUI
    /// GUI can be a ListView or TreeView
    /// </summary>
    /// </summary>
    public class MenuItemViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public MenuItemViewModel()
        {
        }
        public string Name { get; set; }
        public int LineNumber { get; set; }
        public string DisplayText { get; set; }
        public string ImageSource { get; set; }
        public string SpokenText { get; set; }

        private ObservableCollection<MenuItemViewModel> _children;

        public ObservableCollection<MenuItemViewModel> Children
        {
            get { return _children ?? (_children = new ObservableCollection<MenuItemViewModel>()); }
            set
            {
                _children = value;
                OnPropertyChanged("Children");
            }
        }


        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }

        public MenuItemViewModel(string displayText, int lineNumber)
        {
            this.Name = "node" + "_" + lineNumber;
            this.LineNumber = lineNumber;
            //this.Header = displayText;
            this.DisplayText = displayText;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddChild(MenuItemViewModel child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// override function for toString.
        /// can be used to provide screen readers proper text to announce when user focuses on list items in the listview.
        /// narrator makes inconsistent announcements (the class name) when the user focuses on the list item.
        /// override contains "function <functionName> at line <lineNumber>"
        /// so when screen reader gets focus, it reads the override string instead of just function name followed by the line number.
        /// </summary>
        /// <returns> the string to return </returns>
        public override string ToString()
        {
            return SpokenText; // + " at line " + LineNumber.ToString();
        }
    }

    /// <summary>
    /// Interaction logic for AccessibilityToolbarWindowControl.
    /// </summary>
    public partial class AccessibilityToolbarWindowControl : UserControl
    {
        bool isTreeViewActive = false;
        FunctionTypes functionTypesToDisplay = FunctionTypes.MemberFunction | 
                                                FunctionTypes.Constructor | 
                                                FunctionTypes.Destructor | 
                                                FunctionTypes.External | 
                                                FunctionTypes.GlobalFunction | 
                                                FunctionTypes.Operator | 
                                                FunctionTypes.ConversionOperator;


        //Tree Items binded to the WPF
        public ObservableCollection<MenuItemViewModel> TreeItems { get;  }
        public ObservableCollection<MenuItemViewModel> ListItems { get; }
        MenuItemViewModel treeRootViewModel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessibilityToolbarWindowControl"/> class.
        /// </summary>
        public AccessibilityToolbarWindowControl()
        {
            this.InitializeComponent();
            TreeItems = new ObservableCollection<MenuItemViewModel>();
            ListItems = new ObservableCollection<MenuItemViewModel>();
            listView.ItemsSource = ListItems;
            treeView.ItemsSource = TreeItems;
        }

        private void ResizeControl()
        {
            ToolWindowPane listFunctionsWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(AccessibilityToolWindow), 0, true);

            if ((null == listFunctionsWindow) || (null == listFunctionsWindow.Frame))
            {
                return;
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)listFunctionsWindow.Frame;

            Guid gui = Guid.Empty;
            int px, py, pcx, pcy;
            VSSETFRAMEPOS[] vssetArr = { VSSETFRAMEPOS.SFP_fFloat };
            windowFrame.GetFramePos(vssetArr, out gui, out px, out py, out pcx, out pcy);
            MainScrollViewer.Height = pcy;
            MainScrollViewer.Width = pcx;
        }

        public void SetListView(IEnumerable<ISyntaxEntity> entities, string label, bool focusOnFirst = true)
        {
            //Cleaning UI
            listView.Visibility = Visibility.Visible;
            treeView.Visibility = Visibility.Collapsed;
            ListItems.Clear();

            var listItems = entities.Select(entity =>
            {

                return new MenuItemViewModel() { DisplayText = entity.DisplayText(),
                    SpokenText = entity.SpokenText(),
                    Name = entity.DisplayText(),
                    LineNumber = entity.Location.StartLineNumber + Constants.GoToLineOffsite,
                    Children = new ObservableCollection<MenuItemViewModel>(),
                    ImageSource = UIConstants.GetImageSourceFromKind(entity) };
                //return new MenuItem(entity.CurrentCodeFile.Language.SpokenText(entity),
                //                                            entity.Location.StartLineNumber);
            });

            foreach(var item in listItems) { ListItems.Add(item); }

            listView.Focus();

            /// Focus explanation:
            /// The ListViewItems are not generated immediately, because of that we can obtain the ListViewItem to shift focus.
            /// Hence we call ListView.UpdateLayout() which pre-generates the ListViewItems.
            /// Since there are only few tens of items expected in the list view, this is acceptable.
            /// In case the list items will go to hundreds or thousands, this is not efficient way, we need to use the 
            /// status chaged event of the ItemContainerGenerator to get the ListViewItem.
            if (true == focusOnFirst)
            {
                listView.SelectedItem = listView.Items[0];
                listView.UpdateLayout();
                var item = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.Items[0]);
                if (null != item) { item.Focus(); }
            }
            else
            {
                listView.SelectedItem = listView.Items[listItems.Count() - 1];
                listView.SelectedIndex = listItems.Count() - 1;
                listView.UpdateLayout();
                var item = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.Items[listItems.Count() - 1]);
                if (null != item) { item.Focus(); }
            }


            isTreeViewActive = false;

            ResizeControl();
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
                LineNumber = node.Location.StartLineNumber + Constants.GoToLineOffsite,
                Children = new ObservableCollection<MenuItemViewModel>(),
                IsExpanded = (node is Block || node is FunctionDefinition)?false:true,
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
            listView.Visibility = Visibility.Collapsed;
            treeView.Visibility = Visibility.Visible;
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

            isTreeViewActive = true;

            ResizeControl();
        }

        ///<summary>
        ///this function moves cursor to the selected list item in the FunctionsListView.
        ///this is called both by listFunctionsGoButton and Window_KeyDown on pressing enter.
        ///</summary>
        void MoveCursorToSelectedFunction()
        {
            if (false == isTreeViewActive)
            {
                MenuItemViewModel destinationFunction = (MenuItemViewModel)listView.SelectedItem;
                TalkCodePackage.vsOperations.GoToLocationInActiveDocument(destinationFunction.LineNumber);
            }
            else
            {
                MenuItemViewModel destinationFunction = (MenuItemViewModel)treeView.SelectedItem;
                TalkCodePackage.vsOperations.GoToLocationInActiveDocument(destinationFunction.LineNumber);
            }
        }

        public void GoToEntity()
        {
            MoveCursorToSelectedFunction();
			//var window = Window.GetWindow(this);
			//window.Close();

			CloseFrame();
        }

		public static void CloseFrame()
		{
			ToolWindowPane listFunctionsWindow = TalkCodePackage.currentPackage.FindToolWindow(typeof(AccessibilityToolWindow), 0, true);

			if ((null == listFunctionsWindow) || (null == listFunctionsWindow.Frame))
			{
				return;
			}

			IVsWindowFrame windowFrame = (IVsWindowFrame)listFunctionsWindow.Frame;
			windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
		}

        private void ListItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GoToEntity();
        }

        ///<summary>This function handles KeyDown events on ListView.
        ///calls the moveCursorToSelectedFunction when the user presses enter.
        ///</summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
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
    }
}