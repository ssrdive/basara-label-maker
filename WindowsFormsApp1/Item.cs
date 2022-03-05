using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Item
    {
        public int id { get; set; }
        public string item_id { get; set; }
        public string model_id { get; set; }
        public string item_category_id { get; set; }
        public string page_no { get; set; }
        public string item_no { get; set; }
        public string foreign_id { get; set; }
        public string name { get; set; }
        public string price { get; set; }

        public override string ToString()
        {
            return item_id + "     " + name;
        }
    }
}
