using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneTouchSleepTimeChange
{
    public static class Program
    {
        public enum PowerMode
        {
            Sleep,
            Display,
        }

        public enum CurrentType
        {
            AC,
            DC,
        }

        private static NotifyIcon icon;

        private static ContextMenuStrip menu;

        private static bool autosleep;

        private static bool autodisplayoff;

        public static bool AutoSleep
        {
            get
            {
                return autosleep;
            }
            set
            {
                autosleep = value;

                Process.Start(GetStartInfo(PowerMode.Sleep, CurrentType.AC, autosleep));
                Process.Start(GetStartInfo(PowerMode.Sleep, CurrentType.DC, autosleep));
            }
        }

        public static bool AutoDisplayOff
        {
            get
            {
                return autodisplayoff;
            }
            set
            {
                autodisplayoff = value;

                Process.Start(GetStartInfo(PowerMode.Display, CurrentType.AC, autodisplayoff));
                Process.Start(GetStartInfo(PowerMode.Display, CurrentType.DC, autodisplayoff));
            }
        }

        public static ProcessStartInfo GetStartInfo(PowerMode powermode, CurrentType ct, bool enable)
        {
            var info = new ProcessStartInfo();
            info.FileName = "powercfg";
            info.CreateNoWindow = true; // コンソール・ウィンドウを開かない
            info.UseShellExecute = false; // シェル機能を使用しない

            string args = string.Empty;

            switch (powermode)
            {
                case PowerMode.Sleep:
                    args += "-change -standby-timeout";
                    break;
                case PowerMode.Display:
                    args += "-change -monitor-timeout";
                    break;
            }

            switch (ct)
            {
                case CurrentType.AC:
                    args += "-dc ";
                    break;
                case CurrentType.DC:
                    args += "-ac ";
                    break;
            }

            args += (autodisplayoff ? "1" : "0");

            info.Arguments = args;

            return info;
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OneTouchSleepTimeChange.favicon.ico");

            icon = new NotifyIcon();
            icon.Icon = new Icon(stream);
            icon.Visible = true;
            icon.Text = "スリープ時間変更";

            menu = new ContextMenuStrip();
            menu.Opening += new CancelEventHandler(Menu_Opening);
            icon.ContextMenuStrip = menu;

            autosleep = false;
            autodisplayoff = false;

            Process.Start(GetStartInfo(PowerMode.Sleep, CurrentType.AC, autosleep));
            Process.Start(GetStartInfo(PowerMode.Sleep, CurrentType.DC, autosleep));
            Process.Start(GetStartInfo(PowerMode.Display, CurrentType.AC, autodisplayoff));
            Process.Start(GetStartInfo(PowerMode.Display, CurrentType.DC, autodisplayoff));

            Application.Run();
        }

        private static void Menu_Opening(object sender, CancelEventArgs e)
        {
            MenuInitialize();

            e.Cancel = false;
        }

        private static void MenuInitialize()
        {
            menu.Items.Clear();

            ToolStripMenuItem menuEnableDisplayOff = new ToolStripMenuItem();
            menuEnableDisplayOff.Text = "&ディスプレイの電源を自動で切る";
            menuEnableDisplayOff.Click += AutoDisplayOff_Click;
            menuEnableDisplayOff.CheckState = AutoDisplayOff ? CheckState.Checked : CheckState.Unchecked;
            menu.Items.Add(menuEnableDisplayOff);

            ToolStripMenuItem menuEnableSleep = new ToolStripMenuItem();
            menuEnableSleep.Text = "&自動でスリープする";
            menuEnableSleep.Click += AutoSleep_Click;
            menuEnableSleep.CheckState = AutoSleep ? CheckState.Checked : CheckState.Unchecked;
            menu.Items.Add(menuEnableSleep);

            //アプリ終了
            ToolStripMenuItem menuItemExit = new ToolStripMenuItem();
            menuItemExit.Text = "&終了";
            menuItemExit.Click += Exit_Click;
            menu.Items.Add(menuItemExit);

            icon.ContextMenuStrip = menu;
        }

        private static void Exit_Click(object sender, EventArgs e)
        {
            icon.Dispose();
            Application.Exit();
        }

        private static void AutoSleep_Click(object sender, EventArgs e)
        {
            AutoSleep = !AutoSleep;
        }

        private static void AutoDisplayOff_Click(object sender, EventArgs e)
        {
            AutoDisplayOff = !AutoDisplayOff;
        }

    }
}
