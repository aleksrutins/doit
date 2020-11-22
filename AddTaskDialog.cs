using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
namespace DoIt {
    public class AddTaskDialog : Dialog {
        [UI] public TextView desc;
        [UI] public Entry name;
        [UI] public Entry days;
        public AddTaskDialog() : this(new Builder("AddTaskDialog.glade")) {}
        public AddTaskDialog(Builder builder) : base(builder.GetObject("AddTaskDialog").Handle) {
            builder.Autoconnect(this);
            AddButton("Cancel", ResponseType.Cancel);
            AddButton("OK", ResponseType.Ok);
            ShowAll();
        }
    }
}