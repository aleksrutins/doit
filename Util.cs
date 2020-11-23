using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Gtk;

namespace DoIt
{
    public class Util {
        public class DialogParseResult {
            public AddTaskDialog dlg;
            public List<DayOfWeek> days;
            public bool inheritDays;
        }
        public static DialogParseResult AddTask() {
            var dlg = new AddTaskDialog();
            var res = (ResponseType)dlg.Run();
            dlg.Hide();
            if(res == ResponseType.Cancel) return null;
            var days = new List<DayOfWeek>();
            bool inheritDays = false;
            if(dlg.days.Text == "All") {
                days.AddRange(new DayOfWeek[] {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday});
            } else if(dlg.days.Text == "Inherit") {
                inheritDays = true;
            } else if(dlg.days.Text == "Weekdays") {
                days.AddRange(new DayOfWeek[] {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday});
            } else if(!(dlg.days.Text == "" || dlg.days.Text == "None" || dlg.days.Text == null)) {
                var dayStrs = dlg.days.Text.Split(',');
                foreach (var dayStr in dayStrs)
                {
                    days.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayStr));
                }
            }
            return new DialogParseResult {
                dlg = dlg,
                days = days,
                inheritDays = inheritDays
            };
        }
        // https://github.com/GtkSharp/GtkSharp/blob/develop/Source/Samples/Sections/Widgets/ImageSection.cs#L24
        public static Stream GetResourceStream(Assembly assembly, string name)
        {
            var resources = assembly.GetManifestResourceNames();
            var resourceName = resources.SingleOrDefault(str => str == name);

            // try harder:
            if (resourceName == default) {
                resourceName = resources.SingleOrDefault(str => str.EndsWith(name));
            }

            if (resourceName == default)
                return default;
            var stream = assembly.GetManifestResourceStream(resourceName);
            return stream;
        }
        public static string ReadAll(Stream stream) {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        public static string itemsFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.doit.list.bin";
        public static ToDoList toDos;
        public static void LoadToDos() {
            Console.WriteLine("Loading todos");
            var fmt = new BinaryFormatter();
            var stream = new FileStream(itemsFile, FileMode.OpenOrCreate, FileAccess.Read);
            try {
                toDos = (ToDoList)fmt.Deserialize(stream);
            } catch(Exception) {
                toDos = ToDoList.fromArray(new List<ToDoItem>());
            } finally {
                stream.Close();
            }
        }
        public static void SaveToDos() {
            Console.WriteLine("Saving todos");
            var fmt = new BinaryFormatter();
            var stream = new FileStream(itemsFile, FileMode.OpenOrCreate, FileAccess.Write);
            fmt.Serialize(stream, toDos);
            stream.Close();
        }
        public static void RefreshList(Gtk.ListBox widget) {
            RefreshList(widget, toDos, item => {
                toDos.items.Remove(item);
                RefreshList(widget);
            });
        }
        public delegate void ItemRemovedCallback(ToDoItem item);
        public static void RefreshList(Gtk.ListBox widget, ToDoList list, ItemRemovedCallback removedCb) {
            foreach (var child in widget.Children) {
                widget.Remove(child); // Clear listbox
            }
            foreach (var todo in list.items)
            {
                var tdli = new ToDoListItem(widget, todo);
                tdli.deleteBtn.Clicked += delegate {
                    removedCb(todo);
                };
                widget.Add(tdli);
            }
            widget.ShowAll();
        }
    }
}