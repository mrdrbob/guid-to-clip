using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace GuidToClipboard
{
	public class MainForm : Form
	{
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		static extern ushort GlobalAddAtom(string lpString);
		[System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
		static extern ushort GlobalDeleteAtom(ushort nAtom);
        enum KeyModifier
        {
            None 	= 0,
            Alt 	= 0x0001,
            Control = 0x0002,
            Shift 	= 0x0004,
            WinKey 	= 0x0008
        }
        
		NotifyIcon _icon;
		ushort ID;

        public const int WM_HOTKEY = 0x312;
        public const int Z_V_KEY = 0x5A;

        public MainForm()
        {
			Assembly assm = typeof(MainForm).Assembly;

			var str = assm.GetManifestResourceStream("GuidToClipboard.favicon.ico");
            
			_icon = new NotifyIcon();
			_icon.Icon = new Icon(str);
			_icon.Visible = true;
			_icon.Click += delegate {
				SetNewGuid();
			};

			_icon.ContextMenu = new ContextMenu(new MenuItem[] {
				new MenuItem("Quit", new EventHandler(ExitClick))
			});

            string atomName = Thread.CurrentThread.ManagedThreadId.ToString("X8") + this.GetType().FullName;
            ID = GlobalAddAtom(atomName);

            bool registered = RegisterHotKey(this.Handle, ID, (int)(KeyModifier.WinKey | KeyModifier.Shift), Z_V_KEY);
            this.Closing += delegate {
                UnregisterHotKey(this.Handle, ID);
                GlobalDeleteAtom(ID);
            };
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_HOTKEY) {
				SetNewGuid();
			} else {
				base.WndProc(ref m);
			}
		}
		
		void SetNewGuid() {
			string guid = Guid.NewGuid().ToString();
            Clipboard.SetText(guid);
            _icon.BalloonTipTitle = "GUID Copied";
            _icon.BalloonTipText = guid;
            _icon.ShowBalloonTip(5000);
        }
		
		protected void ExitClick(object sender, EventArgs e) {
			this.Close();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (_icon != null) {
				_icon.Dispose();
				_icon = null;
			}
			base.Dispose(disposing);
		}
	}
}
