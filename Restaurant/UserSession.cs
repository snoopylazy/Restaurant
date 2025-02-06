using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    internal class UserSession
    {
        public static string LoggedInUser { get; set; } = "Guest"; // Default value if no user is logged in
    }
}
