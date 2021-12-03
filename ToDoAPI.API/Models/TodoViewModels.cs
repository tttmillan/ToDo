using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoAPI.API.Models
{
    public class TodoViewModels
    {
        public int ToDoId { get; set; }
        public string Action { get; set; }
        public bool Done { get; set; }        
        public Nullable<int> CategoryId { get; set; }

        public virtual CategoryViewModels Category { get; set; }
    }

    public class CategoryViewModels
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
    }
}