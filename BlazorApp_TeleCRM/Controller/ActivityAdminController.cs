using Microsoft.AspNetCore.Mvc;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityAdminController : ControllerBase
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

        private static readonly string[] ActivityProgress = new[]
        {
            "รอดำเนินการ", "สำเร็จ", "ติดตาม" , "เกินกำหนด"
        };

        private static readonly string[] CustomerAction = new[]
        {
             "สั่งสินค้า","ขอคิดดูก่อน","สำเร็จ", "", "ติดต่อไม่ได้", "ปฎิเสธ"
        };

        private static readonly string[] Channel = new[]
        {
             "กิจกรรม","แคมเปญ"
        };

        private static readonly string[] Tag = new[]
         {
             "Up-Sale","Re-Sale","ลูกค้าขุด"
        };


        private static readonly string[] ActivityName = new[]
      {
            "Activity A", "Activity B", "Activity C", "Activity D"
        };


        private static readonly string[] ContactBy = new[]
        {
            "พนักงาน A", "พนักงาน B", "พนักงาน C", "พนักงาน D"
        };

        [HttpGet]
        public IEnumerable<ActivityData> Get()
        {
            return Enumerable.Range(1, 50).Select(index => new ActivityData
            {
                Activity_ID = index,
                Activity_Name = ActivityName[Random.Shared.Next(ActivityName.Length)],
                Activity_Type = Channel[Random.Shared.Next(Channel.Length)],
                Activity_Progress = ActivityProgress[Random.Shared.Next(ActivityProgress.Length)],
                Activity_Duedate = DateTime.Now.AddDays(index),
                Activity_Assign = ContactBy[Random.Shared.Next(ContactBy.Length)],
                CountActivity_Succeed = Random.Shared.Next(0, 50),
                CountActivity_ALL = (Random.Shared.Next(50, 100)) ,
                Activity_Status = Tag[Random.Shared.Next(Tag.Length)],

            })
            .ToArray();
        }

        public class ActivityData
        {
            public int Activity_ID { get; set; }
            public string Activity_Name { get; set; }
            public string Activity_Type { get; set; }
            public string Activity_Progress { get; set; }
            public string Activity_Assign { get; set; }
            public DateTime Activity_Duedate { get; set; }
            public int CountActivity_Succeed { get; set; }
            public int CountActivity_ALL { get; set; }
            public string Activity_Status { get; set; }
            public string Create_By { get; set; }
            public DateTime? Create_Date { get; set; }
            public string UpDate_By { get; set; }
            public DateTime? UpDate_Date { get; set; }
             
        }
    }
}
