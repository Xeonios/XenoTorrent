using System;
using Gtk;
using Gdk;
using System.IO;
using System.Threading;

namespace XenoTorrent
{
	class MainClass
	{
		static string tp,dp;
		static string ep;
		
		private static StatusIcon trayIcon;
		
		public static void Main (string[] args)
		{
			bool onlyInstance;
			//Push changes test
			Mutex mtx = new Mutex (true, "XenoTorrent.exe", out onlyInstance); // используйте имя вашего приложения
			
			if (onlyInstance)
			{
				ep = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location);
	
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
				
					if (File.Exists(ep+"/1.png"))
						trayIcon = new StatusIcon (new Pixbuf (ep+"/1.png"));
					else
						trayIcon = new StatusIcon();
					trayIcon.Visible = true;
		 
		
					// Show a pop up menu when the icon has been right clicked.
					trayIcon.PopupMenu += OnTrayIconPopup;
					// A Tooltip for the Icon
					trayIcon.Tooltip = "XenoTorrent";
					
					MainWindow win = new MainWindow (tp,dp,ep, trayIcon);
					
					// Show/Hide the window (even from the Panel/Taskbar) when the TrayIcon has been clicked.
					trayIcon.Activate += delegate {
						win.Move (win.Screen.Width - 385, win.Screen.Height - 320);
						win.Visible = !win.Visible; 
						};
					win.Move (win.Screen.Width - 385, win.Screen.Height - 320);
					win.Show ();
				
				Application.Run ();
			}
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
				File.Delete(ep+"/Error.txt");
				trayIcon.Dispose();
				Application.Quit (); };
				
			MenuItem menuItemlog = new ImageMenuItem ("Log");
			popupMenu.Add (menuItemlog);
			// Quit the application when quit has been clicked.
			if (File.Exists(ep+"/Error.txt"))
			{
				menuItemlog.Activated += delegate {
					System.Diagnostics.Process.Start(ep+"/Error.txt");
				};
				menuItemlog.Sensitive = true;
			}
			else menuItemlog.Sensitive = false;
				
			popupMenu.ShowAll ();
			popupMenu.Popup ();
		}
	
		public static void InitSettings()
		{
			if (File.Exists (ep+"/ini.txt"))
			{
					string s;
					string[] st;
				
					StreamReader sr = new StreamReader (ep + "/ini.txt");
					try
					{
						s = sr.ReadLine ();
						st = s.Split (("=") [0]);
						tp = st [1];
						s = sr.ReadLine ();
						st = s.Split (("=") [0]);
						dp = st [1];
						sr.Close ();
					}
					catch
					{
						sr.Close();
						NewIni();
					}
			}
			else
			{
				NewIni();
			}
		}
		
		static void NewIni()
		{
			PlatformID p = Environment.OSVersion.Platform;
			string[] st = new string[3];
			if (p == PlatformID.Unix)
			{
				st[0] = "TorrentPath="+ep+"/TorrentFiles";
				st[1] = "DownloadPath=" + ep + "/Downloads";
			}
			
			if (p == PlatformID.Win32NT)
			{
					st [0] = "TorrentPath=" + ep + "\\TorrentFiles";
					st [1] = "DownloadPath=" + ep + "\\Downloads";
			}
			File.WriteAllLines(ep + "/ini.txt", st);
			InitSettings();
		}
	}
}

