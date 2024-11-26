namespace BlazorApp_TeleCRM.Data
{
    public class ActivitysDataList
    {
        public string? guid { get; set; }
        public string? customer_code { get; set; }
        public string? branch_code { get; set; }
        public string? touch_point { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? duedate { get; set; }
        public DateTime? reminder_duedate { get; set; }
        public string? assign_work { get; set; }
        public string? assign_work_type { get; set; }
        public bool? allowagent { get; set; }
        public bool? record_status { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string? modified_by { get; set; }
        public DateTime? modified_date { get; set; }
        public string? activitys_code { get; set; }
        public string? progress { get; set; }
        public int? succeed { get; set; }
        public int? progress_total { get; set; }
        public string? product_code { get; set; }
        public string? status { get; set; }
        public string? call_status { get; set; }
        public string? call_action { get; set; }
        public string? sale_order_no { get; set; }
        public string? code { get; set; }

        public string? assign_work_fullName { get; set; }

    }
}
