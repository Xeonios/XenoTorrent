
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Gtk;
using Pango;
using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Common;
using MonoTorrent.BEncoding;
using MonoTorrent.Tracker;
using MonoTorrent.Client.Encryption;
using System.Threading;

public partial class MainWindow : Gtk.Window
{
	//TODO разделение новых торрентов и старых закачек 
	//TODO Открытие диалога с параметрами торрента до начала скачивания нового торрента
	//TODO Хранение параметров старых закачек на харде
	//TODO добавить возможность работы с торрентами на распределенных хеш таблицах
	ClientEngine engine;
	List<TorrentManager> managers = new List<TorrentManager> ();
	Gtk.Menu jBox;
	Gtk.MenuItem MenuItem3;
	string fastResumePath = "FastResume.Data";
	[TreeNode (ListOnly=true)]
 	public class TorrentInfoRow : Gtk.TreeNode 
 	{
        string song_title;

        public TorrentInfoRow (string Tname,string State, string Status, string Ds, string Us, string sc, Torrent torrent, int index)
        {
    		Index = index;
    		this.torrent = torrent;
    		this.Tname = Tname;
            this.State=State;
            this.Status = Status;
            this.Ds = Ds;
            this.Us = Us;
            this.song_title = song_title;
            SeedCount = sc;
        }

		public Torrent torrent;
		public int Index;
		
        [Gtk.TreeNodeValue (Column = 0)]
        public string State;

        [Gtk.TreeNodeValue (Column = 1)]
        public string Status;
        
        [Gtk.TreeNodeValue(Column = 2)]
		public string Ds;

		[Gtk.TreeNodeValue(Column = 3)]
		public string Us;
		
		[Gtk.TreeNodeValue (Column = 4)]
        public int Statusprogress;
        
		[Gtk.TreeNodeValue(Column = 5)]
		public string SeedCount;
		
		public string Tname;

    }
	
	class CellRendererProgressBar : CellRendererProgress
	{
		public CellRendererProgressBar() : base()
		{
			
		}
		
		protected override void Render (Gdk.Drawable window, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, CellRendererState flags)
		{
			// Don't call base Render method of CellRendererProgress (it's looking badly on Windows)
			
			Gdk.GC gc = new Gdk.GC(window);
			
			gc.RgbFgColor = new Gdk.Color(0,0,0);
			window.DrawRectangle(gc,true,cell_area.X,cell_area.Y,cell_area.Width,cell_area.Height);
			
			gc.RgbFgColor = new Gdk.Color(255,255,255);
			window.DrawRectangle(gc,true,cell_area.X+1,cell_area.Y+1,cell_area.Width-2,cell_area.Height-2);
			
			gc.RgbFgColor = new Gdk.Color(130,0,200);
			window.DrawRectangle(gc,true,cell_area.X+2,cell_area.Y+2,(cell_area.Width-4)*Value/100, cell_area.Height-4);
			
			Pango.Layout layout = new Pango.Layout (widget.PangoContext);
			gc.RgbFgColor = new Gdk.Color(0,0,0);
            layout.Wrap = Pango.WrapMode.Word;
            layout.FontDescription = FontDescription.FromString ("Arial 8");
            layout.SetText(Value.ToString()+"%"); 
			window.DrawLayout (gc, expose_area.X + expose_area.Width/2 - (layout.Text.Length*6/2),cell_area.Y+(int)((float)cell_area.Height/2) -6, layout);
		}
	}
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
		EngineSettings es = new EngineSettings ();
		es.PreferEncryption = false;
		es.AllowedEncryption = EncryptionTypes.All;
		es.MaxOpenFiles = 5;
		es.GlobalMaxConnections = 10;
		es.GlobalMaxHalfOpenConnections=10;
		
		engine = new MonoTorrent.Client.ClientEngine (es);
		
		CellRendererProgressBar g= new CellRendererProgressBar();
		
		engine.StatsUpdate += UpdateAll;
		
