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
        [UI] private Button flushBtn = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _addTask.Clicked += AddTask;
            _tasks.ListRowActivated += (o, args) => {
                var row = (ToDoListItem)args.Row;
                ToDoListItem.ShowDetails(this, row);
            };
            flushBtn.Clicked += delegate {
                Util.SaveToDos();
            };
            Util.RefreshList(_tasks);
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void AddTask(object sender, EventArgs e) {
            var res = Util.AddTask();
            Util.toDos.items.Add(new ToDoItem {
                name = res.dlg.name.Text,
                description = res.dlg.desc.Buffer.Text,
                days = res.days
            });
            Util.RefreshList(_tasks);
        }
    }
}
