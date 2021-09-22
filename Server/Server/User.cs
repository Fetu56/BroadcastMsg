using System.Collections.Generic;
using System.Linq;

namespace Server
{
    class User
    {
        public string IP { get; private set; }
        public string Login { get; private set; }
        public string Pass { get; private set; }
        private User(){}
        private User(string log, string pass, string ip){ Login = log; Pass = pass; IP = ip; }
        static public User Registration(string login, string pass, string ip)
        {
            User user = null;
            if(pass.Length > 0 && pass.Length < 16 && login.Length < 16)
            {
                user = new User(login, pass, ip);
            }
            return user;
        }
        static public bool HaveAUser(List<User> users, string login, string pass, string ip)
        {
            return users.Where(x => x.Login == login && x.Pass == pass && x.IP == ip).Count() > 0;
        }
    }
}
