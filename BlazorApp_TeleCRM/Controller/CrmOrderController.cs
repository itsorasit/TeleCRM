using BlazorApp_TeleCRM.Data;
using BlazorApp_TeleCRM.Models;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrmOrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CrmOrderController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("GetOrderDataByCustomer")]
        public async Task<IActionResult> GetOrderDataByCustomer([FromBody] SearchCriteriaByID searchCriteria)
        {

            if (searchCriteria?.guid == null || string.IsNullOrEmpty(searchCriteria.guid))
            {
                return BadRequest();
            }

            var orders = new List<CrmOrder>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query  = @"SELECT guid, customer_code, branch_code, order_no, order_date, 
                               channel, tracking_no, amount, payment_type, product_codes, 
                               product_names, product_qtys, modified_by, modified_date
                    FROM crm_orders
                    where customer_code = @guid
                    ";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var guid = searchCriteria.guid ?? "";
                    cmd.Parameters.AddWithValue("@guid", guid);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var  crm = new CrmOrder
                            {
                                guid = reader["guid"].ToString(),
                                customer_code = reader["customer_code"].ToString(),
                                branch_code = reader["branch_code"].ToString(),
                                order_no = reader["order_no"].ToString(),
                                order_date = reader["order_date"] != DBNull.Value ? Convert.ToDateTime(reader["order_date"]) : (DateTime?)null,
                                channel = reader["channel"].ToString(),
                                tracking_no = reader["tracking_no"].ToString(),
                                amount = reader["amount"] != DBNull.Value ? Convert.ToDecimal(reader["amount"]) : (decimal?)null,
                                payment_type = reader["payment_type"].ToString(),
                                product_codes = reader["product_codes"].ToString(),
                                product_names = reader["product_names"].ToString(),
                                product_qtys = reader["product_qtys"].ToString(),
                                modified_by = reader["modified_by"].ToString(),
                                modified_date = reader["modified_date"] != DBNull.Value ? Convert.ToDateTime(reader["modified_date"]) : (DateTime?)null
                            };

                            orders.Add(crm);
                        }
                    }
                }
            }

            return Ok(orders);
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
