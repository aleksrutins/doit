using System;
using System.Collections.Generic;

namespace DoIt {
    [Serializable]
    public class ToDoList {
        public List<ToDoItem> items;
        public static ToDoList fromArray(List<ToDoItem> array) {
            var res = new ToDoList();
            res.items = array;
            return res;
        }
    }
}