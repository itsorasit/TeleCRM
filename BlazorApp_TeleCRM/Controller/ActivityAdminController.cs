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


        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string[] Admin = new[]
        {
            "Admin A", "Admin B", "Admin C"
        };

        private static readonly string[] CustomerPhone = new[]
        {
            "086-0981-015", "085-0931-122", "096-0981-013", "067-0981-015"
        };

        private static readonly string[] ActivityProgress = new[]
        {
            "รอดำเนินการ", "สำเร็จ", "ติดตาม" , "เกินกำหนด"
        };

        private static readonly string[] CustomerAction = new[]
        {
             "สั่งสินค้า","ขอคิดดูก่อน","สำเร็จ", "", "ติดต่อไม่ได้", "ปฎิเสธ"
        };

        private static readonly string[] Channel = new[]
        {
             "กิจกรรม","แคมเปญ"
        };

        private static readonly string[] Tag = new[]
         {
             "Up-Sale","Re-Sale","ลูกค้าขุด"
        };


        private static readonly string[] ActivityName = new[]
      {
            "Activity A", "Activity B", "Activity C", "Activity D"
        };


        private static readonly string[] ContactBy = new[]
        {
            "พนักงาน A", "พนักงาน B", "พนักงาน C", "พนักงาน D"
        };

        [HttpGet]
        public IEnumerable<ActivityData> Get()
        {
            return Enumerable.Range(1, 50).Select(index => new ActivityData
            {
                Activity_ID = index,
                Activity_Name = ActivityName[Random.Shared.Next(ActivityName.Length)],
                Activity_Type = Channel[Random.Shared.Next(Channel.Length)],
                Activity_Progress = ActivityProgress[Random.Shared.Next(ActivityProgress.Length)],
                Activity_Duedate = DateTime.Now.AddDays(index),
                Activity_Assign = ContactBy[Random.Shared.Next(ContactBy.Length)],
                CountActivity_Succeed = Random.Shared.Next(0, 50),
                CountActivity_ALL = (Random.Shared.Next(50, 100)) ,
                Activity_Status = Tag[Random.Shared.Next(Tag.Length)],

            })
            .ToArray();
        }

        public class ActivityData
        {
            public int Activity_ID { get; set; }
            public string Activity_Name { get; set; }
            public string Activity_Type { get; set; }
            public string Activity_Progress { get; set; }
            public string Activity_Assign { get; set; }
            public DateTime Activity_Duedate { get; set; }
            public int CountActivity_Succeed { get; set; }
            public int CountActivity_ALL { get; set; }
            public string Activity_Status { get; set; }
            public string Create_By { get; set; }
            public DateTime? Create_Date { get; set; }
            public string UpDate_By { get; set; }
            public DateTime? UpDate_Date { get; set; }
             
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
                        '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total
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
                    '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total
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
                    '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total
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
                                progress = reader["activitys_code"].ToString(),
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
