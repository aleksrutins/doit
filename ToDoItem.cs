using System;
using System.Collections.Generic;
using Gtk;

namespace DoIt
{
    [Serializable]
    public class ToDoItem {
        public string description;
        public List<ToDoItem> subtasks = new List<ToDoItem>();
        public List<string> tags = new List<string>();
        public List<DayOfWeek> days = new List<DayOfWeek>(new DayOfWeek[] {DateTime.Today.DayOfWeek});
        public bool done;
    }
    public class ItemDisplay : Box {
        public ItemDisplay(ToDoItem item) : base(Orientation.Vertical, 6) {
            var hdrBox = new Box(Orientation.Horizontal, 6);
            hdrBox.PackStart(new Label(item.description), true, false, 6);
            var daysBox = new Box(Orientation.Horizontal, 6);
            foreach(var day in item.days) {
                var lbl = new Label(day.ToString());
                if(DateTime.Now.DayOfWeek == day) {
                    // Do it now!
                    lbl.StyleContext.AddClass("doitnow");
                }
                daysBox.PackStart(lbl, false, false, 2);
            }
            hdrBox.PackEnd(daysBox, true, false, 6);
            PackStart(hdrBox, false, false, 6);
            var subtasks = new ListBox();
            foreach(var subtask in item.subtasks) {
                subtasks.Add(new ToDoListItem(subtasks, subtask));
            }
            PackStart(subtasks, true, true, 6);
        }
    }
    public class ToDoListItem : Box {
        public ToDoListItem(ListBox lb, ToDoItem item) : base(Orientation.Horizontal, 6) {
            Add(new Label(item.description));
            var detailsBtn = new Button(new Label("Details"));
            Add(detailsBtn);
            var deleteBtn = new Button(new Label("Delete"));
            deleteBtn.StyleContext.AddClass("danger");
            Add(deleteBtn);
            foreach(var day in item.days) {
                var lbl = new Label(day.ToString());
                if(DateTime.Now.DayOfWeek == day) {
                    // Do it now!
                    lbl.StyleContext.AddClass("doitnow");
                }
                Add(lbl);
            }
            detailsBtn.Clicked += (object o, EventArgs e) => {
                Dialog dlg = new Dialog();
                dlg.Title = $"Item - {item.description}";
                dlg.ContentArea.Add(new ItemDisplay(item));
                dlg.ShowAll();
                dlg.Show();
            };
            deleteBtn.Clicked += (o, e) => {
                Util.toDos.items.Remove(item);
                Util.RefreshList(lb);
            };
        }
    }
}