		nodeview2.NodeStore = new NodeStore (typeof(TorrentInfoRow));
		nodeview2.AppendColumn ("Status", new Gtk.CellRendererText (), "text", 0);
		nodeview2.AppendColumn ("Progress", g, "value", 4);
		nodeview2.AppendColumn ("Ds" + "(КБ/с)", new Gtk.CellRendererText (), "text", 2);
		nodeview2.AppendColumn ("Us" + "(КБ/с)", new Gtk.CellRendererText (), "text", 3);
		nodeview2.AppendColumn ("Seeds", new Gtk.CellRendererText (), "text", 5);
		
		jBox = new Gtk.Menu ();
		Gtk.MenuItem MenuItem1 = new MenuItem ("Запустить");
		MenuItem1.ButtonReleaseEvent += HandleMenuItem1ButtonReleaseEvent;
		jBox.Add (MenuItem1);        
		Gtk.MenuItem MenuItem4 = new MenuItem ("Пауза");
		MenuItem4.ButtonReleaseEvent += HandleMenuItem4ButtonReleaseEvent;
		Gtk.MenuItem MenuItem2 = new MenuItem ("Остановить");
		MenuItem2.ButtonReleaseEvent += HandleMenuItem2ButtonReleaseEvent;
		jBox.Add (MenuItem2);
		MenuItem3 = new MenuItem ("Открыть Файл");
		MenuItem3.Sensitive = false;
		MenuItem3.ButtonReleaseEvent += HandleMenuItem3ButtonReleaseEvent;
		jBox.Add (MenuItem3);
		
		foreach (string file in Directory.GetFiles ("torrents"))
		{
			if (file.EndsWith (".torrent"))
			{
				try
				{
					LoadTorrent (file);
				} catch (Exception e)
				{
					statusbar1.Push(0, "Couldn't decode: "+file);
					continue;
				}
			}
		}
		//engine.StopAll();
		LoadFastResume(managers);
		// Working only with Gtk.Window object! (except [GLib.ConnectBeforeAttribute] attribute is defined on callback method)
		nodeview2.ButtonPressEvent += HandleNodeview2ButtonPressEvent; 
		
		nodeview2.ShowAll ();
		
