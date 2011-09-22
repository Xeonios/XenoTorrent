using System;
using Gtk;
using Gdk;
using System.IO;

namespace XenoTorrent
{
	class MainClass
	{
		static string tp,dp;
		private static StatusIcon trayIcon;
		
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
			trayIcon = new StatusIcon (new Pixbuf ("1.png"));
			trayIcon.Visible = true;
 
			// Show/Hide the window (even from the Panel/Taskbar) when the TrayIcon has been clicked.
			trayIcon.Activate += delegate {
				win.Visible = !win.Visible; 
				win.Move (win.Screen.Width - 385, win.Screen.Height - 320);
				};
			// Show a pop up menu when the icon has been right clicked.
			trayIcon.PopupMenu += OnTrayIconPopup;
			// A Tooltip for the Icon
			trayIcon.Tooltip = "XenoTorrent";
			win.Show ();
			win.Move (win.Screen.Width - 385, win.Screen.Height - 320);
			Application.Run ();
		}
	static void OnTrayIconPopup (object o, EventArgs args)
	{
		Menu popupMenu = new Menu ();
		ImageMenuItem menuItemQuit = new ImageMenuItem ("Quit");
		Gtk.Image appimg = new Gtk.Image (Stock.Quit, IconSize.Menu);
		menuItemQuit.Image = appimg;
		popupMenu.Add (menuItemQuit);
		// Quit the application when quit has been clicked.
		menuItemQuit.Activated += delegate {
			Application.Quit (); };
		popupMenu.ShowAll ();
		popupMenu.Popup ();
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

