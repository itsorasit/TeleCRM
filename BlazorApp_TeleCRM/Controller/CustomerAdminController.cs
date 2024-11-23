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


        private static readonly string[] Channel = new[]
      {
             "MyOrder","GoSell"
        };




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
        WHERE ca_sub.customer_code = mc.guid  and  ca_sub.branch_code =  mc.branch_code
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
        WHERE ca_sub.customer_code = mc.guid  and  ca_sub.branch_code =  mc.branch_code
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
