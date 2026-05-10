using NewShopShoeApp.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewShopShoeApp.Helpers
{
    public class AccessHelper
    {
        public static bool IsAdmin => CurrentSession.CurrentUser?.RoleId == 1;
        public static bool IsManager => CurrentSession.CurrentUser?.RoleId == 2;
        public static bool IsGuest => CurrentSession.CurrentUser?.RoleId == 3;
    }
}
