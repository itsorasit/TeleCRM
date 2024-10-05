namespace BlazorApp_TeleCRM.Models
{
    public class MasCustomers
    {
        public string guid { get; set; }
        public string? code { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string? address1 { get; set; }
        public string? sub_district { get; set; }
        public string? district { get; set; }
        public string? province { get; set; }
        public string? zipcode { get; set; }
        public string? datasource_platform { get; set; }
        public string? social_media  { get; set; }
        public string branch_code { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string? modified_by { get; set; }
        public DateTime? modified_date { get; set; }

    }
}
