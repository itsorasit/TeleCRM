using BlazorApp_TeleCRM.Data;
using BlazorApp_TeleCRM.Models;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MSUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public MSUserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("GetUsersData")]
        public async Task<IActionResult> GetUsersData([FromBody] SearchCriteria searchCriteria)
        {

            DateTime today = DateTime.Now;

            var activitys = new List<User>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();



                var branch_code = searchCriteria.branch_code ?? "";
                var digit_code = 0;

                if (branch_code == "000000")
                {
                    branch_code = branch_code.Substring(0, 2);
                    digit_code = 2;
                }
                else {
                    branch_code = branch_code.Substring(0, 4);
                    digit_code = 4;
                }

                
               var query = @"select mu.id, mu.username, mu.email, mu.firstname, mu.lastname, mu.`role`
                            ,mu.organization, mu.lastlogin, mu.imageurl,mb.name,mu.record_status  
                            from mas_users mu 
                            left join mas_branches mb on mb.code = mu.organization 
                            WHERE LEFT(mu.organization, @digit_code) = @branch_code ";

            
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);
                    cmd.Parameters.AddWithValue("@digit_code", digit_code);


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var u = new User
                            {
                                id = Convert.ToInt32(reader["id"].ToString()),
                                username = reader["username"].ToString(),
                                firstname = reader["firstname"].ToString(),
                                email = reader["email"].ToString(),
                                role = reader["role"].ToString(),
                                organization = reader["organization"].ToString(),
                                imageurl = reader["imageurl"].ToString(),
                                lastlogin = reader.IsDBNull(reader.GetOrdinal("lastlogin")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastlogin")),
                                lastname = reader["name"].ToString(),
                                record_status = reader.IsDBNull(reader.GetOrdinal("record_status")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("record_status"))
                            };

                            //User u = new User();
                            //u.id = Convert.ToInt32(reader["id"].ToString());
                            //u.username = reader["username"].ToString();
                            //u.email = reader["email"].ToString();
                            //u.role = reader["role"].ToString();
                            //u.organization = reader["organization"].ToString();
                            //u.imageurl = reader["imageurl"].ToString();
                            //u.lastlogin = reader.IsDBNull(reader.GetOrdinal("lastlogin")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastlogin"));
                            //u.lastname = reader["name"].ToString();
                            //u.record_status = reader.IsDBNull(reader.GetOrdinal("record_status")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("record_status"));

                            activitys.Add(u);
                        }
                    }
                }
            }

            return Ok(activitys);
        }


        [HttpPost("GetUsersDataV2")]
        public async Task<IActionResult> GetUsersDataV2([FromBody] SearchCriteria2 searchCriteria)
        {
            DateTime today = DateTime.Now;
            var activitys = new List<User>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var branch_code = searchCriteria.branch_code ?? "";
                var digit_code = 0;

                if (branch_code == "000000")
                {
                    branch_code = branch_code.Substring(0, 2);
                    digit_code = 2;
                }
                else
                {
                    branch_code = branch_code.Substring(0, 4);
                    digit_code = 4;
                }

                var pageSize = searchCriteria.PageSize > 0 ? searchCriteria.PageSize : 10; // จำนวนรายการต่อหน้า (ค่าเริ่มต้นคือ 10)
                var pageNumber = searchCriteria.PageNumber > 0 ? searchCriteria.PageNumber : 1; // หน้าที่กำลังดึงข้อมูล (ค่าเริ่มต้นคือ 1)
                var offset = (pageNumber - 1) * pageSize;

                var query = @"SELECT mu.id, mu.username, mu.email, mu.firstname, mu.lastname, mu.`role`
                       ,mu.organization, mu.lastlogin, mu.imageurl, mb.name, mu.record_status  
                       FROM mas_users mu 
                       LEFT JOIN mas_branches mb ON mb.code = mu.organization 
                       WHERE LEFT(mu.organization, @digit_code) = @branch_code
                       LIMIT @pageSize OFFSET @offset";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);
                    cmd.Parameters.AddWithValue("@digit_code", digit_code);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    cmd.Parameters.AddWithValue("@offset", offset);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var u = new User
                            {
                                id = Convert.ToInt32(reader["id"]),
                                username = reader["username"].ToString(),
                                firstname = reader["firstname"].ToString(),
                                email = reader["email"].ToString(),
                                role = reader["role"].ToString(),
                                organization = reader["name"].ToString(),
                                imageurl = reader["imageurl"].ToString(),
                                lastlogin = reader.IsDBNull(reader.GetOrdinal("lastlogin")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastlogin")),
                                lastname = reader["name"].ToString(),
                                record_status = reader.IsDBNull(reader.GetOrdinal("record_status")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("record_status"))
                            };

                            activitys.Add(u);
                        }
                    }
                }
            }

            return Ok(activitys);
        }


        [HttpPost("GetUserCount")]
        public async Task<IActionResult> GetUserCount([FromBody] SearchCriteria2 searchCriteria)
        {
            int count = 0;

            var branch_code = searchCriteria.branch_code ?? "";
            var digit_code = 0;

            if (branch_code == "000000")
            {
                branch_code = branch_code.Substring(0, 2);
                digit_code = 2;
            }
            else
            {
                branch_code = branch_code.Substring(0, 4);
                digit_code = 4;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // ปรับปรุง Query ให้รองรับการกรองข้อมูลตามเงื่อนไขที่กำหนด
                var query = @"
            SELECT COUNT(*)
            FROM mas_users mu 
            LEFT JOIN mas_branches mb ON mb.code = mu.organization 
            WHERE LEFT(mu.organization, @digit_code) = @branch_code";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@digit_code", digit_code);
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }

            return Ok(count);
        }



        public class SearchCriteria2
        {
            public string? branch_code { get; set; }
            public int PageSize { get; set; } = 10;  // จำนวนรายการต่อหน้า
            public int PageNumber { get; set; } = 1; // หน้าที่ต้องการดึงข้อมูล
        }


        public class SearchCriteria
        {
            public string? branch_code { get; set; }
        }

    }
}
