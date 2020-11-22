using System;
using System.Collections.Generic;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace DoIt
{
    class MainWindow : Window
    {
        [UI] private ListBox _tasks = null;
        [UI] private Button _addTask = null;

        private int _counter;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _addTask.Clicked += AddTask;
            Util.RefreshList(_tasks);
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void AddTask(object sender, EventArgs e) {
            var dlg = new AddTaskDialog(this);
            var res = (ResponseType)dlg.Run();
            dlg.Hide();
            if(res == ResponseType.Cancel) return;
            Util.toDos.items.Add(new ToDoItem {
                description = dlg.desc.Text
            });
            Util.RefreshList(_tasks);
        }
    }
}
