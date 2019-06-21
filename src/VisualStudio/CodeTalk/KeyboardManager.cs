//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using Microsoft.CodeTalk.Commands;
using Microsoft.CodeTalk.Profilepoints;
using Microsoft.CodeTalk.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Microsoft.CodeTalk
{
    /// <summary>
    /// Class for managing the Keyboard events
    /// </summary>
    class KeyboardManager : IDisposable
	{

		public static bool IsWindowInFocus;

		public static bool IsCommandMode;

		private const int WH_KEYBOARD_LL = 13;

		private const int WM_KEYDOWN = 0x0100;

		private LowLevelKeyboardProc _proc = HookCallback;

		private static IntPtr _hookID = IntPtr.Zero;

		private static System.Threading.Timer CommandModeTimer;

		private static System.Threading.Timer ErrorDetectTimer;

        static FunctionLevelDetailsHandler breakpointHandler;


		/// <summary>
		/// Add low level Keyboard Hook
		/// </summary>
		public void AddKeyboardHook()
		{
			UnHook();
			_hookID = SetHook(_proc);
		}

        public void AddBreakpointHandler(FunctionLevelDetailsHandler TalkCodePackageBreakpointHandler)
        {
            breakpointHandler = TalkCodePackageBreakpointHandler;
        }


		/// <summary>
		/// Unhook the low level keyboard events
		/// </summary>
		public static void UnHook()
		{
			if (_hookID == IntPtr.Zero) { return; }
            SafeNativeMethods.UnhookWindowsHookEx(_hookID);
		}



		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			{
				using (ProcessModule curModule = curProcess.MainModule)
				{
					return SafeNativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, proc, SafeNativeMethods.GetModuleHandle(curModule.ModuleName), 0);
				}
			}
		}


		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


		/// <summary>
		/// Low level keyboard hook callback
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			try
			{
				//If window is not in focus, do not do anything
				if (!IsWindowInFocus) { return SafeNativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam); }

				//If only Keydown
				if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
				{
					//Will give the key press for the event.
					int vkCode = Marshal.ReadInt32(lParam);
					var key = (Keys)vkCode;

					//If the key pressers are modifier keys, exit the callback
					if (key == Keys.Control || key == Keys.Shift
						|| key == Keys.CapsLock || key == Keys.Insert
						|| key == Keys.LControlKey || key == Keys.RControlKey
						|| key == Keys.LShiftKey || key == Keys.RShiftKey)
					{
						//Do nothing
						return SafeNativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
					}

					//If autonmatic error detection is enabled, reset timer
					if (Constants.AutomaticErrorDetectEnabled)
					{
						//Automatic Error detection timer
						if (null != ErrorDetectTimer) { ErrorDetectTimer.Dispose(); }
						//Create the timer
						ErrorDetectTimer = new System.Threading.Timer((s) =>
						{
							bool isActiveWindowFocussed = TalkCodePackage.vsOperations.IsActiveDocumentFocussed();
							if (!isActiveWindowFocussed) { return; }
						//Call the error detector
						System.Diagnostics.Debug.WriteLine("Detect Error");
							Task.Run(() =>
							{
								TalkCodePackage.vsOperations.PlaySoundIfError();
							});

						}, null, Constants.ErrorDetectWaitTimeMilliseconds, Timeout.Infinite);
					}

					//Check for command mode
					if (!IsCommandMode && key == TalkCodePackage.currentCodeTalkConfig.CodeTalkKey)
					{
						if (IsKeyPressed(TalkCodePackage.currentCodeTalkConfig.CodeTalkModifierKey))
						{
							//Control + ~ pressed : Enable command mode, which will exipre in few seconds
							//System.Diagnostics.Debug.WriteLine("Control + Tilde");
							IsCommandMode = true;
							if (null != CommandModeTimer) { CommandModeTimer.Dispose(); }

							//Expiration timer for command mode
							CommandModeTimer = new System.Threading.Timer((s) =>
							{
							//Disable the command mode
							IsCommandMode = false;

							}, null, Constants.CommandModeWaitTimeMilliseconds, Timeout.Infinite);
							//Don't pass the control
							return (IntPtr)1;
						}
						//else do nothing
						return SafeNativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
					}

					//If Escape, then close windows
					if (key == Keys.Escape && !IsCtrlPressed())
					{
						//Close all windows
						CloseAllCodeTalkFrames();
					}

					//Exit if not in command mode
					if (!IsCommandMode)
					{
						//Do nothing
						return SafeNativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
					}

					//Find the appropriate command and execute it
					try
					{
						//disable command mode.
						IsCommandMode = false;
						CommandModeTimer.Dispose();
						CommandModeTimer = null;

						var command = TalkCodePackage.currentCodeTalkConfig.GetCommands().Where(c => c.Keys.CommandKey == key).FirstOrDefault();

						if (null != command && IsKeyPressed(command.Keys.CommandModifierKey))
						{
							command.Execute();

							if (command.PassControl())
							{
								return SafeNativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
							}

							//Exit without passing control
							return (IntPtr)1;
						}
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine(e.StackTrace);
					}
				}
			}
			catch (Exception exp)
			{
				System.Diagnostics.Debug.WriteLine(exp.StackTrace);
			}
			//To pass it along
			return SafeNativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		private static void CloseAllCodeTalkFrames()
		{
			CodeTalkOptionsControl.CloseFrame();
			AccessibilityToolbarWindowControl.CloseFrame();
			AboutControl.CloseFrame();
			CodeTalkOptionsControl.CloseFrame();
			TalkpointToolWindowControl.CloseFrame();
			GetSummaryToolWindowControl.CloseFrame();
		}

        static class SafeNativeMethods
        {

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool UnhookWindowsHookEx(IntPtr hhk);


            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr GetModuleHandle(string lpModuleName);
        }

		#region KeyboardMethods
		private static bool IsKeyPressed(Key key)
		{
			return Keyboard.IsKeyDown(key);
		}

		private static bool IsKeyPressed(CommandConstants.ModifierKeys key)
		{
			switch (key)
			{
				case CommandConstants.ModifierKeys.Control:
					return IsCtrlPressed();
				case CommandConstants.ModifierKeys.Shift:
					return IsShiftPressed();
				case CommandConstants.ModifierKeys.Insert:
					return IsInsertPressed();
			}
			return false;
		}

		private static bool IsCtrlPressed()
		{
			return IsKeyPressed(Key.RightCtrl) || IsKeyPressed(Key.LeftCtrl);
		}

		private static bool IsShiftPressed()
		{
			return IsKeyPressed(Key.RightShift) || IsKeyPressed(Key.LeftShift);
		}

		private static bool IsAltPressed()
		{
			return IsKeyPressed(Key.RightAlt) || IsKeyPressed(Key.LeftAlt);
		}

		private static bool IsInsertPressed()
		{
			return IsKeyPressed(Key.Insert);
		}
		#endregion


		public void Dispose()
		{
			//Unhooking the keyboard callback
			UnHook();
			//Disposing the timers
			if (null != CommandModeTimer) { CommandModeTimer.Dispose(); }
			if (null != ErrorDetectTimer) { ErrorDetectTimer.Dispose(); }
		}

	}
}
