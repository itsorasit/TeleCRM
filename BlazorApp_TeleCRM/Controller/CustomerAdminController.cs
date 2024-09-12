using Microsoft.AspNetCore.Mvc;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAdminController : ControllerBase
    {
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
    }
}
