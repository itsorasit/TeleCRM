using BlazorApp_TeleCRM.Data;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Radzen.Blazor;
using static QRCoder.PayloadGenerator;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public JobController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("GetJobData")]
        public async Task<IActionResult> GetJobData([FromBody] SearchCriteria searchCriteria)
        {

            DateTime today = DateTime.Now;

            var activitys = new List<JobDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query = "";

                if (searchCriteria.fdate == null || searchCriteria.ldate == null)
                {
                    query = @"SELECT ca.guid, ca.customer_code, ca.branch_code, ca.touch_point, ca.name, ca.description, ca.startdate, ca.duedate, ca.reminder_duedate,            
                    ca.assign_work, ca.assign_work_type, ca.allowagent, ca.record_status, ca.created_by, ca.created_date, ca.modified_by, ca.modified_date,     
                    '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total ,ca.status ,
                    mc.name as customer_name ,'' as product_code ,ca.call_status ,ca.call_action, 
                    mc.phone  as customer_phone, mc.province as customer_province
                    FROM crm_activitys ca
                    inner join mas_customers mc on mc.guid = ca.customer_code 
                    WHERE ca.branch_code = @branch_code 
                    AND ca.assign_work = @assign_work 
                    ORDER BY ca.created_date  DESC LIMIT 500 ";

                    searchCriteria.fdate = today;
                    searchCriteria.ldate = today;
                }
                else
                {
                    query = @"SELECT ca.guid, ca.customer_code, ca.branch_code, ca.touch_point, ca.name, ca.description, ca.startdate, ca.duedate, ca.reminder_duedate,            
                    ca.assign_work, ca.assign_work_type, ca.allowagent, ca.record_status, ca.created_by, ca.created_date, ca.modified_by, ca.modified_date,     
                    '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total ,ca.status ,
                    mc.name as customer_name ,'' as product_code ,ca.call_status ,ca.call_action, 
                    mc.phone  as customer_phone, mc.province as customer_province
                    FROM crm_activitys ca
                    inner join mas_customers mc on mc.guid = ca.customer_code 
                    WHERE ca.branch_code = @branch_code
                    AND  ca.created_date >= @FromDate 
                    AND ca.assign_work = @assign_work
                    AND  ca.created_date <= @ToDate
                    AND ca.assign_work = @assign_work 
                    ORDER BY ca.created_date";

                }

                using (var cmd = new MySqlCommand(query, connection))
                {
                    // กำหนดค่าช่วงเวลาของ FromDate เป็นเริ่มต้นของวัน
                    var fromDate = searchCriteria.fdate?.Date ?? DateTime.MinValue.Date;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);

                    // กำหนดค่าช่วงเวลาของ ToDate เป็นสิ้นสุดของวัน
                    var toDate = (searchCriteria.ldate?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    var assign_work = searchCriteria.assign_work ?? "";
                    cmd.Parameters.AddWithValue("@assign_work", assign_work);


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dataList = new JobDataList
                            {
                                guid = reader["guid"].ToString(),
                                customer_code = reader["customer_code"].ToString(),
                                branch_code = reader["branch_code"].ToString(),
                                touch_point = reader["touch_point"].ToString(),
                                
                                customer_name = reader["customer_name"].ToString(),
                                customer_phone = reader["customer_phone"].ToString(),
                                customer_province = reader["customer_province"].ToString(),

                                product_code = reader["product_code"].ToString(),

                                description = reader["description"].ToString(),
                                startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate")),
                                reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate")),
                                assign_work = reader["assign_work"].ToString(),
                                assign_work_type = reader["assign_work_type"].ToString(),
                                allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent")),
                              

                                created_by = reader["created_by"].ToString(),
                                created_date = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString(),
                                modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date")),

                                act_status = reader["status"].ToString(),
                                call_status = reader["call_status"].ToString(),
                                call_action = reader["call_action"].ToString(),

                                activitys_code = reader["activitys_code"].ToString(),
                                progress = reader["activitys_code"].ToString(),
                                succeed = reader.IsDBNull(reader.GetOrdinal("succeed")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("succeed")),
                                progress_total = reader.IsDBNull(reader.GetOrdinal("progress_total")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("progress_total"))
                            };

                            activitys.Add(dataList);
                        }
                    }
                }
            }

            return Ok(activitys);
        }

        [HttpPost("GetJobDataByID")]
        public async Task<IActionResult> GetJobDataByID([FromBody] SearchCriteriaByID searchCriteria)
        {

            if (searchCriteria?.guid == null || string.IsNullOrEmpty(searchCriteria.guid))
            {
                return BadRequest();
            }

            var activitys = new List<JobDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


               string query = @"SELECT ca.guid, ca.customer_code, ca.branch_code, ca.touch_point, ca.name, ca.description, ca.startdate, ca.duedate, ca.reminder_duedate,            
                    ca.assign_work, ca.assign_work_type, ca.allowagent, ca.record_status, ca.created_by, ca.created_date, ca.modified_by, ca.modified_date,     
                    '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total ,ca.status ,
                    mc.name as customer_name ,'' as product_code ,ca.call_status ,ca.call_action, 
                    mc.phone  as customer_phone, mc.province as customer_province,
                    mc.address1 as customer_address1, 
                    mc.sub_district as customer_sub_district,
                    mc.district as customer_district,
                    mc.zipcode as customer_zipcode, ca.sale_order_no, ca.remark ,IFNULL(ca.statusparticipation, 0) as statusparticipation,
                    ca.contact_by, ca.contact_date, ca.contact_use_phone, ca.new_activity_ref_guid, ca.appointment_date ,ca.old_activity_guid,
                    ca.re_activity
                    FROM crm_activitys ca
                    inner join mas_customers mc on mc.guid = ca.customer_code 
                    where ca.guid = @guid
                    ";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var guid = searchCriteria.guid ?? "";
                    cmd.Parameters.AddWithValue("@guid", guid);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var  activitysData = new JobDataList
                            {
                                guid = reader["guid"].ToString(),
                                act_guid=  reader["guid"].ToString(),
                                customer_code = reader["customer_code"].ToString(),
                                branch_code = reader["branch_code"].ToString(),
                                touch_point = reader["touch_point"].ToString(),
                                act_name = reader["name"].ToString(),
                                remark  = reader["remark"].ToString(),
                                sale_order_no = reader["sale_order_no"].ToString(),
                                statusparticipation = reader.IsDBNull(reader.GetOrdinal("statusparticipation")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("statusparticipation")),


                                product_code = reader["product_code"].ToString(),

                                description = reader["description"].ToString(),
                                startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate")),
                                reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate")),
                                assign_work = reader["assign_work"].ToString(),
                                assign_work_type = reader["assign_work_type"].ToString(),
                                allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent")),


                                created_by = reader["created_by"].ToString(),
                                created_date = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString(),
                                modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date")),


                                contact_by = reader["contact_by"].ToString(),
                                contact_date = reader.IsDBNull(reader.GetOrdinal("contact_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("contact_date")),
                                contact_use_phone = reader["contact_use_phone"].ToString(),
                                new_activity_ref_guid = reader["new_activity_ref_guid"].ToString(),
                                appointment_date = reader.IsDBNull(reader.GetOrdinal("appointment_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("appointment_date")),
                                old_activity_guid = reader["old_activity_guid"].ToString(),
                                re_activity = reader.IsDBNull(reader.GetOrdinal("re_activity")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("re_activity")),


                                act_status = reader["status"].ToString(),
                                call_status = reader["call_status"].ToString(),
                                call_action = reader["call_action"].ToString(),

                                activitys_code = reader["activitys_code"].ToString(),
                                progress = reader["activitys_code"].ToString(),
                                succeed = reader.IsDBNull(reader.GetOrdinal("succeed")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("succeed")),
                                progress_total = reader.IsDBNull(reader.GetOrdinal("progress_total")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("progress_total")),

                                customer_name = reader["customer_name"].ToString(),
                                customer_phone = reader["customer_phone"].ToString(),
                                customer_address1 = reader["customer_address1"].ToString(),
                                customer_sub_district = reader["customer_sub_district"].ToString(),
                                customer_district = reader["customer_district"].ToString(),
                                customer_province = reader["customer_province"].ToString(),
                                customer_zipcode = reader["customer_zipcode"].ToString()

                            };

                            activitys.Add(activitysData);
                        }
                    }
                }
            }

            return Ok(activitys);
        }


        public class SearchCriteria
        {
            public DateTime? fdate { get; set; }
            public DateTime? ldate { get; set; }
            public string? branch_code { get; set; }
            public string? assign_work { get; set; }
        }


        public class SearchCriteriaByID
        {
            public string? guid { get; set; }
        }

    }
}
