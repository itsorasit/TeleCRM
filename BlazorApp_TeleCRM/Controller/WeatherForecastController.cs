using Microsoft.AspNetCore.Mvc;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly string[] CustomerFullName = new[]
        {
            "Customer Name A", "Customer Name B", "Customer Name C", "Customer Name D", "Customer Name E", "Customer Name F", "Customer Name G", "Customer Name H", "Customer Name I", "Customer Name J"
        };

        private static readonly string[] CustomerPhone = new[]
        {
            "086-0981-015", "085-0931-122", "096-0981-013", "067-0981-015"
        };

        private static readonly string[] CustomerAddress = new[]
        {
            "กรุงเทพ", "สระบุรี", "ราชบุรี"
        };

        private static readonly string[] CustomerAction = new[]
        {
             "สั่งสินค้า","ขอคิดดูก่อน","สำเร็จ", "", "ติดต่อไม่ได้", "ปฎิเสธ"
        };


        private static readonly string[] ContactBy = new[]
        {
            "พนักงาน A", "พนักงาน B", "พนักงาน C", "พนักงาน D"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 1000).Select(index => new WeatherForecast
            {
                JobID = index,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                Customer_FullName = CustomerFullName[Random.Shared.Next(CustomerFullName.Length)],
                Customer_Phone = CustomerPhone[Random.Shared.Next(CustomerPhone.Length)],
                Customer_Address = CustomerAddress[Random.Shared.Next(CustomerAddress.Length)],
                Customer_Action  = CustomerAction[Random.Shared.Next(CustomerAction.Length)],
                Contact_By = ContactBy[Random.Shared.Next(ContactBy.Length)],
                Contact_Date = DateTime.Now.AddDays(index),
                Last_Contact_By=ContactBy[Random.Shared.Next(ContactBy.Length)],
                Last_Contact_Date =DateTime.Now.AddDays(-index),
                Last_Contact_Action = CustomerAction[Random.Shared.Next(CustomerAction.Length)],
                Remark =""

            })
            .ToArray();
        }

        public class WeatherForecast
        {
            public int JobID { get; set; }
            public DateTime Date { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }
            public string Customer_FullName { get; set; }
            public string Customer_Phone { get; set; }
            public string Customer_Address { get; set; }
            public string Customer_Action { get; set; }
            public string Contact_By { get; set; }
            public DateTime? Contact_Date { get; set; }
            public string Last_Contact_By { get; set; }
            public DateTime? Last_Contact_Date { get; set; }
            public string Last_Contact_Action { get; set; }
            public string Remark { get; set; }

        }
    }
}
