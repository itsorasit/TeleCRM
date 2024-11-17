using BlazorApp_TeleCRM.Data;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityAdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ActivityAdminController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("GetActivitysData")]
        public async Task<IActionResult> GetActivitysData([FromBody] SearchCriteria searchCriteria)
        {

            DateTime today = DateTime.Now;

            var activitys = new List<ActivitysDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query = "";

                if (searchCriteria.fdate == null || searchCriteria.ldate == null)
                {
                    query = @"SELECT guid, customer_code, branch_code, touch_point, name, description, startdate, duedate, reminder_duedate, 
                        assign_work, assign_work_type, allowagent, record_status, created_by, created_date, modified_by, modified_date,
                        '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total ,status
                        FROM crm_activitys ca
                        where ca.branch_code = @branch_code
                        ORDER BY ca.created_date  DESC LIMIT 500 ";

                    searchCriteria.fdate = today;
                    searchCriteria.ldate = today;
                }
                else
                {
                    query = @"SELECT guid, customer_code, branch_code, touch_point, name, description, startdate, duedate, reminder_duedate, 
                    assign_work, assign_work_type, allowagent, record_status, created_by, created_date, modified_by, modified_date,
                    '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total ,status
                    FROM crm_activitys ca
                    where ca.branch_code = @branch_code
                    AND  ca.created_date >= @FromDate 
                    AND  ca.created_date <= @ToDate
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


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dataList = new ActivitysDataList
                            {
                                guid = reader["guid"].ToString(),
                                customer_code = reader["customer_code"].ToString(),
                                branch_code = reader["branch_code"].ToString(),
                                touch_point = reader["touch_point"].ToString(),
                                name = reader["name"].ToString(),
                                description = reader["description"].ToString(),
                                startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate")),
                                reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate")),
                                assign_work = reader["assign_work"].ToString(),
                                assign_work_type = reader["assign_work_type"].ToString(),
                                allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent")),
                                record_status = reader.IsDBNull(reader.GetOrdinal("record_status")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("record_status")),


                                created_by = reader["created_by"].ToString(),
                                created_date = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString(),
                                modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date")),

                                status = reader["status"].ToString(),

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

        [HttpPost("GetActivitysById")]
        public async Task<IActionResult> GetActivitysById([FromBody] SearchCriteriaByID searchCriteria)
        {

            if (searchCriteria?.guid == null || string.IsNullOrEmpty(searchCriteria.guid))
            {
                return BadRequest();
            }

            var activitys = new List<ActivitysDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query  = @"SELECT guid, customer_code, branch_code, touch_point, name, description, startdate, duedate, reminder_duedate, 
                    assign_work, assign_work_type, allowagent, record_status, created_by, created_date, modified_by, modified_date,
                    ca.code as activitys_code, ca.status as progress,0 as succeed, 0 as progress_total ,call_status ,call_action, sale_order_no ,ca.status
                    FROM crm_activitys ca
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
                            var  activitysData = new ActivitysDataList
                            {
                                guid = reader["guid"].ToString(),
                                customer_code = reader["customer_code"].ToString(),
                                branch_code = reader["branch_code"].ToString(),
                                touch_point = reader["touch_point"].ToString(),
                                name = reader["name"].ToString(),
                                description = reader["description"].ToString(),
                                startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate")),
                                reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate")),
                                assign_work = reader["assign_work"].ToString(),
                                assign_work_type = reader["assign_work_type"].ToString(),
                                allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent")),
                                record_status = reader.IsDBNull(reader.GetOrdinal("record_status")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("record_status")),


                                created_by = reader["created_by"].ToString(),
                                created_date = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString(),
                                modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date")),


                                activitys_code = reader["activitys_code"].ToString(),
 
                               // progress = reader["progress"].ToString(),
                                status = reader["status"].ToString(),
                               
                                call_status = reader["call_status"].ToString(),
                                call_action  = reader["call_action"].ToString(),
                                sale_order_no = reader["sale_order_no"].ToString(),

                                succeed = reader.IsDBNull(reader.GetOrdinal("succeed")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("succeed")),
                                progress_total = reader.IsDBNull(reader.GetOrdinal("progress_total")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("progress_total"))
                            };

                            activitys.Add(activitysData);
                        }
                    }
                }
            }

            return Ok(activitys);
        }

        [HttpPost("GetActivitysByCustomer")]
        public async Task<IActionResult> GetActivitysByCustomer([FromBody] SearchCriteriaByID searchCriteria)
        {

            DateTime today = DateTime.Now;

            var activitys = new List<ActivitysDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query = @"SELECT guid, customer_code, branch_code, touch_point, name, description, startdate, duedate, reminder_duedate, 
                    assign_work, assign_work_type, allowagent, record_status, created_by, created_date, modified_by, modified_date,
                    '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total ,status ,call_status ,call_action, sale_order_no 
                    FROM crm_activitys ca
                    where ca.customer_code = @customer_code
                    ORDER BY ca.created_date";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    
                    var customer_code = searchCriteria.guid ?? "";
                    cmd.Parameters.AddWithValue("@customer_code", customer_code);


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dataList = new ActivitysDataList
                            {
                                guid = reader["guid"].ToString(),
                                customer_code = reader["customer_code"].ToString(),
                                branch_code = reader["branch_code"].ToString(),
                                touch_point = reader["touch_point"].ToString(),
                                name = reader["name"].ToString(),
                                description = reader["description"].ToString(),
                                startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate")),
                                reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate")),
                                assign_work = reader["assign_work"].ToString(),
                                assign_work_type = reader["assign_work_type"].ToString(),
                                allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent")),
                                record_status = reader.IsDBNull(reader.GetOrdinal("record_status")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("record_status")),


                                created_by = reader["created_by"].ToString(),
                                created_date = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString(),
                                modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date")),

                                status = reader["status"].ToString(),

                                activitys_code = reader["activitys_code"].ToString(),
                                progress = reader["activitys_code"].ToString(),
                                succeed = reader.IsDBNull(reader.GetOrdinal("succeed")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("succeed")),
                                progress_total = reader.IsDBNull(reader.GetOrdinal("progress_total")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("progress_total")),


                                call_status = reader["call_status"].ToString(),
                                call_action = reader["call_action"].ToString(),
                                sale_order_no = reader["sale_order_no"].ToString(),

                            };

                            activitys.Add(dataList);
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
        }


        public class SearchCriteriaByID
        {
            public string? guid { get; set; }
        }

    }
}
