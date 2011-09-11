using System;
using MonoTorrent.Common;

namespace XenoTorrent
{
	public partial class TorrentSettings : Gtk.Dialog
	{
		public TorrentSettings (Torrent t)
		{
			this.Build ();
			
			label1.Text = t.Name;
			
		}
	}
}

