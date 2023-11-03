namespace ShopAPI.Models
{
    public class User
    {
        public User(int userID, int user_roleID, string user_name,string user_passwordHash)
        {
            this.userID = userID;
            this.user_roleID = user_roleID;
            this.user_name = user_name;
            this.user_passwordHash = user_passwordHash;
        }

        public int userID { get; set; }
        public int user_roleID { get; set; }
        public string user_name { get; set; }
        public string user_passwordHash { get; set; }
    }
}
