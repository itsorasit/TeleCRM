namespace BlazorApp_TeleCRM.Data
{
    public class JobDataList
    {
        public string guid { get; set; }
        public string? code { get; set; }
        public string? customer_name { get; set; }
        public string? customer_phone { get; set; }
        public string? customer_address1 { get; set; }
        public string? customer_sub_district { get; set; }
        public string? customer_district { get; set; }
        public string? customer_province { get; set; }
        public string? customer_zipcode { get; set; }
        public string? datasource_platform { get; set; }
        public string? social_media { get; set; }
        public string? branch_code { get; set; }
        public string? created_by { get; set; }
        public DateTime? created_date { get; set; }
        public string? modified_by { get; set; }
        public DateTime? modified_date { get; set; }
        public int?  count_activity { get; set; }
        public string activity_code { get; set; }
        public DateTime? activity_lastdate { get; set; }

        //job
        public string? customer_code { get; set; }
        public string? act_status { get; set; }
        public string? act_guid { get; set; }
        public string? act_name { get; set; }

        public string? call_status { get; set; }
        public string? call_action { get; set; }
        public string? touch_point { get; set; }
        public string? description { get; set; }

        public DateTime? startdate { get; set; }
        public DateTime? duedate { get; set; }
        public DateTime? reminder_duedate { get; set; }

        public string? assign_work { get; set; }
        public string? assign_work_type { get; set; }
        public bool? allowagent { get; set; }
        public string? activitys_code { get; set; }
        public string? progress { get; set; }
        public int? succeed { get; set; }
        public int? progress_total { get; set; }
        public string? product_code { get; set; }
        public string? sale_order_no { get; set; }
        public string? remark { get; set; }
        public int? statusparticipation { get; set; }

        public string? contact_by { get; set; }
        public DateTime? contact_date { get; set; }
        public string? contact_use_phone { get; set; }
        public string? new_activity_ref_guid { get; set; }
        public DateTime? appointment_date { get; set; }
        public string? old_activity_guid { get; set; }
        public bool? re_activity { get; set; }
        public string? contact_result2 { get; set; }
        public string?  contact_created_by { get; set; }
        public DateTime?  contact_created_at { get; set; }

    }
}
