using System;
using System.Windows.Forms;

namespace ComponentOneCloser
{
    public class ContextMenus
    {
        // Is about box displayed?
        public bool isAboutLoaded = false;

        /// <summary>
        /// Creates the menu for our systray icon
        /// </summary>
        /// <returns></returns>
        public ContextMenuStrip Create()
        {
            // Add default menu options
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;

            // About + Stats
            item = new ToolStripMenuItem();
            item.Text = "About / Stats";
            item.Click += new EventHandler(About_Click);
            //item.Image = Resources.About;
            menu.Items.Add(item);

            // Separator
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new System.EventHandler(Exit_Click);
            //item.Image = Resources.Exit;
            menu.Items.Add(item);

            return menu;
        }

        /// <summary>
        /// Calls AboutBoxOpen() to open the About Box when it is selected from our systray menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void About_Click(object sender, EventArgs e)
        {
            AboutBoxOpen();
        }

        /// <summary>
        /// Opens the About Box
        /// </summary>
        public void AboutBoxOpen()
        {
            if(!isAboutLoaded)
            {
                isAboutLoaded = true;
        
                new AboutBox().ShowDialog();

                isAboutLoaded = false;
            }
        }

        /// <summary>
        /// If exit is selected we quit the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
