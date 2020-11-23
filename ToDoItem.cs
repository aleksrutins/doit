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
        public bool done = false;
        public DateTime? doneOn = null;
    }
    public class ItemDisplay : Window {
        [UI] private Box daysBox = null;
        [UI] private Label toDoName = null;
        [UI] private TextView description = null;
        [UI] private ListBox subtasks = null;
        [UI] private Button addSubtaskBtn = null;
        [UI] private Button saveDetailsBtn = null;
        [UI] private Label doneLabel = null;
        [UI] private Button completeBtn = null;
        [UI] private Button refreshSubtasksBtn = null;
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
            if(item.done && (item.doneOn.Value.Day == DateTime.Now.Day || (item.days.IndexOf(DateTime.Now.DayOfWeek) == -1))) {
                doneLabel.StyleContext.AddClass("done-label");
                doneLabel.Text = "Done";
                doneLabel.StyleContext.RemoveClass("notdone");
                doneLabel.StyleContext.AddClass("done");
            } else if(item.days.IndexOf(DateTime.Now.DayOfWeek) != -1) {
                doneLabel.StyleContext.AddClass("done-label");
                item.done = false;
                doneLabel.StyleContext.RemoveClass("done");
                doneLabel.StyleContext.AddClass("notdone");
                doneLabel.Text = "Not done";
            }
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
            if(item.days.Count == 0) completeBtn.Hide();
            addSubtaskBtn.Clicked += delegate {
                var res = Util.AddTask();
                if(res == null) return; // Cancelled
                if(res.inheritDays) {
                    res.days = item.days;
                }
                item.subtasks.Add(new ToDoItem {
                    name = res.dlg.name.Text,
                    description = res.dlg.desc.Buffer.Text,
                    days = res.days
                });
                Util.SaveToDos();
                Util.RefreshList(subtasks, ToDoList.fromArray(item.subtasks), subtaskRemoved);
            };
            subtasks.ListRowActivated += (o, args) => {
                var row = (ToDoListItem)args.Row;
                ToDoListItem.ShowDetails(this, row);
            };
            saveDetailsBtn.Clicked += delegate {
                item.description = description.Buffer.Text;
                Util.SaveToDos();
            };
            completeBtn.Clicked += delegate {
                item.done = true;
                item.doneOn = DateTime.Now;
                doneLabel.Text = "Done";
                doneLabel.StyleContext.RemoveClass("notdone");
                doneLabel.StyleContext.AddClass("done");
            };
            refreshSubtasksBtn.Clicked += delegate {
                Util.RefreshList(subtasks, ToDoList.fromArray(item.subtasks), subtaskRemoved);
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
            var doneLabel = new Label();
            if(item.done && (item.doneOn.Value.Day == DateTime.Now.Day || (item.days.IndexOf(DateTime.Now.DayOfWeek) == -1))) {
                doneLabel.StyleContext.AddClass("done-label");
                doneLabel.Text = "Done";
                doneLabel.StyleContext.RemoveClass("notdone");
                doneLabel.StyleContext.AddClass("done");
            } else if(item.days.IndexOf(DateTime.Now.DayOfWeek) != -1) {
                doneLabel.StyleContext.AddClass("done-label");
                item.done = false;
                doneLabel.StyleContext.RemoveClass("done");
                doneLabel.StyleContext.AddClass("notdone");
                doneLabel.Text = "Not done";
                Util.SaveToDos();
            }
            box.PackStart(doneLabel, false, false, 3);
            var completeBtn = new Button(new Label("Complete"));
            completeBtn.Clicked += delegate {
                item.done = true;
                item.doneOn = DateTime.Now;
                doneLabel.Text = "Done";
                doneLabel.StyleContext.RemoveClass("notdone");
                doneLabel.StyleContext.AddClass("done");
                Util.SaveToDos();
            };
            foreach(var day in item.days) {
                var lbl = new Label(day.ToString());
                lbl.StyleContext.AddClass("day");
                if(DateTime.Now.DayOfWeek == day) {
                    // Do it now!
                    lbl.StyleContext.AddClass("doitnow");
                }
                box.Add(lbl);
            }
            if(item.days.Count != 0) box.Add(completeBtn);
            box.Add(deleteBtn);
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
