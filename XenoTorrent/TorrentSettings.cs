using System;
using System.IO;
using Gtk;
using MonoTorrent.Common;

namespace XenoTorrent
{
	public partial class TorrentSettings : Gtk.Dialog
	{
		Torrent torrent;
		//TODO Добавить столбец с процентом закаченности
		//TODO Добавить столбец с общим обьемом файла
		//TODO Автоматическое принятие изменений в столбце Priority при нажатии кнопки "ok" 
		public TorrentSettings (Torrent t)
		{
			this.Build ();
			
			torrent = t;
			label1.Text = t.Name;
			CellRendererText crte = new CellRendererText();
			
			
			CellRendererToggle crt = new CellRendererToggle();
			crt.Activatable =true;
			crt.Toggled += HandleCrtToggled;

			string[] cbe = new string[5];
			cbe[0] = "Highest";
			cbe[1] = "High";
			cbe[2] = "Normal";
			cbe[3] = "Low";
			cbe[4] = "Lowest";
					
			ListStore m = new ListStore(typeof(string));
			m.AppendValues(cbe[0]);
			m.AppendValues (cbe[1]);
			m.AppendValues (cbe[2]);
			m.AppendValues (cbe[3]);
			m.AppendValues (cbe[4]);
			CellRendererCombo crc = new CellRendererCombo();
			crc.Model = m;
			crc.TextColumn = 0;
			crc.Editable = true;

			crc.Edited += HandleCrcEdited;
			
			treeview1.AppendColumn ("Path", new CellRendererText(), "text", 0);
			treeview1.Columns[0].Sizing = TreeViewColumnSizing.Fixed;
			treeview1.Columns[0].MinWidth = 50;
			treeview1.Columns[0].MaxWidth = 500;
			treeview1.Columns[0].FixedWidth = 200;
			treeview1.Columns[0].Resizable = true;

			treeview1.AppendColumn ("IsDownload", crt, "active", 1);
			treeview1.AppendColumn("Priority", crc, "text", 2);

			Gtk.TreeStore tfileListStore = new Gtk.TreeStore (typeof(string), typeof(bool), typeof(string));
			
			foreach (TorrentFile file in t.Files)
			{
				if (file.Priority != Priority.DoNotDownload)
				{
					tfileListStore.AppendValues (file.Path, true, file.Priority.ToString());
				}
				else
				{
					tfileListStore.AppendValues (file.Path, false, "Normal");	
				}

					
			}
			
			//Gtk.TreeIter iter = musicListStore.AppendValues (t.Files);
			//musicListStore.AppendValues (iter, "Fannypack", "Nu Nu (Yeah Yeah) (double j and haze radio edit)");
			
			treeview1.Model = tfileListStore;
			treeview1.ShowAll();
		}

		void HandleCrcEdited (object o, EditedArgs args)
		{
			TreeIter it = new TreeIter ();			

			treeview1.Model.GetIterFromString (out it, args.Path);
			treeview1.Model.SetValue (it, 2, args.NewText);
		}

		
		void HandleCrtToggled (object o, ToggledArgs args)
		{
			
			TreeIter it = new TreeIter ();
			treeview1.Model.GetIterFromString(out it, args.Path);
			
			bool crt = Convert.ToBoolean(treeview1.Model.GetValue (it, 1));
			treeview1.Model.SetValue(it,1,!crt);
			
			//(o as CellRendererToggle).Active = crt;
		}
		
		[GLib.ConnectBeforeAttribute]
		protected virtual void OnButtonCancelPress (object o, Gtk.ButtonPressEventArgs args)
		{
			//this.HideAll();
			this.Destroy();
		}
		
		[GLib.ConnectBeforeAttribute]
		protected virtual void OnButtonOkPress (object o, Gtk.ButtonPressEventArgs args)
		{
			TreeIter it = new TreeIter ();
			treeview1.Model.GetIterFirst (out it);
			foreach (TorrentFile file in torrent.Files)
			{
				bool crt = Convert.ToBoolean((treeview1.Model.GetValue (it, 1)));
				string PrioritySetting = treeview1.Model.GetValue (it, 2).ToString();
				
				if (!crt)
					file.Priority = Priority.DoNotDownload;
				else
					switch (PrioritySetting)
					{
					case ("Normal"):
						
						
						{
							file.Priority = Priority.Normal;
							break;
						}

					case ("Highest"):
						
						
						{
							file.Priority = Priority.Highest;
							break;
						}	
	
					case ("High"):
						
						
						{
							file.Priority = Priority.High;
							break;
						}

					case ("Lowest"):
						
						
						{
							file.Priority = Priority.Lowest;
							break;
						}
					
					case ("Low"):
						
						
						{
							file.Priority = Priority.Low;
							break;
						}

					}
				treeview1.Model.IterNext(ref it);
			}
			this.Destroy ();
		}
		
		
		
	}
}

