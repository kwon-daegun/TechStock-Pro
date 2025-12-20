using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Core.Models;

namespace InventorySystem.Services {
    public static class Session {
        public static User? CurrentUser { get; private set; }

        public static void Start(User user) {
            CurrentUser = user;
        }

        public static void Stop() {
            CurrentUser = null;
        }

        public static bool IsAdmin() {
            return CurrentUser?.Role == "Admin";
        }
    }
}