namespace BlazorApp_TeleCRM.Models
{
    public class CrmNote
    {
        public string? guid { get; set; }
        public string? activity_id { get; set; }
        public string? customer_id { get; set; }
        public string? note { get; set; }
        public string? branch_code { get; set; }
        public string? created_by { get; set; }
        public DateTime?  created_at { get; set; }
    }
}
