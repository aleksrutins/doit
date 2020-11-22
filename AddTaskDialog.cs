using Gtk;

namespace DoIt {
    public class AddTaskDialog : Dialog {
        public Entry desc;
        public AddTaskDialog(Window parent) : base("Add Task", parent, DialogFlags.UseHeaderBar) {
            var lblDesc = new Label("Description: ");
            desc = new Entry();
            var lblDays = new Label("Days: ");
            var days = new Button(new Label("Choose"));
            var mainBox = new Box(Orientation.Vertical, 2);
            var boxDays = new Box(Orientation.Horizontal, 2);
            var boxDesc = new Box(Orientation.Horizontal, 2);
            boxDesc.Add(lblDesc);
            boxDesc.Add(desc);
            boxDays.Add(lblDays);
            boxDays.Add(days);
            mainBox.Add(boxDesc);
            mainBox.Add(boxDays);
            ContentArea.Add(mainBox);
            AddButton("Cancel", ResponseType.Cancel);
            AddButton("OK", ResponseType.Ok);
            ShowAll();
        }
    }
}