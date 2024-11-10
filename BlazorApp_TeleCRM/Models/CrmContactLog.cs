namespace BlazorApp_TeleCRM.Models
{
    public class CrmContactLog
    {
        public int? log_id { get; set; }              // log_id
        public string? activity_id { get; set; }      // activity_id
        public string? customer_id { get; set; }      // customer_id
        public DateTime? contact_date { get; set; }   // contact_date
        public string? contact_method { get; set; }   // contact_method
        public string? contact_result { get; set; }   // contact_result
        public string? contact_result2 { get; set; }
        public string? contact_remark { get; set; }   // contact_remark
        public string? branch_code { get; set; }      // branch_code
        public string? created_by { get; set; }       // created_by
        public DateTime? created_at { get; set; }     // created_at
        public int? statusparticipation { get; set; }
    }
}
