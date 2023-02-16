using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ComponentOneCloser
{
    public class Closer
    {
        // Left mouse button down and up codes
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;

        // closerTimer
        static System.Windows.Forms.Timer closerTimer = new System.Windows.Forms.Timer();

        private static ulong _iClosedTotal = 0;
        public static ulong iClosedTotal
        {
            get { return _iClosedTotal; }
            set
            {
                _iClosedTotal = value;

                //AboutBox.AboutBoxRefreshCounts();
                // TODO: Trigger about box update besides via the 1 second timer.
            }
        }

        // Counter for how many nag windows we have closed since last restart
        private static ulong _iClosed = 0;
        public static ulong iClosed
        {
            get { return _iClosed; }
            set
            {
                _iClosed = value;

                //AboutBox.AboutBoxRefreshCounts();
                // TODO: Trigger about box update besides via the 1 second timer.
            }
        }

        #region "Windows API Function DLL Imports"

        //[DllImport("USER32.DLL")]
        //public static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);

        // Get a handle to an application window.
        //[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        //public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("USER32.DLL")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        // Activate an application window.
        //[DllImport("USER32.DLL")]
        //public static extern bool SetForegroundWindow(IntPtr hWnd);

        // Start of changes I am making to help grab the window class, or even do it automatically - 11/17/17
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("USER32.DLL")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        #endregion

        /// <summary>
        /// Looks for processes that Component One will show up as, and then attempts to close them. This gets run on a timer
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        public static void FindAndCloseTimer(Object myObject, EventArgs myEventArgs)
        {
            Process[] ddiProcs = Process.GetProcessesByName("DDI inForm.vshost");
            Process[] lcProcs = Process.GetProcessesByName("lc");
            int i = 0;

            //Debug.WriteLine("FindAndCloseTimer triggered...");

            // Try and remove as much duplication as possible
            // Clean this up more, anyway to find the class easier by looking for all BUTTON's as the class name?

            for (i = 0; i < ddiProcs.Length; i++)
            {
                IntPtr handleDDI = ddiProcs[i].MainWindowHandle;

                Debug.WriteLine(ddiProcs[i].ToString());

                if (handleDDI == IntPtr.Zero)
                {
                    Debug.WriteLine("DDI Main window handle specified is Zero");
                }
                else
                {
                    Debug.WriteLine("DDI APP Handle Found!");

                    DisplayChildList(handleDDI);
                    FindButtonsToClose(handleDDI);
                }
            }

            for (i = 0; i < lcProcs.Length; i++)
            {
                IntPtr handleLc = lcProcs[i].MainWindowHandle;

                Debug.WriteLine(lcProcs[i].ToString());

                if (handleLc == IntPtr.Zero)
                {
                    Debug.WriteLine("LC Main window handle specified is Zero");
                }
                else
                {
                    Debug.WriteLine("LC APP Handle Found!");

                    DisplayChildList(handleLc);
                    FindButtonsToClose(handleLc);
                }
            }
            //Debug.WriteLine("Loop finished");
        }
        
        /// <summary>
        /// Takes the windowHandle of the application searched for, and tries to send messages to the button to close it based on its handle
        /// </summary>
        /// <param name="windowHandle"></param>
        public static void FindButtonsToClose(IntPtr windowHandle)
        {
            IntPtr button = IntPtr.Zero;

            // TODO: Eventually migrate this to a configuration file or a structure/linked list we can parse through and call FindWindowEx() against each item in the list

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.1ca0192_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #1");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.1a0e24_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #2");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.bf7d44_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #3");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.3ee13a2_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #4");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.3c47a4f_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #5");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.f90a95_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #6");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.277abd1_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #7");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.389238_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #8");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.3002ce0_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #9");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.2aaaa52_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #10");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.2bce4cc_r51_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #11");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.34f5582_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #12");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.259f9d2_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #13");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.3fbb6c5_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #14");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.259f9d2_r10_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #15");
                ClickOK(button);
            }

            button = FindWindowEx(windowHandle, IntPtr.Zero, "WindowsForms10.BUTTON.app.0.12ab327_r9_ad1", "OK");
            if (button != IntPtr.Zero)
            {
                Debug.WriteLine("Button Found! #16");
                ClickOK(button);
            }
        }

        /// <summary>
        /// Closes the Component One window by clicking OK programatticaly on each of the 10 buttons
        /// </summary>
        /// <param name="okButton"></param>
        public static void ClickOK(IntPtr okButton) //, IntPtr windowHandle
        {
            // No need to click twice or SetForegroundWindow() first. works with just a click down/up
            // SetForegroundWindow(windowHandle);

            // It was suggested to do this twice as the first run makes it take focus, and the second run clicks the button but I haven't found that necessary.
            //SendMessage(okButton, WM_LBUTTONDOWN, 0, 0);
            //SendMessage(okButton, WM_LBUTTONUP, 0, 0);

            SendMessage(okButton, WM_LBUTTONDOWN, 0, 0);
            SendMessage(okButton, WM_LBUTTONUP, 0, 0);

            // Increment both closed nag window counters (the one for just this run and the lifetime
            iClosed++;
            iClosedTotal++;
            
            // Save the totalNagsClosed count to the application settings file
            Properties.Settings.Default["totalNagsClosed"] = iClosedTotal;
            Properties.Settings.Default.Save(); // Saves settings in application configuration file
        }

        /// <summary>
        /// Adds a timer that gets used to update the Closed window counts inside of the AboutBox
        /// </summary>
        /// <param name="ms"></param>
        public static void AddCloserTimer(int ms)
        {
            closerTimer.Tick += new EventHandler(FindAndCloseTimer);
            closerTimer.Interval = ms;
            closerTimer.Start();
        }

        public static void DisplayChildList(IntPtr hwndParent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowsProc childProc = new EnumWindowsProc(EnumWindow);
                EnumChildWindows(hwndParent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if(listHandle.IsAllocated) listHandle.Free();
            }
            Debug.WriteLine("handle count: " + result.Count.ToString());
            //return result;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null) throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            list.Add(handle);
            return true;
        }
    }
}
