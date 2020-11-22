using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace DoIt
{
    public class Util {
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
            var fmt = new BinaryFormatter();
            var stream = new FileStream(itemsFile, FileMode.OpenOrCreate, FileAccess.Read);
            try {
                toDos = (ToDoList)fmt.Deserialize(stream);
            } catch(Exception e) {
                toDos = ToDoList.fromArray(new List<ToDoItem>());
            } finally {
                stream.Close();
            }
        }
        public static void SaveToDos() {
            var fmt = new BinaryFormatter();
            var stream = new FileStream(itemsFile, FileMode.OpenOrCreate, FileAccess.Write);
            fmt.Serialize(stream, toDos);
            stream.Close();
        }
        public static void RefreshList(Gtk.ListBox widget) {
            foreach (var child in widget.Children) {
                widget.Remove(child); // Clear listbox
            }
            foreach (var todo in toDos.items)
            {
                widget.Add(new ToDoListItem(widget, todo));
            }
            widget.ShowAll();
        }
    }
}