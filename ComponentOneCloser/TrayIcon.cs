using System;
using System.Windows.Forms;
using ComponentOneCloser.Properties;

namespace ComponentOneCloser
{
    class TrayIcon : IDisposable
    {
        NotifyIcon ni;

        /// <summary>
        /// Create a new instance of the TrayIcon class
        /// </summary>
        public TrayIcon()
        {
            ni = new NotifyIcon();
        }

        /// <summary>
        /// Display the icon in the SysTray
        /// </summary>
        public void Display()
        {
            ni.Icon = Resources.SystemTrayApp;
            ni.Text = "Component One Closer";
            ni.Visible = true;

            // Attach a context menu.
            ni.ContextMenuStrip = new ContextMenus().Create();
        }

        /// <summary>
        /// Releases the resources for the NotifyIcon/SysTray
        /// </summary>
        public void Dispose()
        {
            // When the app closes, this removes the icon immediately from systray
            ni.Dispose();
        }
    }
}