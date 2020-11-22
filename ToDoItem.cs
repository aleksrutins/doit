using System;
using System.Collections.Generic;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace DoIt
{
    [Serializable]
    public class ToDoItem {
        public string name;
        public string description;
        public List<ToDoItem> subtasks = new List<ToDoItem>();
        public List<string> tags = new List<string>();
        public List<DayOfWeek> days = new List<DayOfWeek>(new DayOfWeek[] {DateTime.Today.DayOfWeek});
        public bool done;
    }
    public class ItemDisplay : Window {
        [UI] private Box daysBox = null;
        [UI] private Label toDoName = null;
        [UI] private TextView description = null;
        [UI] private ListBox subtasks = null;
        [UI] private Button addSubtaskBtn = null;
        [UI] private Button saveDetailsBtn = null;
        private ToDoItem item;

        public ItemDisplay(ToDoItem item) : this(new Builder("ItemDisplay.glade"), item) {}
        private void subtaskRemoved(ToDoItem subtaskItem) {
            item.subtasks.Remove(subtaskItem);
            Util.RefreshList(subtasks, ToDoList.fromArray(item.subtasks), subtaskRemoved);
        }
        public ItemDisplay(Builder builder, ToDoItem item) : base(builder.GetObject("ItemDisplay").Handle) {
            this.item = item;
            builder.Autoconnect(this);
            toDoName.Text = item.name;
            description.Buffer.Text = item.description;
            foreach(var day in item.days) {
                var lbl = new Label(day.ToString());
                lbl.StyleContext.AddClass("day");
                if(DateTime.Now.DayOfWeek == day) {
                    // Do it now!
                    lbl.StyleContext.AddClass("doitnow");
                }
                daysBox.Add(lbl);
            }
            daysBox.ShowAll();
            Util.RefreshList(subtasks, ToDoList.fromArray(item.subtasks), subtaskRemoved);
            subtasks.ShowAll();
            addSubtaskBtn.Clicked += delegate {
                var res = Util.AddTask();
                item.subtasks.Add(new ToDoItem {
                    name = res.dlg.name.Text,
                    description = res.dlg.desc.Buffer.Text,
                    days = res.days
                });
                Util.RefreshList(subtasks, ToDoList.fromArray(item.subtasks), subtaskRemoved);
            };
            subtasks.ListRowActivated += (o, args) => {
                var row = (ToDoListItem)args.Row;
                ToDoListItem.ShowDetails(this, row);
            };
            saveDetailsBtn.Clicked += delegate {
                item.description = description.Buffer.Text;
            };
        }
    }
    public class ToDoListItem : ListBoxRow {
        private ToDoItem tdItem;
        public Button deleteBtn;
        public ToDoListItem(ListBox lb, ToDoItem item) {
            var box = new Box(Orientation.Horizontal, 0);
            tdItem = item;
            var label = new Label(item.name);
            label.Halign = Align.Start;
            box.PackStart(label, true, true, 0);
            deleteBtn = new Button(new Label("Delete"));
            deleteBtn.StyleContext.AddClass("danger");
            box.Add(deleteBtn);
            foreach(var day in item.days) {
                var lbl = new Label(day.ToString());
                lbl.StyleContext.AddClass("day");
                if(DateTime.Now.DayOfWeek == day) {
                    // Do it now!
                    lbl.StyleContext.AddClass("doitnow");
                }
                box.Add(lbl);
            }
            Add(box);
        }
        public static void ShowDetails(Window parent, ToDoListItem listItem) {
            var dlg = new ItemDisplay(listItem.tdItem);
            dlg.Parent = parent;
            dlg.AttachedTo = parent;
            dlg.Show();
        }
    }
}