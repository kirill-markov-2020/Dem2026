using NewShopShoeApp.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewShopShoeApp.Statics
{
    public class CurrentSession
    {
        public static User CurrentUser { get; set; }
    }
}
