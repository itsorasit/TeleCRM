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

        public class SearchCriteria
        {
            public string? branch_code { get; set; }
        }

    }
}
