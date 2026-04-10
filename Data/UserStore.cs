using Luftreise_Command_project_.Models;
using System.Collections.Generic;
using System.Linq;

namespace Luftreise_Command_project_.Data
{
    public static class UserStore
    {
        // список користувачів
        private static List<User> users = new List<User>
        {
            new User
            {
                Id = 1,
                FullName = "Admin",
                Email = "admin@gmail.com",
                Phone = "+380000000000",
                City = "Uzhhorod",
                Country = "Ukraine",
                BirthDate = new DateTime(2000, 1, 1),
                Password = "admin123",
                ConfirmPassword = "admin123",
                IsAdmin = true
            }
        };

      
        public static List<User> Users => users;

        
        public static User? CurrentUser { get; set; }

        public static void AddUser(User user)
        {
            users.Add(user);
        }

        public static User? GetUser(string email, string password)
        {
            return users.FirstOrDefault(u =>
                u.Email == email && u.Password == password);
        }

        public static bool EmailExists(string email)
        {
            foreach (var user in users)
            {
                if (user.Email == email)
                    return true;
            }
            return false;
        }

        public static User? GetUserByEmail(string email)
        {
            return users.FirstOrDefault(u => u.Email == email);
        }

        public static void RemoveUser(User user)
        {
            users.Remove(user);
        }

        public static int GetNextId()
        {
            if (users.Count == 0)
                return 1;

            return users.Max(u => u.Id) + 1;
        }

        public static List<User> GetAllUsers()
        {
            return users;
        }

        public static User? GetUserById(int id)
        {
            return users.FirstOrDefault(u => u.Id == id);
        }

        public static void RemoveUserById(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                users.Remove(user);
            }
        }
    }
}