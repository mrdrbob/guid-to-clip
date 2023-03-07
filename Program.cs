using System;
using System.Windows.Forms;

namespace GuidToClipboard
{
	internal sealed class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			MainForm mainForm = new MainForm();
			Application.Run(mainForm);
		}
		
	}
}
