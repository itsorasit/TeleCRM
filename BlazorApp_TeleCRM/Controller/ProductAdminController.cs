using BlazorApp_TeleCRM.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductAdminController : ControllerBase
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
             "Up-Sale","Re-Sale","ลูกค้าขุด","Cross-Sale"
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
        public IEnumerable<Products> Get()
        {
            List<Products> prders = new List<Products>
        {
            new Products { Code = "P001", Name = "Product 1", Price = 100 ,SalePrice=100 },
            new Products { Code = "P002", Name = "Product 2", Price = 200 ,SalePrice=150 },
            new Products { Code = "P003", Name = "Product 3", Price = 150 ,SalePrice=100 },
            new Products { Code = "P004", Name = "Product 4", Price = 99 ,SalePrice=99 },
            new Products { Code = "P005", Name = "Product 5", Price = 250 ,SalePrice=250 }
        };
            return prders.ToArray();
        }

    }
}
