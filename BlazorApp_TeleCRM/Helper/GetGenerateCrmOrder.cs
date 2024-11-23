using BlazorApp_TeleCRM.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace BlazorApp_TeleCRM.Helper
{
    public class GetGenerateCrmOrder
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public GetGenerateCrmOrder(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("DefaultConnection is null or empty. Check appsettings.json or configuration setup.");
            }

            _connectionString = connectionString;
        }

        public async Task<bool> Upload(CrmOrder data)
        {
            bool result = false;

            // ใช้ MySqlConnection สำหรับเชื่อมต่อกับฐานข้อมูล
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL สำหรับตรวจสอบว่ามีข้อมูลอยู่แล้วหรือไม่
                    string checkQuery = @"
                SELECT COUNT(*) 
                FROM crm_orders 
                WHERE customer_code = @customer_code 
                  AND branch_code = @branch_code 
                  AND order_no = @order_no 
                  AND order_date = @order_date 
                  AND channel = @channel";

                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        // เพิ่มพารามิเตอร์เพื่อป้องกัน SQL Injection
                        checkCommand.Parameters.AddWithValue("@customer_code", data.customer_code);
                        checkCommand.Parameters.AddWithValue("@branch_code", data.branch_code);
                        checkCommand.Parameters.AddWithValue("@order_no", data.order_no);
                        checkCommand.Parameters.AddWithValue("@order_date", data.order_date);
                        checkCommand.Parameters.AddWithValue("@channel", data.channel);

                        // ตรวจสอบผลลัพธ์
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count == 0) // ถ้ายังไม่มีข้อมูล
                        {
                            // SQL สำหรับเพิ่มข้อมูลใหม่
                            string insertQuery = @"
                        INSERT INTO crm_orders 
                        (guid, customer_code, branch_code, order_no, order_date, channel, 
                         tracking_no, amount, payment_type, product_codes, product_names, 
                         product_qtys, modified_by, modified_date) 
                        VALUES 
                        (@guid, @customer_code, @branch_code, @order_no, @order_date, @channel, 
                         @tracking_no, @amount, @payment_type, @product_codes, @product_names, 
                         @product_qtys, @modified_by, @modified_date)";

                            using (var insertCommand = new MySqlCommand(insertQuery, connection))
                            {
                                // เพิ่มพารามิเตอร์สำหรับ Insert
                                insertCommand.Parameters.AddWithValue("@guid", Guid.NewGuid().ToString());
                                insertCommand.Parameters.AddWithValue("@customer_code", data.customer_code);
                                insertCommand.Parameters.AddWithValue("@branch_code", data.branch_code);
                                insertCommand.Parameters.AddWithValue("@order_no", data.order_no);
                                insertCommand.Parameters.AddWithValue("@order_date", data.order_date);
                                insertCommand.Parameters.AddWithValue("@channel", data.channel);
                                insertCommand.Parameters.AddWithValue("@tracking_no", data.tracking_no);
                                insertCommand.Parameters.AddWithValue("@amount", data.amount);
                                insertCommand.Parameters.AddWithValue("@payment_type", data.payment_type);
                                insertCommand.Parameters.AddWithValue("@product_codes", data.product_codes);
                                insertCommand.Parameters.AddWithValue("@product_names", data.product_names);
                                insertCommand.Parameters.AddWithValue("@product_qtys", data.product_qtys);
                                insertCommand.Parameters.AddWithValue("@modified_by", data.modified_by);
                                insertCommand.Parameters.AddWithValue("@modified_date", data.modified_date);

                                // Execute คำสั่ง Insert
                                int rowsAffected = insertCommand.ExecuteNonQuery();

                                // ถ้า Insert สำเร็จ
                                if (rowsAffected > 0)
                                {
                                    result = true;
                                }
                            }
                        }
                       
                    }
                }
                catch
                {
                    //  Console.WriteLine($"Error: {ex.Message}");
                    connection.Close();
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }


    }
}