		engine.StartAll ();
	}
	
	[GLib.ConnectBeforeAttribute]  // without it, the ButtonPress event of nodeview2 doesn't invoked
	void HandleNodeview2ButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		if ((TorrentInfoRow)nodeview2.NodeSelection.SelectedNode != null)
		{
			if ((args.Event.Button==1))
			{
					XenoTorrent.TorrentSettings ts = new XenoTorrent.TorrentSettings (((TorrentInfoRow)nodeview2.NodeSelection.SelectedNode).torrent);
					ts.Show ();
			}
			
			if (args.Event.Button==3)
			{
				if (managers[((TorrentInfoRow)nodeview2.NodeSelection.SelectedNode).Index].State == TorrentState.Seeding)
					MenuItem3.Sensitive = true;
				else
					MenuItem3.Sensitive = false;
				jBox.ShowAll ();
				jBox.Popup ();
			}
		}
	}

	void HandleMenuItem4ButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		managers [((TorrentInfoRow)nodeview2.NodeSelection.SelectedNode).Index].Pause ();
	}
	
	void HandleMenuItem3ButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		System.Diagnostics.Process.Start("Downloads/"+ ((TorrentInfoRow)nodeview2.NodeSelection.SelectedNode).torrent.Name);
		
	}

	void HandleMenuItem2ButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		managers [((TorrentInfoRow)nodeview2.NodeSelection.SelectedNode).Index].Stop ();
	}

	void HandleMenuItem1ButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		managers [((TorrentInfoRow)nodeview2.NodeSelection.SelectedNode).Index].Start ();
	}

	void UpdateAll (object sender, StatsUpdateEventArgs e)
	{
		Application.Invoke(updateState);
	}
	
	
	void LoadTorrent (string path)
	{
		// Load a .torrent file into memory
		Torrent torrent = Torrent.Load (path);
		// Set all the files to not download
	//	foreach (TorrentFile file in torrent.Files)
	//		file.Priority = Priority.DoNotDownload;
		
		// Set the first file as high priority and the second one as normal
		//torrent.Files[0].Priority = Priority.Highest;
		//torrent.Files[1].Priority = Priority.Normal;
		
		TorrentManager manager = new TorrentManager (torrent, "Downloads", new TorrentSettings ());
		//manager.HashCheck (false);
		managers.Add (manager);
		
		engine.Register(manager);
		
		// Disable rarest first and randomised picking - only allow priority based picking (i.e. selective downloading)
		PiecePicker picker = new StandardPicker ();
		picker = new PriorityPicker (picker);
		manager.ChangePicker (picker);
		
		nodeview2.NodeStore.AddNode (new TorrentInfoRow (manager.Torrent.Name, manager.State.ToString (), manager.Torrent.Name.Substring (0, 10), "-0", "0", "-0", torrent, managers.Count -1));
		nodeview2.Columns[0].MinWidth=100;
		nodeview2.Columns[1].MinWidth = 80;
		nodeview2.Columns[2].MinWidth = 65;
		nodeview2.Columns[3].MinWidth = 65;
		nodeview2.Columns[4].MinWidth = 50;
		nodeview2.NodeSelection.Changed += new System.EventHandler (OnSelectionChanged);
	}



	
	void OnSelectionChanged (object o, System.EventArgs args)
	{
		Gtk.NodeSelection selection = (Gtk.NodeSelection)o;
		
		if ((TorrentInfoRow)selection.SelectedNode != null)
		{
			TorrentInfoRow node = (TorrentInfoRow)selection.SelectedNode;
			statusbar1.Push(0,node.Tname);
		}else
			statusbar1.Push (0, ""); 
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		SaveFastResume (managers);
		engine.StopAll ();
		Application.Quit ();
		a.RetVal = true;
	}


	void updateState(object sendera, EventArgs e)
	{
		int i=0;
		foreach (TorrentInfoRow row in nodeview2.NodeStore)
		{
			TorrentManager tm = managers[i];
			row.State = tm.State.ToString();
			row.Status = tm.Progress.ToString("0.00")+"%";
			row.Statusprogress = (int)(tm.Progress);
			row.Ds = ((float)tm.Monitor.DownloadSpeed/1000).ToString("0.0");
			row.Us = ((float)tm.Monitor.UploadSpeed /1000).ToString("0.0");
			row.SeedCount = tm.Peers.Seeds.ToString();
			i++;
		}
		nodeview2.QueueDraw();
	}
	
     public void SaveFastResume (List <TorrentManager> managers)
     {
         // Store the fast resume for each torrent in a list,
         // then serialise the list to the disk.
         BEncodedList list = new BEncodedList ();
         foreach (TorrentManager manager in managers) {

             // Get the fast resume data for the torrent
             FastResume data = manager.SaveFastResume ();

             // Encode the FastResume data to a BEncodedDictionary.
             BEncodedDictionary fastResume = data.Encode ();
             // Add the FastResume dictionary to the main dictionary using
             // the torrents infohash as the key
             list.Add((fastResume as BEncodedValue));
             
         }

         // Write all the fast resume data to disk
         File.WriteAllBytes (fastResumePath, list.Encode ());
     }

	public void LoadFastResume (List <TorrentManager> managers)
	{
		// Read the main dictionary from disk and iterate through
		// all the fast resume items
		if (File.Exists(fastResumePath))
		{
			BEncodedList list = (BEncodedList) BEncodedValue.Decode (File.ReadAllBytes (fastResumePath));
			foreach (BEncodedDictionary fastResume in list) 
			{
				// Decode the FastResume data from the BEncodedDictionary
				FastResume data = new FastResume (fastResume);
				
				// Find the torrentmanager that the fastresume belongs to
				// and then load it
				foreach (TorrentManager manager in managers) 
					if (manager.InfoHash == data.Infohash)
						manager.LoadFastResume (data);
			}
			foreach (TorrentManager manager in managers) 
				if (!manager.HashChecked)
				{
					manager.HashCheck(true);
					XenoTorrent.TorrentSettings ts = new XenoTorrent.TorrentSettings (manager.Torrent);
					ts.Show ();
				}
		}
	}
}

