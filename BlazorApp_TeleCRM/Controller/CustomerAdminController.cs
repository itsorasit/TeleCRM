using BlazorApp_TeleCRM.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

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


        [HttpPost]
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
                    query = @"SELECT guid, code, name, phone, address1, sub_district, district, province, zipcode, datasource_platform, social_media, branch_code, created_by, created_date, modified_by, modified_date
                          FROM mas_customers
                          WHERE branch_code='000000' order by modified_date desc LIMIT 500 ";

                    searchCriteria.fdate = today;
                    searchCriteria.ldate = today;
                }
                else {
                    query = @"SELECT guid, code, name, phone, address1, sub_district, district, province, zipcode, datasource_platform, social_media, branch_code, created_by, created_date, modified_by, modified_date
                          FROM mas_customers
                          WHERE modified_date >= @FromDate 
                          AND modified_date <= @ToDate 
                          AND branch_code='000000' 
                          order by modified_date desc ";

                }

                using (var cmd = new MySqlCommand(query, connection))
                {
                    // กำหนดค่าช่วงเวลาของ FromDate เป็นเริ่มต้นของวัน
                    var fromDate = searchCriteria.fdate?.Date ?? DateTime.MinValue.Date;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);

                    // กำหนดค่าช่วงเวลาของ ToDate เป็นสิ้นสุดของวัน
                    var toDate = (searchCriteria.ldate?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

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
                                modified_date = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15)
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
        }
    }
}
