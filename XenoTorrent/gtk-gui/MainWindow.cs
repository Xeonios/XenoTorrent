
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.VBox vbox1;
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	private global::Gtk.NodeView nodeview2;
	private global::Gtk.Statusbar statusbar1;
    
	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.Events = ((global::Gdk.EventMask)(768));
		this.ExtensionEvents = ((global::Gdk.ExtensionMode)(1));
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("Xeno Torrent");
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Events = ((global::Gdk.EventMask)(768));
		this.vbox1.ExtensionEvents = ((global::Gdk.ExtensionMode)(1));
		this.vbox1.Name = "vbox1";
		// Container child vbox1.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Events = ((global::Gdk.EventMask)(768));
		this.GtkScrolledWindow.ExtensionEvents = ((global::Gdk.ExtensionMode)(1));
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.nodeview2 = new global::Gtk.NodeView ();
		this.nodeview2.CanFocus = true;
		this.nodeview2.Events = ((global::Gdk.EventMask)(256));
		this.nodeview2.Name = "nodeview2";
		this.nodeview2.HoverSelection = true;
		this.GtkScrolledWindow.Add (this.nodeview2);
		this.vbox1.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.GtkScrolledWindow]));
		w2.Position = 0;
		// Container child vbox1.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar ();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		this.vbox1.Add (this.statusbar1);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.statusbar1]));
		w3.Position = 1;
		w3.Expand = false;
		w3.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null))
		{
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 372;
		this.DefaultHeight = 227;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
	}
}
