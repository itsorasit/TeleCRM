using BlazorApp_TeleCRM.Data;
using BlazorApp_TeleCRM.Models;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using Radzen.Blazor;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using static BlazorApp_TeleCRM.Controller.CustomerAdminController;
using static QRCoder.PayloadGenerator;

namespace BlazorApp_TeleCRM.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public JobController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("GetJobData")]
        public async Task<IActionResult> GetJobData([FromBody] SearchCriteria searchCriteria)
        {

            DateTime today = DateTime.Now;

            var activitys = new List<JobDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query = "";

                #region
                query = @"SELECT 
    ca.guid, 
    ca.customer_code, 
    ca.branch_code, 
    ca.touch_point, 
    ca.name, 
    ca.description, 
    ca.startdate, 
    ca.duedate, 
    ca.reminder_duedate,            
    ca.assign_work, 
    ca.assign_work_type, 
    ca.allowagent, 
    ca.record_status, 
    ca.created_by , 
    ca.created_date , 
    ca.modified_by, 
    ca.modified_date,     
    '' AS activitys_code, 
    '' AS progress, 
    0 AS succeed, 
    0 AS progress_total,
    ca.status,
    mc.name AS customer_name,
    '' AS product_code,
    ca.call_status, 
    ca.call_action, 
    mc.phone AS customer_phone, 
    mc.province AS customer_province, 
    ca.remark,
    cl.contact_result2,
    cl.created_by AS contact_created_by,
    cl.created_at AS contact_created_at
FROM 
    crm_activitys ca
INNER JOIN 
    mas_customers mc ON mc.guid = ca.customer_code
LEFT JOIN 
    (SELECT customer_id, contact_result2, created_by, created_at,
            ROW_NUMBER() OVER (PARTITION BY customer_id ORDER BY created_at DESC) AS rn
     FROM crm_contact_logs) cl ON cl.customer_id = ca.customer_code AND cl.rn = 1
                    WHERE ca.branch_code = @branch_code
                    AND ca.assign_work = @assign_work
                    AND DATE(ca.startdate) <= DATE(@ToDate)
                    AND DATE(ca.duedate)   >=  DATE(@FromDate)
                    ORDER BY ca.startdate";

                #endregion



                using (var cmd = new MySqlCommand(query, connection))
                {
                    // กำหนดค่าช่วงเวลาของ FromDate เป็นเริ่มต้นของวัน
                    var fromDate = searchCriteria.fdate?.Date ?? DateTime.MinValue.Date;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);

                    // กำหนดค่าช่วงเวลาของ ToDate เป็นสิ้นสุดของวัน
                    var toDate = (searchCriteria.ldate?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    var assign_work = searchCriteria.assign_work ?? "";
                    cmd.Parameters.AddWithValue("@assign_work", assign_work);


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            JobDataList d = new JobDataList();

                            d.guid = reader["guid"].ToString();
                            d.customer_code = reader["customer_code"].ToString();
                            d.branch_code = reader["branch_code"].ToString();
                            d.touch_point = reader["touch_point"].ToString();

                            d.customer_name = reader["customer_name"].ToString();
                            d.customer_phone = reader["customer_phone"].ToString();
                            d.customer_province = reader["customer_province"].ToString();

                            d.product_code = reader["product_code"].ToString();

                            d.description = reader["description"].ToString();
                            d.startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate"));
                            d.duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate"));
                            d.reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate"));
                            d.assign_work = reader["assign_work"].ToString();
                            d.assign_work_type = reader["assign_work_type"].ToString();
                            d.allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent"));


                            d.created_by = reader["created_by"].ToString();
                            d.created_date = reader.GetDateTime(reader.GetOrdinal("created_date"));
                            d.modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString();
                            d.modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date"));

                            d.act_status = reader["status"].ToString();
                            d.call_status = reader["call_status"].ToString();
                            d.call_action = reader["call_action"].ToString();

                            d.activitys_code = reader["activitys_code"].ToString();
                            d.progress = reader["activitys_code"].ToString();
                            d.succeed = reader.IsDBNull(reader.GetOrdinal("succeed")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("succeed"));
                            d.progress_total = reader.IsDBNull(reader.GetOrdinal("progress_total")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("progress_total"));
                            d.remark = reader["remark"].ToString();
                            d.contact_result2 = reader["contact_result2"].ToString();
                            d.contact_created_by = reader["contact_created_by"].ToString();
                            d.contact_created_at = reader.IsDBNull(reader.GetOrdinal("contact_created_at")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("contact_created_at"));

                            activitys.Add(d);
                        };

                    }
                }
            }
            return Ok(activitys);
        }


        [HttpPost("GetJobDataV2")]
        public async Task<IActionResult> GetJobDataV2([FromBody] SearchCriteriaV2 searchCriteria)
        {
            DateTime today = DateTime.Now;

            var activitys = new List<JobDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var pageSize = searchCriteria.PageSize > 0 ? searchCriteria.PageSize : 10;
                var pageNumber = searchCriteria.PageNumber > 0 ? searchCriteria.PageNumber : 1;
                var offset = (pageNumber - 1) * pageSize;

                var query = @"
            SELECT 
                ca.guid, 
                ca.customer_code, 
                ca.branch_code, 
                ca.touch_point, 
                ca.name, 
                ca.description, 
                ca.startdate, 
                ca.duedate, 
                ca.reminder_duedate,            
                ca.assign_work, 
                ca.assign_work_type, 
                ca.allowagent, 
                ca.record_status, 
                ca.created_by , 
                ca.created_date , 
                ca.modified_by, 
                ca.modified_date,     
                '' AS activitys_code, 
                '' AS progress, 
                0 AS succeed, 
                0 AS progress_total,
                ca.status,
                mc.name AS customer_name,
                '' AS product_code,
                ca.call_status, 
                ca.call_action, 
                mc.phone AS customer_phone, 
                mc.province AS customer_province, 
                ca.remark
            FROM 
                crm_activitys ca
            INNER JOIN 
                mas_customers mc ON mc.guid = ca.customer_code
            WHERE  
                ca.branch_code = @branch_code 
                AND ca.assign_work = @assign_work 
        ";

                if (!string.IsNullOrEmpty(searchCriteria.filteringKey))
                {
                    if (searchCriteria.filteringKey == "touch_point")
                    {
                        query += " AND ca.touch_point = @filteringValue ";
                    }
                    else if (searchCriteria.filteringKey == "act_status")
                    {
                        if (searchCriteria.filteringValue == "ปิดการขาย")
                        {
                            query += " AND ca.status = 'ปิดการขาย' ";
                        }
                        else if (searchCriteria.filteringValue == "จัดการแล้ว")
                        {
                            query += " AND ca.status IS NOT NULL AND ca.status <> '' ";
                        }
                        else if (searchCriteria.filteringValue == "งานค้าง")
                        {
                            query += " AND (ca.status IS NULL OR ca.status = '') ";
                        }
                    }
                }

                // เงื่อนไขสำหรับการค้นหาด้วยเบอร์โทรศัพท์หรือชื่อลูกค้า โดยไม่ใช้ช่วงวันที่
                if (!string.IsNullOrEmpty(searchCriteria.filtering_type) &&
                    (searchCriteria.filtering_type == "ค้นจากเบอร์ลูกค้า" || searchCriteria.filtering_type == "ค้นจากชื่อลูกค้า") &&
                    !string.IsNullOrEmpty(searchCriteria.filteringValue2))
                {
                    if (searchCriteria.filtering_type == "ค้นจากเบอร์ลูกค้า")
                    {
                        query += " AND mc.phone LIKE CONCAT('%', @filteringValue2, '%') ";
                    }
                    else if (searchCriteria.filtering_type == "ค้นจากชื่อลูกค้า")
                    {
                        query += " AND mc.name LIKE CONCAT('%', @filteringValue2, '%') ";
                    }
                }
                else
                {
                    // ใช้ช่วงวันที่ถ้าไม่ใช่การค้นหาจากเบอร์ลูกค้าหรือชื่อลูกค้า
                    query += " AND ca.startdate <= @ToDate AND ca.duedate >= @FromDate ";
                }

                query += " ORDER BY ca.startdate LIMIT @pageSize OFFSET @offset; ";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var fromDate = searchCriteria.fdate?.Date ?? DateTime.MinValue.Date;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);

                    var toDate = (searchCriteria.ldate?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    cmd.Parameters.AddWithValue("@branch_code", searchCriteria.branch_code ?? "");
                    cmd.Parameters.AddWithValue("@assign_work", searchCriteria.assign_work ?? "");

                    if (!string.IsNullOrEmpty(searchCriteria.filteringKey))
                    {
                        cmd.Parameters.AddWithValue("@filteringValue", searchCriteria.filteringValue ?? "");
                    }

                    if (!string.IsNullOrEmpty(searchCriteria.filteringValue2))
                    {
                        cmd.Parameters.AddWithValue("@filteringValue2", searchCriteria.filteringValue2);
                    }

                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    cmd.Parameters.AddWithValue("@offset", offset);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            JobDataList d = new JobDataList
                            {
                                guid = reader["guid"].ToString(),
                                customer_code = reader["customer_code"].ToString(),
                                branch_code = reader["branch_code"].ToString(),
                                touch_point = reader["touch_point"].ToString(),
                                customer_name = reader["customer_name"].ToString(),
                                customer_phone = reader["customer_phone"].ToString(),
                                customer_province = reader["customer_province"].ToString(),
                                product_code = reader["product_code"].ToString(),
                                description = reader["description"].ToString(),
                                startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate")),
                                reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate")),
                                assign_work = reader["assign_work"].ToString(),
                                assign_work_type = reader["assign_work_type"].ToString(),
                                allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent")),
                                created_by = reader["created_by"].ToString(),
                                created_date = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString(),
                                modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date")),
                                act_status = reader["status"].ToString(),
                                call_status = reader["call_status"].ToString(),
                                call_action = reader["call_action"].ToString(),
                                remark = reader["remark"].ToString()
                            };

                            activitys.Add(d);
                        }
                    }
                }
            }

            return Ok(activitys);
        }



        [HttpPost("GetJobDataCountV2")]
        public async Task<IActionResult> GetJobDataCountV2([FromBody] SearchCriteriaV2 searchCriteria)
        {
            int count = 0;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
            SELECT COUNT(*) 
            FROM crm_activitys ca
            INNER JOIN mas_customers mc ON mc.guid = ca.customer_code
            WHERE ca.branch_code = @branch_code
            AND ca.assign_work = @assign_work
        ";

                // กรณีค้นหาจากเบอร์ลูกค้าหรือชื่อลูกค้า ไม่ใช้ช่วงวันที่
                if (searchCriteria.filtering_type == "ค้นจากเบอร์ลูกค้า" || searchCriteria.filtering_type == "ค้นจากชื่อลูกค้า")
                {
                    if (!string.IsNullOrEmpty(searchCriteria.filteringValue2))
                    {
                        query += searchCriteria.filtering_type == "ค้นจากเบอร์ลูกค้า"
                            ? " AND mc.phone LIKE CONCAT('%', @filteringValue2, '%') "
                            : " AND mc.name LIKE CONCAT('%', @filteringValue2, '%') ";
                    }
                }
                else
                {
                    query += @"
            AND ca.startdate <= @ToDate
            AND ca.duedate >= @FromDate
            ";
                }

                if (searchCriteria.filteringKey == "touch_point")
                {
                    query += " AND ca.touch_point = @filteringValue ";
                }
                else if (searchCriteria.filteringKey == "act_status")
                {
                    if (searchCriteria.filteringValue == "จัดการแล้ว")
                    {
                        query += " AND (ca.status IS NOT NULL AND ca.status <> '') ";
                    }
                    else if (searchCriteria.filteringValue == "งานค้าง")
                    {
                        query += " AND (ca.status IS NULL OR ca.status = '') ";
                    }
                    else if (searchCriteria.filteringValue == "ปิดการขาย")
                    {
                        query += " AND ca.status = 'ปิดการขาย' ";
                    }
                }

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var fromDate = searchCriteria.fdate?.Date ?? DateTime.MinValue.Date;
                    var toDate = (searchCriteria.ldate?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);

                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    cmd.Parameters.AddWithValue("@branch_code", searchCriteria.branch_code ?? "");
                    cmd.Parameters.AddWithValue("@assign_work", searchCriteria.assign_work ?? "");

                    if (!string.IsNullOrEmpty(searchCriteria.filteringValue))
                    {
                        cmd.Parameters.AddWithValue("@filteringValue", searchCriteria.filteringValue);
                    }

                    if (!string.IsNullOrEmpty(searchCriteria.filteringValue2))
                    {
                        cmd.Parameters.AddWithValue("@filteringValue2", searchCriteria.filteringValue2);
                    }

                    count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            return Ok(count);
        }


        [HttpPost("GetTouchPointCounts")]
        public async Task<IActionResult> GetTouchPointCounts([FromBody] SearchCriteriaV2 searchCriteria)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
            SELECT 
                SUM(CASE WHEN ca.touch_point = 'Up-Sale' THEN 1 ELSE 0 END) AS UpSaleCount,
                SUM(CASE WHEN ca.touch_point = 'Cross-Sale' THEN 1 ELSE 0 END) AS CrossSaleCount,
                SUM(CASE WHEN ca.touch_point = 'Re-Sale' THEN 1 ELSE 0 END) AS ReSaleCount,
                SUM(CASE WHEN ca.touch_point = 'ลูกค้าขุด' THEN 1 ELSE 0 END) AS ลูกค้าขุดCount,
                SUM(CASE WHEN ca.status IS NOT NULL AND ca.status <> '' THEN 1 ELSE 0 END) AS CompletedCount,
                SUM(CASE WHEN ca.status = 'ปิดการขาย' THEN 1 ELSE 0 END) AS SaleCount,
                SUM(CASE WHEN ca.status IS NULL OR ca.status = '' THEN 1 ELSE 0 END) AS `งานค้าง`
            FROM 
                crm_activitys ca 
            INNER JOIN mas_customers mc ON mc.guid = ca.customer_code
            WHERE 
                ca.branch_code = @branch_code 
                AND ca.assign_work = @assign_work  
        ";

                // เงื่อนไขเพิ่มสำหรับการค้นหา
                if (searchCriteria.filtering_type != "ค้นจากเบอร์ลูกค้า" && searchCriteria.filtering_type != "ค้นจากชื่อลูกค้า")
                {
                    query += @" 
                AND ca.startdate <= @ToDate 
                AND ca.duedate >= @FromDate
            ";
                }

                if (searchCriteria.filtering_type == "ค้นจากเบอร์ลูกค้า" && !string.IsNullOrEmpty(searchCriteria.filteringValue2))
                {
                    query += " AND mc.phone LIKE CONCAT('%', @filteringValue2, '%') ";
                }
                else if (searchCriteria.filtering_type == "ค้นจากชื่อลูกค้า" && !string.IsNullOrEmpty(searchCriteria.filteringValue2))
                {
                    query += " AND mc.name LIKE CONCAT('%', @filteringValue2, '%') ";
                }

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@branch_code", searchCriteria.branch_code ?? "");
                    cmd.Parameters.AddWithValue("@assign_work", searchCriteria.assign_work ?? "");

                    // เงื่อนไขเพิ่มสำหรับการค้นหา
                    if (searchCriteria.filtering_type != "ค้นจากเบอร์ลูกค้า" && searchCriteria.filtering_type != "ค้นจากชื่อลูกค้า")
                    {
                        cmd.Parameters.AddWithValue("@FromDate", searchCriteria.fdate ?? DateTime.MinValue);
                        cmd.Parameters.AddWithValue("@ToDate", searchCriteria.ldate ?? DateTime.MaxValue);
                    }

                    if (!string.IsNullOrEmpty(searchCriteria.filteringValue2))
                    {
                        cmd.Parameters.AddWithValue("@filteringValue2", searchCriteria.filteringValue2);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var result = new
                            {
                                UpSaleCount = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                CrossSaleCount = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                ReSaleCount = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                ลูกค้าขุดCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                CompletedCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                SaleCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                งานค้าง = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                            };

                            return Ok(result);
                        }
                    }
                }
            }
            return BadRequest("Failed to retrieve counts.");
        }



        [HttpPost("GetJobDataByID")]
        public async Task<IActionResult> GetJobDataByID([FromBody] SearchCriteriaByID searchCriteria)
        {

            if (searchCriteria?.guid == null || string.IsNullOrEmpty(searchCriteria.guid))
            {
                return BadRequest();
            }

            var activitys = new List<JobDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                string query = @"SELECT ca.guid, ca.customer_code, ca.branch_code, ca.touch_point, ca.name, ca.description, ca.startdate, ca.duedate, ca.reminder_duedate,            
                    ca.assign_work, ca.assign_work_type, ca.allowagent, ca.record_status, ca.created_by, ca.created_date, ca.modified_by, ca.modified_date,     
                    '' as activitys_code,'' as progress,0 as succeed, 0 as progress_total ,ca.status ,
                    mc.name as customer_name ,'' as product_code ,ca.call_status ,ca.call_action, 
                    mc.phone  as customer_phone, mc.province as customer_province,
                    mc.address1 as customer_address1, 
                    mc.sub_district as customer_sub_district,
                    mc.district as customer_district,
                    mc.zipcode as customer_zipcode, ca.sale_order_no, ca.remark ,IFNULL(ca.statusparticipation, 0) as statusparticipation,
                    ca.contact_by, ca.contact_date, ca.contact_use_phone, ca.new_activity_ref_guid, ca.appointment_date ,ca.old_activity_guid,
                    ca.re_activity, ca.sale_amount 
                    FROM crm_activitys ca
                    inner join mas_customers mc on mc.guid = ca.customer_code 
                    where ca.guid = @guid
                    ";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var guid = searchCriteria.guid ?? "";
                    cmd.Parameters.AddWithValue("@guid", guid);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            decimal _sale_amount = 0;

                            if (!string.IsNullOrEmpty(reader["sale_amount"].ToString()))
                            {
                                _sale_amount = Convert.ToDecimal(reader["sale_amount"].ToString());
                            }


                            var activitysData = new JobDataList
                            {
                                guid = reader["guid"].ToString(),
                                act_guid = reader["guid"].ToString(),
                                customer_code = reader["customer_code"].ToString(),
                                branch_code = reader["branch_code"].ToString(),
                                touch_point = reader["touch_point"].ToString(),
                                act_name = reader["name"].ToString(),
                                remark = reader["remark"].ToString(),
                                sale_order_no = reader["sale_order_no"].ToString(),
                                sale_amount = _sale_amount,

                                statusparticipation = reader.IsDBNull(reader.GetOrdinal("statusparticipation")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("statusparticipation")),


                                product_code = reader["product_code"].ToString(),

                                description = reader["description"].ToString(),
                                startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate")),
                                reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate")),
                                assign_work = reader["assign_work"].ToString(),
                                assign_work_type = reader["assign_work_type"].ToString(),
                                allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent")),


                                created_by = reader["created_by"].ToString(),
                                created_date = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString(),
                                modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date")),


                                contact_by = reader["contact_by"].ToString(),
                                contact_date = reader.IsDBNull(reader.GetOrdinal("contact_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("contact_date")),
                                contact_use_phone = reader["contact_use_phone"].ToString(),
                                new_activity_ref_guid = reader["new_activity_ref_guid"].ToString(),
                                appointment_date = reader.IsDBNull(reader.GetOrdinal("appointment_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("appointment_date")),
                                old_activity_guid = reader["old_activity_guid"].ToString(),
                                re_activity = reader.IsDBNull(reader.GetOrdinal("re_activity")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("re_activity")),


                                act_status = reader["status"].ToString(),
                                call_status = reader["call_status"].ToString(),
                                call_action = reader["call_action"].ToString(),

                                activitys_code = reader["activitys_code"].ToString(),
                                progress = reader["activitys_code"].ToString(),
                                succeed = reader.IsDBNull(reader.GetOrdinal("succeed")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("succeed")),
                                progress_total = reader.IsDBNull(reader.GetOrdinal("progress_total")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("progress_total")),

                                customer_name = reader["customer_name"].ToString(),
                                customer_phone = reader["customer_phone"].ToString(),
                                customer_address1 = reader["customer_address1"].ToString(),
                                customer_sub_district = reader["customer_sub_district"].ToString(),
                                customer_district = reader["customer_district"].ToString(),
                                customer_province = reader["customer_province"].ToString(),
                                customer_zipcode = reader["customer_zipcode"].ToString()

                            };

                            activitys.Add(activitysData);
                        }
                    }
                }
            }

            return Ok(activitys);
        }

        [HttpPost("GetCrmContactLogDataByID")]
        public async Task<IActionResult> GetCrmContactLogDataByID([FromBody] SearchCriteriaByID searchCriteria)
        {

            if (searchCriteria?.guid == null || string.IsNullOrEmpty(searchCriteria.guid))
            {
                return BadRequest();
            }

            var Datas = new List<CrmContactLog>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                string query = @"SELECT log_id, activity_id, customer_id, contact_date, contact_method, contact_result, contact_remark, 
                    branch_code, created_by, created_at ,statusparticipation ,
                    contact_result2
                    FROM crm_contact_logs 
                    where customer_id = @guid and branch_code=@branch_code 
                    order by created_at desc  ";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var guid = searchCriteria.guid ?? "";
                    cmd.Parameters.AddWithValue("@guid", guid);

                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var Data = new CrmContactLog
                            {
                                log_id = reader.GetInt32("log_id"),
                                activity_id = reader.GetString("activity_id"),
                                customer_id = reader.GetString("customer_id"),
                                contact_date = reader.GetDateTime("contact_date"),
                                contact_method = reader.GetString("contact_method"),
                                contact_result = reader.GetString("contact_result"),
                                contact_result2 = reader.GetString("contact_result2"),
                                contact_remark = reader.GetString("contact_remark"),
                                branch_code = reader.GetString("branch_code"),
                                created_by = reader.GetString("created_by"),
                                created_at = reader.GetDateTime("created_at"),
                                statusparticipation = reader.GetInt32("statusparticipation")

                            };

                            Datas.Add(Data);
                        }
                    }
                }
            }

            return Ok(Datas);
        }

        [HttpPost("GetCrmNoteDataByID")]
        public async Task<IActionResult> GetCrmNoteDataByID([FromBody] SearchCriteriaByID searchCriteria)
        {

            if (searchCriteria?.guid == null || string.IsNullOrEmpty(searchCriteria.guid))
            {
                return BadRequest();
            }

            var Datas = new List<CrmNote>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                string query = @"SELECT guid, activity_id, customer_id, 
                    note, branch_code, created_by, created_at 
                    FROM crm_notes 
                    where customer_id = @guid and branch_code = @branch_code 
                    order by created_at desc  ";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    var guid = searchCriteria.guid ?? "";
                    cmd.Parameters.AddWithValue("@guid", guid);

                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var Data = new CrmNote
                            {
                                guid = reader.GetString("guid"),
                                activity_id = reader.GetString("activity_id"),
                                customer_id = reader.GetString("customer_id"),
                                note = reader.GetString("note"),
                                branch_code = reader.GetString("branch_code"),
                                created_by = reader.GetString("created_by"),
                                created_at = reader.GetDateTime("created_at"),
                            };

                            Datas.Add(Data);
                        }
                    }
                }
            }

            return Ok(Datas);
        }


        [HttpPost("GetjobCalendar")]
        public async Task<IActionResult> GetjobCalendar([FromBody] SearchCriteria searchCriteria)
        {

            DateTime today = DateTime.Now;

            var appointments = new List<Appointment>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query = "";

                if (searchCriteria.fdate == null || searchCriteria.ldate == null)
                {
                    query = @"SELECT 
    ca.touch_point,
    DATE(ca.startdate) AS startdate, 
    IFNULL(ca.status, 'รอดำเนินการ') AS status,
    COUNT(*) AS count
FROM 
    crm_activitys ca
WHERE 
    ca.branch_code = @branch_code 
    AND ca.assign_work = @assign_work
GROUP BY 
    ca.touch_point, 
    IFNULL(ca.status, 'รอดำเนินการ'),
    DATE(ca.startdate) 
";
                    //AND ca.startdate <= NOW()
                    //  AND ca.duedate >= NOW()


                    searchCriteria.fdate = today;
                    searchCriteria.ldate = today;
                }
                else
                {
                    query = @"SELECT 
    ca.guid, 
    ca.customer_code, 
    ca.branch_code, 
    ca.touch_point, 
    ca.name, 
    ca.description, 
    ca.startdate, 
    ca.duedate, 
    ca.reminder_duedate,            
    ca.assign_work, 
    ca.assign_work_type, 
    ca.allowagent, 
    ca.record_status, 
    ca.created_by , 
    ca.created_date , 
    ca.modified_by, 
    ca.modified_date,     
    '' AS activitys_code, 
    '' AS progress, 
    0 AS succeed, 
    0 AS progress_total,
    ca.status,
    mc.name AS customer_name,
    '' AS product_code,
    ca.call_status, 
    ca.call_action, 
    mc.phone AS customer_phone, 
    mc.province AS customer_province, 
    ca.remark,
    cl.contact_result2,
    cl.created_by AS contact_created_by,
    cl.created_at AS contact_created_at
FROM 
    crm_activitys ca
INNER JOIN 
    mas_customers mc ON mc.guid = ca.customer_code
LEFT JOIN 
    (SELECT customer_id, contact_result2, created_by, created_at,
            ROW_NUMBER() OVER (PARTITION BY customer_id ORDER BY created_at DESC) AS rn
     FROM crm_contact_logs) cl ON cl.customer_id = ca.customer_code AND cl.rn = 1
                    WHERE ca.branch_code = @branch_code
                    AND  ca.startdate >= @FromDate 
                    AND ca.assign_work = @assign_work
                    AND  ca.startdate <= @ToDate
                    AND ca.assign_work = @assign_work 
                    ORDER BY ca.startdate";

                }

                using (var cmd = new MySqlCommand(query, connection))
                {
                    // กำหนดค่าช่วงเวลาของ FromDate เป็นเริ่มต้นของวัน
                    var fromDate = searchCriteria.fdate?.Date ?? DateTime.MinValue.Date;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);

                    // กำหนดค่าช่วงเวลาของ ToDate เป็นสิ้นสุดของวัน
                    var toDate = (searchCriteria.ldate?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    var assign_work = searchCriteria.assign_work ?? "";
                    cmd.Parameters.AddWithValue("@assign_work", assign_work);


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Appointment d = new Appointment();

                            d.Text = reader["touch_point"].ToString() + "-[" + reader["status"].ToString() + " (" + reader["count"].ToString() + ")]";
                            d.Start = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate"));
                            d.End = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate"));

                            appointments.Add(d);
                        };

                    }
                }
            }
            return Ok(appointments);
        }


        [HttpPost("GetAdminjobCalendar")]
        public async Task<IActionResult> GetAdminjobCalendar([FromBody] SearchCriteria searchCriteria)
        {

            DateTime today = DateTime.Now;

            var appointments = new List<Appointment>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"SELECT 
    ca.touch_point,
    DATE(ca.startdate) AS startdate, 
    IFNULL(ca.status, 'รอดำเนินการ') AS status,
    COUNT(*) AS count
FROM 
    crm_activitys ca
WHERE 
    ca.branch_code = @branch_code 
    AND FIND_IN_SET(ca.assign_work, @assign_work) 
GROUP BY 
    ca.touch_point, 
    IFNULL(ca.status, 'รอดำเนินการ'),
    DATE(ca.startdate) 
";

                using (var cmd = new MySqlCommand(query, connection))
                {

                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    var assign_work = searchCriteria.assign_work ?? "";
                    cmd.Parameters.AddWithValue("@assign_work", assign_work);


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Appointment d = new Appointment();

                            d.Text = reader["touch_point"].ToString() + "-[" + reader["status"].ToString() + " (" + reader["count"].ToString() + ")]";
                            d.Start = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate"));
                            d.End = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate"));

                            appointments.Add(d);
                        };

                    }
                }
            }
            return Ok(appointments);
        }



        [HttpPost("GetJobDataTargetedByKey")]
        public async Task<IActionResult> GetJobDataTargetedByKey([FromBody] SearchCriteriaByKey searchCriteria)
        {

            DateTime today = DateTime.Now;

            var activitys = new List<JobDataList>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var query = "";

                if (searchCriteria.key1 == "ค้นจากเบอร์ลูกค้า")
                {
                    query = @"SELECT 
    ca.guid, 
    ca.customer_code, 
    ca.branch_code, 
    ca.touch_point, 
    ca.name, 
    ca.description, 
    ca.startdate, 
    ca.duedate, 
    ca.reminder_duedate,            
    ca.assign_work, 
    ca.assign_work_type, 
    ca.allowagent, 
    ca.record_status, 
    ca.created_by , 
    ca.created_date , 
    ca.modified_by, 
    ca.modified_date,     
    '' AS activitys_code, 
    '' AS progress, 
    0 AS succeed, 
    0 AS progress_total,
    ca.status,
    mc.name AS customer_name,
    '' AS product_code,
    ca.call_status, 
    ca.call_action, 
    mc.phone AS customer_phone, 
    mc.province AS customer_province, 
    ca.remark,
    cl.contact_result2,
    cl.created_by AS contact_created_by,
    cl.created_at AS contact_created_at
FROM 
    crm_activitys ca
INNER JOIN 
    mas_customers mc ON mc.guid = ca.customer_code
LEFT JOIN 
    (SELECT customer_id, contact_result2, created_by, created_at,
            ROW_NUMBER() OVER (PARTITION BY customer_id ORDER BY created_at DESC) AS rn
     FROM crm_contact_logs) cl ON cl.customer_id = ca.customer_code AND cl.rn = 1
                    WHERE ca.branch_code = @branch_code
                    AND ca.assign_work = @assign_work
                    AND mc.phone LIKE CONCAT('%', @value1, '%') 
                    ORDER BY ca.startdate";
                }
                else if (searchCriteria.key1 == "ค้นจากชื่อลูกค้า")
                {
                    query = @"SELECT 
    ca.guid, 
    ca.customer_code, 
    ca.branch_code, 
    ca.touch_point, 
    ca.name, 
    ca.description, 
    ca.startdate, 
    ca.duedate, 
    ca.reminder_duedate,            
    ca.assign_work, 
    ca.assign_work_type, 
    ca.allowagent, 
    ca.record_status, 
    ca.created_by , 
    ca.created_date , 
    ca.modified_by, 
    ca.modified_date,     
    '' AS activitys_code, 
    '' AS progress, 
    0 AS succeed, 
    0 AS progress_total,
    ca.status,
    mc.name AS customer_name,
    '' AS product_code,
    ca.call_status, 
    ca.call_action, 
    mc.phone AS customer_phone, 
    mc.province AS customer_province, 
    ca.remark,
    cl.contact_result2,
    cl.created_by AS contact_created_by,
    cl.created_at AS contact_created_at
FROM 
    crm_activitys ca
INNER JOIN 
    mas_customers mc ON mc.guid = ca.customer_code
LEFT JOIN 
    (SELECT customer_id, contact_result2, created_by, created_at,
            ROW_NUMBER() OVER (PARTITION BY customer_id ORDER BY created_at DESC) AS rn
     FROM crm_contact_logs) cl ON cl.customer_id = ca.customer_code AND cl.rn = 1
                    WHERE ca.branch_code = @branch_code
                    AND ca.assign_work = @assign_work
                    AND mc.name LIKE CONCAT('%', @value1, '%') 
                    ORDER BY ca.startdate";
                }



                using (var cmd = new MySqlCommand(query, connection))
                {
                    var value1 = searchCriteria.value1 ?? "";
                    cmd.Parameters.AddWithValue("@value1", value1);

                    var branch_code = searchCriteria.branch_code ?? "";
                    cmd.Parameters.AddWithValue("@branch_code", branch_code);

                    var assign_work = searchCriteria.assign_work ?? "";
                    cmd.Parameters.AddWithValue("@assign_work", assign_work);


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            JobDataList d = new JobDataList();

                            d.guid = reader["guid"].ToString();
                            d.customer_code = reader["customer_code"].ToString();
                            d.branch_code = reader["branch_code"].ToString();
                            d.touch_point = reader["touch_point"].ToString();

                            d.customer_name = reader["customer_name"].ToString();
                            d.customer_phone = reader["customer_phone"].ToString();
                            d.customer_province = reader["customer_province"].ToString();

                            d.product_code = reader["product_code"].ToString();

                            d.description = reader["description"].ToString();
                            d.startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("startdate"));
                            d.duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate"));
                            d.reminder_duedate = reader.IsDBNull(reader.GetOrdinal("reminder_duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("reminder_duedate"));
                            d.assign_work = reader["assign_work"].ToString();
                            d.assign_work_type = reader["assign_work_type"].ToString();
                            d.allowagent = reader.IsDBNull(reader.GetOrdinal("allowagent")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("allowagent"));


                            d.created_by = reader["created_by"].ToString();
                            d.created_date = reader.GetDateTime(reader.GetOrdinal("created_date"));
                            d.modified_by = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? null : reader["modified_by"].ToString();
                            d.modified_date = reader.IsDBNull(reader.GetOrdinal("modified_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("modified_date"));

                            d.act_status = reader["status"].ToString();
                            d.call_status = reader["call_status"].ToString();
                            d.call_action = reader["call_action"].ToString();

                            d.activitys_code = reader["activitys_code"].ToString();
                            d.progress = reader["activitys_code"].ToString();
                            d.succeed = reader.IsDBNull(reader.GetOrdinal("succeed")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("succeed"));
                            d.progress_total = reader.IsDBNull(reader.GetOrdinal("progress_total")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("progress_total"));
                            d.remark = reader["remark"].ToString();
                            d.contact_result2 = reader["contact_result2"].ToString();
                            d.contact_created_by = reader["contact_created_by"].ToString();
                            d.contact_created_at = reader.IsDBNull(reader.GetOrdinal("contact_created_at")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("contact_created_at"));

                            activitys.Add(d);
                        };

                    }
                }
            }
            return Ok(activitys);
        }



        public class SearchCriteria
        {
            public DateTime? fdate { get; set; }
            public DateTime? ldate { get; set; }
            public string? branch_code { get; set; }
            public string? assign_work { get; set; }
            public string? filtering_type { get; set; }

            public string? filteringKey { get; set; }
            public string? filteringValue { get; set; }


        }


        public class SearchCriteriaV2
        {
            public DateTime? fdate { get; set; }
            public DateTime? ldate { get; set; }
            public string? branch_code { get; set; }
            public string? assign_work { get; set; }
            public string? filtering_type { get; set; }
            public string? filteringValue2 { get; set; }

            public string? filteringKey { get; set; }
            public string? filteringValue { get; set; }

            public int PageNumber { get; set; } = 1; // ค่าเริ่มต้นคือ หน้าแรก
            public int PageSize { get; set; } = 10;  // ค่าเริ่มต้นคือ 10 รายการต่อหน้า
        }



        public class SearchCriteriaByKey
        {
            public string? key1 { get; set; }
            public string? value1 { get; set; }
            public string? branch_code { get; set; }
            public string? assign_work { get; set; }
        }



        public class SearchCriteriaByID
        {
            public string? guid { get; set; }
            public string? branch_code { get; set; }
        }

    }
}
