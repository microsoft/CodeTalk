//------------------------------------------------------------------------------
// <copyright file="TalkCodePackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System.Windows.Automation;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Windows.Forms;
using Microsoft.CodeTalk.Commands;
using Microsoft.CodeTalk.Profilepoints;

namespace Microsoft.CodeTalk
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(TalkCodePackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideToolWindow(typeof(AccessibilityToolWindow), Window = Microsoft.VisualStudio.Shell.Interop.ToolWindowGuids.Outputwindow, Orientation = ToolWindowOrientation.Left, PositionX = 300, PositionY = 200, Width = 500, Height = 300, Style = VsDockStyle.AlwaysFloat)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.SolutionExists)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.CodeWindow)]
    [ProvideToolWindow(typeof(CodeTalk.UI.TalkpointToolWindow), Style = VsDockStyle.AlwaysFloat)]
    [ProvideToolWindow(typeof(About))]
    [ProvideToolWindow(typeof(CodeTalkOptions))]
    [ProvideToolWindow(typeof(CodeTalk.UI.GetSummaryToolWindow))]
    public sealed class TalkCodePackage : Package
    {
        /// <summary>
        /// TalkCodePackage GUID string.
        /// </summary>
        public const string PackageGuidString = "8fb8ab31-525c-4764-a1dc-e13d56a498d5";

        /// <summary>
        /// Keyboard manager for executing commands.
        /// </summary>
        internal static KeyboardManager keyboardManager;

        /// <summary>
        /// WebSocket Server for setting up data flow channels with Visual Studio and Web Views.
        /// </summary>
        /// 
        public CodeTalkWebSocketServer codeTalkWebSocketServer;

        ///summary>
        /// Breakpoint Handler for Profile Points
        /// </summary>
        static FunctionLevelDetailsHandler TalkCodePackageBreakpointHandler;

        /// <summary>
        /// Current process Id. Used for focus monitoring.
        /// </summary>
        private static int CurrentProcessId;

        /// <summary>
        /// Current Package visible to all the classes
        /// </summary>
        public static Package currentPackage;

        /// <summary>
        /// Current config for CodeTalk
        /// </summary>
        public static CodeTalkConfig currentCodeTalkConfig;

        /// <summary>
        /// VSOperations instance
        /// </summary>
        public static VSOperations vsOperations;

        /// <summary>
        /// Visual studio shutdown handler
        /// </summary>
        public static _dispDTEEvents_OnBeginShutdownEventHandler ShutdownHandler;

        //Store folder Location
        private static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string CodeTalkFileStorePath = Path.Combine(AppDataPath, "CodeTalk");

        public static string TonesPath = Path.Combine(CodeTalkFileStorePath, "Tones");

        public static string CodeTalkConfigPath = Path.Combine(CodeTalkFileStorePath, "CodeTalk.config");

        /// <summary>
        /// The below three objects are used to find visual studio shutdown
        /// </summary>
        private DTE2 m_applicationObject = null;
        DTEEvents m_packageDTEEvents = null;
        public DTE2 ApplicationObject
        {
            get
            {
                if (m_applicationObject == null)
                {
                    // Get an instance of the currently running Visual Studio IDE
                    DTE dte = (DTE)GetService(typeof(DTE));
                    m_applicationObject = dte as DTE2;
                }
                return m_applicationObject;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ListFunctionsCommand"/> class.
        /// </summary>
        public TalkCodePackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            //Initializing File Store
            if (!Directory.Exists(CodeTalkFileStorePath))
            {
                Directory.CreateDirectory(CodeTalkFileStorePath);
            }
            if (!Directory.Exists(TonesPath))
            {
                Directory.CreateDirectory(TonesPath);
            }

            //Loading CodeTalk config
            LoadCodeTalkConfig();

            //Adding Focus changed event
            CurrentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChangedHandler);

            //Keyboard Hook
            if (null != keyboardManager) { keyboardManager.Dispose(); }
            keyboardManager = new KeyboardManager();
            keyboardManager.AddKeyboardHook();


            //Visual Studio close event
            m_packageDTEEvents = ApplicationObject.Events.DTEEvents;
            ShutdownHandler = new _dispDTEEvents_OnBeginShutdownEventHandler(HandleVisualStudioShutdown);
            m_packageDTEEvents.OnBeginShutdown += ShutdownHandler;

            base.Initialize();
            FunctionsListToolWindowCommand.Initialize(this);
            CodeTalk.UI.TalkpointToolWindowCommand.Initialize(this);

            currentPackage = this;

            vsOperations = new VSOperations();

            codeTalkWebSocketServer = new CodeTalkWebSocketServer();

            TalkCodePackageBreakpointHandler = new FunctionLevelDetailsHandler(codeTalkWebSocketServer);
            vsOperations.SetBreakpointHandler(TalkCodePackageBreakpointHandler);

            SetProfilepointsCommand.vsOperations = vsOperations;
            TextToSpeech.IsTextToSpeechEnabled = true;
            AboutCommand.Initialize(this);
            CodeTalkOptionsCommand.Initialize(this);
            CodeTalk.UI.GetSummaryToolWindowCommand.Initialize(this);
        }

        public void LoadCodeTalkConfig()
        {
            if (File.Exists(CodeTalkConfigPath))
            {
                try
                {
                    currentCodeTalkConfig = CodeTalkConfig.LoadFromXml(CodeTalkConfigPath);
                }
                catch (Exception ex)    //We have to catch the exception here, or the IDE can crash
				{
                    MessageBox.Show($"Exception '{ex.Message}' occurred while opening file {CodeTalkConfigPath}. Using defaults.", "Config load failed", MessageBoxButtons.OK);
                    LoadDefaultCodeTalkConfig();
                }
            }
            else
            {
				SaveDefaultConfig();
                LoadDefaultCodeTalkConfig();
            }
        }

        public static void SaveCodeTalkConfig(CodeTalkConfig config)
        {
            try
            {
                config.SaveAsXml(CodeTalkConfigPath);
                TalkCodePackage.currentCodeTalkConfig = config;
            }
            catch ( Exception ex)   //We have to catch the exception here, or the IDE can crash
			{
                MessageBox.Show($"General exception '{ex.Message}' occurred while saving configuration to file {CodeTalkConfigPath}. Your customizations might be lost.",
                                "Config save failed",
                                MessageBoxButtons.OK);

            }


        }

        void LoadDefaultCodeTalkConfig()
        {
			currentCodeTalkConfig = CommandConstants.DefualtCodeTalkConfig;//CodeTalkConfig.LoadFromXml(@".\DefaultCodeTalkConfig.config");
			currentCodeTalkConfig.ProcessAndValidate();
        }

		void SaveDefaultConfig()
		{
			SaveCodeTalkConfig(CommandConstants.DefualtCodeTalkConfig);
		}

        #endregion

        /// <summary>
        /// Called when visual studio is closed.
        /// </summary>
        public void HandleVisualStudioShutdown()
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    Automation.RemoveAutomationFocusChangedEventHandler(OnFocusChangedHandler);
                });

                m_packageDTEEvents.OnBeginShutdown -= ShutdownHandler;
                ShutdownHandler = null;
                currentPackage = null;
            }
            catch (Exception expk)  //We have to catch the exception here, or the IDE can crash
			{
                Debug.WriteLine(expk.StackTrace);
            }
            try { keyboardManager.Dispose(); } catch (Exception expk) { Debug.WriteLine(expk); }    //We have to catch the exception here, or the IDE can crash
			try { vsOperations.Dispose(); } catch (Exception expv) { Debug.WriteLine(expv); }   //We have to catch the exception here, or the IDE can crash

		}

        /// <summary>
        /// Called when focus of any window is changed. Will be used to hook and unhook the Keyboard events.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="args"></param>
        public static void OnFocusChangedHandler(object src, AutomationFocusChangedEventArgs args)
        {
            AutomationElement automationElement = src as AutomationElement;
            if (null != automationElement)
            {
                try
                {
                    var currentElement = automationElement.Current;
                    var pID = currentElement.ProcessId;
                    if (pID == CurrentProcessId)
                    {
                        //Activate
                        KeyboardManager.IsWindowInFocus = true;
                        System.Diagnostics.Debug.WriteLine("Active");
                        //KeyboardManager.Hook();
                    }
                    else
                    {
                        //Deactivate
                        KeyboardManager.IsWindowInFocus = false;
                        System.Diagnostics.Debug.WriteLine("Deactive");
                        //KeyboardManager.UnHook();
                    }
                }
                catch (Exception e) //We have to catch the exception here, or the IDE can crash
				{
                    if (e.HResult == -2147220991)
                    {
                        //let it go
                    }
                    else
                    {
                        //Do nothing for now
                    }
                }
            }
        }
    }
}
