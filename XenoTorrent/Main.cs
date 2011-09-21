using System;
using Gtk;
using System.IO;

namespace XenoTorrent
{
	class MainClass
	{
		static string tp,dp;
		public static void Main (string[] args)
		{
		
			InitSettings();
			
			if (!Directory.Exists(tp))
			{
				Directory.CreateDirectory(tp);
			}
			
			if (!Directory.Exists (dp))
			{
				Directory.CreateDirectory (dp);
			}
			
			foreach (string s in args)
			{
				try
				{
					File.Copy(s, tp+"/"+Path.GetFileName(s));
				}
				catch{}
			}

			
			Application.Init ();
			MainWindow win = new MainWindow (tp,dp);
			win.Show ();
			Application.Run ();
		}

	public static void InitSettings()
	{
		if (File.Exists ("ini.txt"))
		{
				string s;
				string[] st;
			
				StreamReader sr = new StreamReader ("ini.txt");
				s = sr.ReadLine ();
				st = s.Split (("=") [0]);
				tp = st [1];
				s = sr.ReadLine ();
				st = s.Split (("=") [0]);
				dp = st [1];
				sr.Close ();
		}
		else
		{
			PlatformID p = Environment.OSVersion.Platform;
			string[] st = new string[2];
			if (p == PlatformID.Unix)
			{
				st[0] = "TorrentPath=TorrentFiles";
				st[1] = "DownloadPath=Downloads";
			}
			
			if (p == PlatformID.Win32Windows)
			{
					st [0] = "TorrentPath=c:\\TorrentFiles";
					st [1] = "DownloadPath=c:\\Downloads";
			}
			File.WriteAllLines("ini.txt",st);
			InitSettings();
		}
	}
	}
}

