namespace BlazorApp_TeleCRM.Data
{
    public class DataAdminfollowUp
    {
        public string? customer_id { get; set; }
        public string? customer_fullname { get; set; }
        public string? customer_phone { get; set; }
        public int? follow_up_count { get; set; }
        public int? successful_sales { get; set; }
        public DateTime? start_contact { get; set; }
        public DateTime? last_contact { get; set; }
        public DateTime? last_sale_date { get; set; }
        public int? days_since_last_contact { get; set; }
        public decimal? total_sales { get; set; }
        public int? days_since_last_sale { get; set; }
        public string? last_contact_remark { get; set; }
        public string? crm_staff_name { get; set; }


    }
}
