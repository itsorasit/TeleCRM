using BlazorApp_TeleCRM.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CustomerAdminController(IConfiguration configuration)
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

        private static readonly string[] CustomerAddress = new[]
        {
            "กรุงเทพมหานคร", "สระบุรี", "ราชบุรี"
        };

        private static readonly string[] CustomerAction = new[]
        {
             "สั่งสินค้า","ขอคิดดูก่อน","สำเร็จ", "", "ติดต่อไม่ได้", "ปฎิเสธ"
        };

        private static readonly string[] Channel = new[]
      {
             "MyOrder","GoSell"
        };



        private static readonly string[] CustomerFullName = new[]
      {
            "Customer Name A", "Customer Name B", "Customer Name C", "Customer Name D", "Customer Name E", "Customer Name F", "Customer Name G", "Customer Name H", "Customer Name I", "Customer Name J"
        };


        private static readonly string[] ContactBy = new[]
        {
            "พนักงาน A", "พนักงาน B", "พนักงาน C", "พนักงาน D"
        };

        [HttpGet]
        public IEnumerable<CustomerData> Get()
        {
            return Enumerable.Range(1, 50).Select(index => new CustomerData
            {
                Customer_ID = index,
                Customer_FullName = CustomerFullName[Random.Shared.Next(CustomerFullName.Length)],
                Customer_Channel = Channel[Random.Shared.Next(Channel.Length)],
                Customer_Address =   CustomerAddress[Random.Shared.Next(CustomerAddress.Length)],
                Customer_Phone = CustomerPhone[Random.Shared.Next(CustomerPhone.Length)],
                CountActivity = Random.Shared.Next(0, 5),
                Activity_Latest ="xxx",
                Upload_By = Admin[Random.Shared.Next(Admin.Length)],
                Upload_Date = DateTime.Now.AddDays(-index),
               
            })
            .ToArray();
        }

        public class CustomerData
        {
            public int Customer_ID { get; set; }

            public string Customer_FullName { get; set; }
            public string Customer_Phone { get; set; }
            public string Customer_Address { get; set; }
            public string Customer_Channel { get; set; }
            public int CountActivity { get; set; }

            public string Activity_Latest { get; set; }

            public string Upload_By { get; set; }
            public DateTime? Upload_Date { get; set; }
            public string FileName1 { get; set; }
            public string Remark1 { get; set; }
            public string Remark2 { get; set; }
            public string Remark3 { get; set; }
            public string Remark4 { get; set; }
            public string Remark5 { get; set; }

        }


        [HttpPost("GetCustomerData")]
        public async Task<IActionResult> GetCustomerData([FromBody] SearchCriteria searchCriteria)
        {

            DateTime today = DateTime.Now;

            var customers = new List<CustomerDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query = "";

                if (searchCriteria.fdate == null || searchCriteria.ldate == null)
                {
                    query = @"SELECT mc.guid, 
       mc.code, 
       mc.name, 
       mc.phone, 
       mc.address1, 
       mc.sub_district, 
       mc.district, 
       mc.province, 
       mc.zipcode, 
       mc.datasource_platform, 
       mc.social_media, 
       mc.branch_code, 
       mc.created_by, 
       mc.created_date, 
       mc.modified_by, 
       mc.modified_date, 
       COUNT(ca.guid) AS activity_count, 
       (SELECT ca_sub.guid 
        FROM crm_activitys ca_sub 
        WHERE ca_sub.customer_code = mc.guid 
        ORDER BY ca_sub.created_date DESC 
        LIMIT 1) AS latest_activity_guid
FROM mas_customers mc
LEFT JOIN crm_activitys ca ON ca.customer_code = mc.guid
WHERE  mc.branch_code = @branch_code
GROUP BY mc.guid, 
         mc.code, 
         mc.name, 
         mc.phone, 
         mc.address1, 
         mc.sub_district, 
         mc.district, 
         mc.province, 
         mc.zipcode, 
         mc.datasource_platform, 
         mc.social_media, 
         mc.branch_code, 
         mc.created_by, 
         mc.created_date, 
         mc.modified_by, 
         mc.modified_date
         ORDER BY mc.modified_date DESC LIMIT 500 ";

                    searchCriteria.fdate = today;
                    searchCriteria.ldate = today;
                }
                else {
                    query = @"SELECT mc.guid, 
       mc.code, 
       mc.name, 
       mc.phone, 
       mc.address1, 
       mc.sub_district, 
       mc.district, 
       mc.province, 
       mc.zipcode, 
       mc.datasource_platform, 
       mc.social_media, 
       mc.branch_code, 
       mc.created_by, 
       mc.created_date, 
       mc.modified_by, 
       mc.modified_date, 
       COUNT(ca.guid) AS activity_count, 
       (SELECT ca_sub.guid 
        FROM crm_activitys ca_sub 
        WHERE ca_sub.customer_code = mc.guid 
        ORDER BY ca_sub.created_date DESC 
        LIMIT 1) AS latest_activity_guid
FROM mas_customers mc
LEFT JOIN crm_activitys ca ON ca.customer_code = mc.guid
WHERE mc.modified_date >= @FromDate 
  AND mc.modified_date <= @ToDate 
  AND mc.branch_code = @branch_code
GROUP BY mc.guid, 
         mc.code, 
         mc.name, 
         mc.phone, 
         mc.address1, 
         mc.sub_district, 
         mc.district, 
         mc.province, 
         mc.zipcode, 
         mc.datasource_platform, 
         mc.social_media, 
         mc.branch_code, 
         mc.created_by, 
         mc.created_date, 
         mc.modified_by, 
         mc.modified_date
        ORDER BY mc.modified_date DESC;";

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
                            var customer = new CustomerDataList
                            {
                                guid = reader.GetString(0),
                                code = reader.GetString(1),
                                name = reader.GetString(2),
                                phone = reader.GetString(3),
                                address1 = reader.GetString(4),
                                sub_district = reader.GetString(5),
                                district = reader.GetString(6),
                                province = reader.GetString(7),
                                zipcode = reader.GetString(8),
                                datasource_platform = reader.GetString(9),
                                social_media = reader.GetString(10),
                                branch_code = reader.GetString(11),
                                created_by = reader.GetString(12),
                                created_date = reader.GetDateTime(13),
                                modified_by = reader.IsDBNull(14) ? null : reader.GetString(14),
                                modified_date = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15),
                                count_activity = reader.IsDBNull(16) ? (int?)null : reader.GetInt32(16),
                                activity_code = reader.IsDBNull(17) ? null : reader.GetString(17),
                            };

                            customers.Add(customer);
                        }
                    }
                }
            }

            return Ok(customers);
        }

        [HttpPost("GetCustomerDataById")]
        public async Task<IActionResult> GetCustomerDataById([FromBody] SearchCriteriaByID searchCriteria)
        {
            DateTime today = DateTime.Now;
            var customers = new List<CustomerDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"SELECT mc.guid, 
                             mc.code, 
                             mc.name, 
                             mc.phone, 
                             mc.address1, 
                             mc.sub_district, 
                             mc.district, 
                             mc.province, 
                             mc.zipcode, 
                             mc.datasource_platform, 
                             mc.social_media, 
                             mc.branch_code, 
                             mc.created_by, 
                             mc.created_date, 
                             mc.modified_by, 
                             mc.modified_date
                      FROM mas_customers mc
                      WHERE mc.guid = @guid and mc.branch_code = @branch_code";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var guid = searchCriteria.guid ?? "";
                    cmd.Parameters.AddWithValue("@guid", guid);

                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var customer = new CustomerDataList
                            {
                                guid = reader.GetString(reader.GetOrdinal("guid")),
                                code = reader.GetString(reader.GetOrdinal("code")),
                                name = reader.GetString(reader.GetOrdinal("name")),
                                phone = reader.GetString(reader.GetOrdinal("phone")),
                                address1 = reader.GetString(reader.GetOrdinal("address1")),
                                sub_district = reader.GetString(reader.GetOrdinal("sub_district")),
                                district = reader.GetString(reader.GetOrdinal("district")),
                                province = reader.GetString(reader.GetOrdinal("province")),
                                zipcode = reader.GetString(reader.GetOrdinal("zipcode")),
                                datasource_platform = reader.GetString(reader.GetOrdinal("datasource_platform")),
                                social_media = reader.GetString(reader.GetOrdinal("social_media")),
                                branch_code = reader.GetString(reader.GetOrdinal("branch_code")),
                                created_by = reader.GetString(reader.GetOrdinal("created_by")),
                                created_date = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader.GetString(reader.GetOrdinal("modified_by")),
                                modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date")),
                            };

                            customers.Add(customer);
                        }
                    }
                }
            }

            return Ok(customers);
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
            public string? branch_code { get; set; }
        }

    }
}
