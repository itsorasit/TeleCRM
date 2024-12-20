namespace BlazorApp_TeleCRM.Models
{
    public class MasProductions
    {
        public string? guid { get; set; }
        public string? branch_code { get; set; }
        public string? production_code { get; set; }
        public string? production_name { get; set; }
        public string? description { get; set; }
        public string? image_url { get; set; }
        public string? created_by { get; set; }
        public DateTime? created_date { get; set; }
        public string? modified_by { get; set; }
        public DateTime? modified_date { get; set; }
        public  decimal price { get; set; }
        public string? category { get; set; }
        public string? tags { get; set; }

    }
}
