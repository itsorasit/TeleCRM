using BlazorApp_TeleCRM.Data;
using BlazorApp_TeleCRM.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductAdminController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductAdminController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }




        [HttpPost("GetProduct")]
        public async Task<IActionResult> GetProduct([FromBody] SearchCriteriaByID searchCriteria)
        {
            var product = new List<MasProductions>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"SELECT p.guid, p.branch_code, p.production_code, p.production_name, 
                p.description, p.image_url, p.created_by, p.created_date, p.modified_by, p.modified_date, p.price ,mc.category_name  
                FROM mas_productions p
                left join mas_categorys mc on mc.guid =p.category  
                WHERE  p.branch_code = @branch_code";

                using (var cmd = new MySqlCommand(query, connection))
                {
                   
                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            MasProductions productions = new MasProductions();

                            productions.guid = reader.GetString(reader.GetOrdinal("guid"));
                            productions.production_code = reader.GetString(reader.GetOrdinal("production_code"));
                            productions.production_name = reader.GetString(reader.GetOrdinal("production_name"));
                            productions.image_url = reader.GetString(reader.GetOrdinal("image_url"));
                            productions.price = decimal.Parse(reader["price"].ToString());
                            productions.category = reader["category_name"].ToString();

                            product.Add(productions);
                        }
                    }
                }
            }

            return Ok(product);
        }

        public class SearchCriteriaByID
        {
            public string? guid { get; set; }
            public string? branch_code { get; set; }
        }

    }
}
