using System;
using Gtk;
using System.IO;

namespace XenoTorrent
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (!Directory.Exists("torrents"))
			{
				Directory.CreateDirectory("torrents");
			}
			
			if (!Directory.Exists ("Downloads"))
			{
				Directory.CreateDirectory ("Downloads");
			}
			
			foreach (string s in args)
			{
				try
				{
					File.Copy(s, "torrents/"+Path.GetFileName(s));
				}
				catch{}
			}
			

			
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}


	}
}

