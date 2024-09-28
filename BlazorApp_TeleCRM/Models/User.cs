namespace BlazorApp_TeleCRM.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string role { get; set; }
        public string organization { get; set; }
        public string? loginkey { get; set; }
        public DateTime? lastlogin { get; set; }
    }
}
