using System;
using Gtk;

namespace DoIt
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            Util.LoadToDos();

            var app = new Application("com.munchkinhalfling.DoIt", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var cssProv = new CssProvider();
            using (var stream = Util.GetResourceStream(typeof(Program).Assembly, "app.css")) {
                cssProv.LoadFromData(Util.ReadAll(stream));
            }
            StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProv, Gtk.StyleProviderPriority.User);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
            Util.SaveToDos();
        }
    }
}
