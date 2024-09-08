using Microsoft.AspNetCore.Mvc;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadJobFileController : ControllerBase
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
            "99/1 xxxxxxx xxxxx xxxxx 12050", "10 xxxxxxx xxxxx xxxxx 12050", "31/1 xxxxxxx xxxxx xxxxx 12050"
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
        public IEnumerable<UploadJobFile> Get()
        {
            return Enumerable.Range(1, 100).Select(index => new UploadJobFile
            {
                UploadID = index,
                Upload_By = Admin[Random.Shared.Next(Admin.Length)],
                Upload_Date = DateTime.Now.AddDays(-index),
                CountRowData = Random.Shared.Next(10, 500),
                FileName1 =  "upload_งาน"+ DateTime.Now.AddDays(-index).ToString("ddMMyyyy")+".xlsx",
                Remark = ""
            })
            .ToArray();
        }

        public class UploadJobFile
        {
            public int UploadID { get; set; }
            public string Upload_By { get; set; }
            public DateTime? Upload_Date { get; set; }
            public int? CountRowData { get; set; }
            public string FileName1 { get; set; }
            public string Remark { get; set; }

        }
    }
}
