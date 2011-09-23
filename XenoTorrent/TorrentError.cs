using System;
using System.IO;
using Gtk;

namespace XenoTorrent
{
	public partial class TorrentError : Gtk.Dialog
	{
		string tpath;
		public TorrentError (string errortext, string tpath)
		{
			this.Build ();
			this.tpath = tpath;

			Gtk.TextTag tt = new Gtk.TextTag("bold");
			tt.Weight = Pango.Weight.Bold;
			
			textview1.Buffer.TagTable.Add(tt);
			
			TextIter tr = textview1.Buffer.GetIterAtLine(0);
			textview1.Buffer.InsertWithTags(ref tr,"<Torrent File>\n",tt);
			textview1.Buffer.Insert(ref tr, tpath+"\n\n");
			textview1.Buffer.InsertWithTags(ref tr,"<Error>\n",tt);
			textview1.Buffer.Insert(ref tr, errortext + "\n\n");
			textview1.Buffer.Insert(ref tr, "Do you want to delete this file?");
		}
		
		[GLib.ConnectBeforeAttribute]
		protected void OnYesButtonRelease (object o, Gtk.ButtonReleaseEventArgs args)
		{
			if (File.Exists(tpath))
				File.Delete(tpath);
			Destroy();
		}
		
		[GLib.ConnectBeforeAttribute]
		protected void OnNoButtonRelease (object o, Gtk.ButtonReleaseEventArgs args)
		{
			Destroy();
		}
	}
}

