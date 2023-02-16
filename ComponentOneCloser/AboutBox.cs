using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TaskScheduler;

namespace ComponentOneCloser
{
    public partial class AboutBox : Form
    {
        private const String _sStartupKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Runs when we create a new AboutBox()
        /// </summary>
        public AboutBox()
        {
            InitializeComponent();

            // Set AboutBox's various field values
            AboutBoxRefreshCounts();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = AssemblyDescription;

            // During load disable the check changed event for the checkbox and then add it back
            toggleStartupCheckbox(doWeRunAtStartup());

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += new EventHandler(AboutBoxTimer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Class timer that runs every 1 second for this AboutBox form. TODO: Make configurable? not likely.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutBoxTimer_Tick(object sender, EventArgs e)
        {
            AboutBoxRefreshCounts();
        }

        /// <summary>
        /// Refresh the values that AboutBox() displays for # of nag windows closed this run, and lifetime
        /// </summary>
        public void AboutBoxRefreshCounts()
        {
            this.labelNumClosed.Text = Closer.iClosed.ToString();
            this.labelTotalNumClosed.Text = Closer.iClosedTotal.ToString();
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        // Get this to save counts and get it to populate the checkbox on startup as well
        private void chkRunAtStartup_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rk = openRegistryKey();

            Debug.WriteLine("chkRunAtStartup_CheckedChanged event triggered");
            Debug.WriteLine("checked: " + chkRunAtStartup.Checked.ToString());

            // Unable to open registry key
            if (rk == null)
            {
                // Create the subkey
                rk = Registry.LocalMachine.CreateSubKey(_sStartupKey, true);

                if (rk == null)
                {
                    Debug.WriteLine("chkRunAtStartup_CheckedChanged: Could not open Registry Key or create it.");
                    MessageBox.Show("chkRunAtStartup_CheckedChanged: Could not open Registry Key or create it.");
                    return;
                }
            }

            // name is taken from the AssemblyInfo.cs Properties file instead of 'AppName'
            // If the checkbox chkRunAtStartup is checked
            if (chkRunAtStartup.Checked)
            {
                // If we don't have the registry key then create it
                if (doWeRunAtStartup(rk) == false) rk.SetValue(Application.ProductName, "\"" + Application.ExecutablePath + "\""); // Note: we add quotes here so that the path gets set right and spaces don't mess things up

                // Check if the key we just created was successfully created
                if (doWeRunAtStartup(rk) == false)
                {
                    MessageBox.Show("Unable to create startup registry key. Toggled checkbox back to false");
                    Debug.WriteLine("chkRunAtStartup_CheckedChanged(): Unable to create startup registry key. Toggled checkbox back to false");
                    toggleStartupCheckbox(false);
                }
            }
            else
            {
                // If we have a startup key, deleted it
                if (doWeRunAtStartup(rk) == true) rk.DeleteValue(Application.ProductName, false);

                // Check if the key was deleted properly
                if (doWeRunAtStartup(rk) == true)
                {
                    MessageBox.Show("Unable to delete startup registry key. Toggled checkbox back to true");
                    toggleStartupCheckbox(true);
                }
            }

            // Close the key
            rk.Close();
        }

        /// <summary>
        /// Toggles the chkRunAtStartup button to be enabled or disabled, without triggering an event
        /// </summary>
        /// <param name="boxValue"></param>
        private void toggleStartupCheckbox(bool boxValue) // true for set to checked, false if we are changing it to be empty
        {
            // Removed event handler before changing the value, then reenables the event handler. Acts as a debounce and prevents event triggering on AboutBox laod
            chkRunAtStartup.CheckedChanged -= chkRunAtStartup_CheckedChanged;
            chkRunAtStartup.Checked = boxValue;
            chkRunAtStartup.CheckedChanged += chkRunAtStartup_CheckedChanged;
        }

        /// <summary>
        /// Opens the Registry Key for use in within our functions
        /// </summary>
        /// <returns></returns>
        private RegistryKey openRegistryKey()
        {
            RegistryKey rk = null;

            try
            {
                rk = Registry.LocalMachine.OpenSubKey(_sStartupKey, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
                if (rk == null)
                {
                    Debug.WriteLine("openRegistryKey(): Unable to open registry key");
                    MessageBox.Show("openRegistryKey(): Unable to open registry key");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // add exception handling here
            }
            return rk;
        }

        // This overload passes initialDraw and passes in an already opened RegistryKey handle
        /// <summary>
        /// Opens the registry key and then checks it against doWeRunAtStartup(rk)
        /// </summary>
        /// <returns>True if we already are set to run at startup, and False if we do not.</returns>
        private bool doWeRunAtStartup()
        {
            RegistryKey rk = openRegistryKey();
            return doWeRunAtStartup(rk);
        }

        /// <summary>
        /// If we are set to run at startup already, then show the checkbox as checked.
        /// </summary>
        /// <returns>True if we already run at startup, and False if we do not.</returns>
        private bool doWeRunAtStartup(RegistryKey rk)
        {
            bool InStartup = false;

            // Checks the registry key in HKLM and if it exists we output it's contents
            if (rk == null) return InStartup;

            Object runAtStartup = rk.GetValue(Application.ProductName);

            if (runAtStartup != null)
            {
                InStartup = true;
                Debug.WriteLine("doWeRunAtStartup: yes, read value: " + runAtStartup.ToString());
            }
            else
            {
                Debug.WriteLine("doWeRunAtStartup: no, key not set in registry currently");
            }
            return InStartup;
        }
    }
}
