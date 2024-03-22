namespace UniCom.Commerce.Api.Models
{
    public class UsersCountResponse
    {
        public UsersCountResponse() { }
        public UsersCountResponse(double count) : this((int)count) { }
        public UsersCountResponse(int count)
        {
            UsersCount = count;
        }

        public int UsersCount { get; set; }
    }
}

