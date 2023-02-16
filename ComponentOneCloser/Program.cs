using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

#region "TODO"
/********Prompt to add to startup on first run w/ a box not to ask again, or to remind later
 * Test themes work. They are enabled in the app.manifest file but make sure that they work
 * Test performance of the AboutBox 1second timer updates.  CPU and memory usage needs to be ok, and latency needs to be not noticeable.
 * Add more comments, clean code, commit code, optimize code.
 * Add project to VSO/TF
 * Save failed attempts to close a window? need a way to track this
 * Add startup toggle w/ popup verification to systray menu.
 * Use settings and manifest more
 * Trigger the AboutBoxRefreshCounts() function as soon as a value changes instead of waiting for the 1 second timer.
 * Add Icon's to menu system.
 * Migrate registry key code to a different file somewhere?
 * Move RegistryKey rk to be private and called _rk and part of the whole class (not sure if I will do this still)
 */
#endregion

namespace ComponentOneCloser
{
    static class Program
    {
        // Mutex to prevent duplicate runs simultaneously
        static Mutex _mt = null;
        
        /// <summary>
        /// Can we open an existing mutex? mutex check / verification
        /// </summary>
        /// <returns>true - only one instance is running. false - more than one instance is running</returns>
        static bool mutexVerify()
        {
            try
            {
                _mt = Mutex.OpenExisting("ComponentOneCloser");
            }
            catch
            {
                // If there is an exception it means the mutex doesn't exist
                _mt = new Mutex(true, "ComponentOneCloser");

                // Only one instance running
                return true;
            }

            // More than one instance
            return false;
        }

        /// <summary>
        /// Allows garbage collection of the mutex to finally occur and then releases the mutex
        /// </summary>
        static void mutexFree()
        {
            GC.KeepAlive(_mt);
            _mt.ReleaseMutex();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if(!mutexVerify())
            {
                Debug.WriteLine("Component One Closer: More than one instance of the mutex found");
                MessageBox.Show("Component One Closer: Already running.");
                
                // The mutex existed so exit
                _mt.Close();

                Application.Exit();
                return;
            }
            else
            {
                Debug.WriteLine("This is the only instance of Component One Closer. Carry on");
            }

            // Set the iClosedTotals value based on totalNagsClosed
            Closer.iClosedTotal = Properties.Settings.Default.totalNagsClosed;
            Debug.WriteLine("Total nag windows closed (all runs): " + Closer.iClosedTotal);

            // Check privilege escalation and running as admin
            if (AdminHelper.IsRunningAsAdmin() == true)
            {
                Debug.WriteLine("Running as admin already");
            }
            else
            {
                Debug.WriteLine("Not running as admin.");

                if (AdminHelper.CanElevateToAdmin() == true)
                {
                    Debug.WriteLine("Not running as admin. Attempting to RestartAsAdmin().  If that fails, please restart with Administrator Privileges for this to work");
                    MessageBox.Show("Component One Closer: Not running as admin. We will attempt to start this again with Administrator Privileges. If that fails please quit and start it on your own with Administrator Privileges");

                    // Restart this application with administrator privileges
                    AdminHelper.RestartAsAdmin();
                }
                else
                {
                    Debug.WriteLine("Not running as admin. Application cannot be elevated to Administrator Privileges.");
                    MessageBox.Show("Component One Closer: Not running as admin. Cannot elevate to Administrator Privileges.  Exiting.");
                }
                return;
            }

            // Load the windows forms etc
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start timer to run the Closer
            Closer.AddCloserTimer(1000); // 1 second, make this configurable? no need

            // Display systray icon
            using (TrayIcon ti = new TrayIcon())
            {
                // TODO: Should i move some of this to the code before this into the top of here? any benefit?
                ti.Display();

                Application.Run();
            }

            // Allow the mutex to be free at lats
            mutexFree();
        }
    }   
}