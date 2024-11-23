using BlazorApp_TeleCRM.Models;
using DocumentFormat.OpenXml.Office.Word;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace BlazorApp_TeleCRM.Helper
{
    public class GetGenerateID
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public GetGenerateID(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("DefaultConnection is null or empty. Check appsettings.json or configuration setup.");
            }

            _connectionString = connectionString;
        }

        public List<MasDocumentControl> GenId(string document_type , string company_code , string branch_code )
        {
            List<MasDocumentControl> result = new List<MasDocumentControl>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT 
                document_type, branch_code, company_code, string_format, year_lang, year_digit,
                prefix, `year`, `month`, `number`, document_no, description, module, record_status, created_by, 
                created_time, updated_by, updated_time, month_no 
                FROM mas_document_controls WHERE document_type = @DocumentType 
                AND company_code = @CompanyCode AND branch_code = @branch_code LIMIT 1";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DocumentType", document_type);
                    command.Parameters.AddWithValue("@CompanyCode", company_code);
                    command.Parameters.AddWithValue("@branch_code", branch_code);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            MasDocumentControl id = new MasDocumentControl
                            {
                                document_type = reader["document_type"].ToString(),
                                company_code = reader["company_code"].ToString(),
                                year = reader["year"]?.ToString(),
                                year_digit = reader["year_digit"] != DBNull.Value ? Convert.ToInt32(reader["year_digit"]) : 0,
                                year_lang = reader["year_lang"]?.ToString() ?? "en",
                                month = reader["month"]?.ToString(),
                                month_no = reader["month_no"]?.ToString(),
                                number = Convert.ToInt32(reader["number"]),
                                prefix = reader["prefix"]?.ToString(),
                                string_format = reader["string_format"]?.ToString()
                            };

                            bool hasyear = !string.IsNullOrEmpty(id.year);
                            int yeardigit = id.year_digit > 0 ? id.year_digit.Value : 2;
                            bool hasmonth = id.month == "Yes";
                            string lang = id.year_lang;
                            string prepairYearMonth = "";
                            bool isNewyear = false;

                            if (hasyear)
                            {
                                int y1 = lang == "th" ? ConvertDateToThaiYear(DateTime.Now) : ConvertDateToEngYear(DateTime.Now);

                                if (!string.IsNullOrEmpty(id.year) && Convert.ToInt32(id.year) < y1)
                                {
                                    isNewyear = true;
                                    id.number = 1;
                                    id.year = y1.ToString();
                                }
                                else if (string.IsNullOrEmpty(id.year))
                                {
                                    id.year = y1.ToString();
                                }

                                prepairYearMonth += id.year.Substring(id.year.Length - yeardigit);
                            }

                            if (hasmonth)
                            {
                                int m1 = DateTime.Now.Month;
                                if (!isNewyear)
                                {
                                    if (id.month == "Yes" && Convert.ToInt32(id.month_no) < m1)
                                    {
                                        id.number = 1;
                                        id.month_no = m1.ToString();
                                    }

                                    prepairYearMonth += Convert.ToInt32(id.month_no).ToString("00");
                                }
                                else
                                {
                                    prepairYearMonth += "01";
                                    id.month_no = "01";
                                }
                            }

                            id.document_no = id.prefix + prepairYearMonth + id.number.Value.ToString(id.string_format);

                            MasDocumentControl nextId = new MasDocumentControl
                            {
                                document_type = id.document_type,
                                string_format = id.string_format,
                                prefix = id.prefix,
                                year = id.year,
                                month = id.month,
                                month_no = id.month_no,
                                number = id.number + 1,
                                document_no = id.prefix + prepairYearMonth + (id.number + 1).Value.ToString(id.string_format)
                            };

                            result.Add(id);
                            result.Add(nextId);


                            // Close the reader before performing another operation
                            reader.Close();

                            //  string updateQuery = "UPDATE mas_document_controls SET number = @Number, year = @Year, month_no = @MonthNo WHERE document_type = @DocumentType AND company_code = @CompanyCode";
                            string updateQuery = @"UPDATE mas_document_controls 
                             SET number = @number,document_no=@document_no  
                             WHERE document_type = @DocumentType 
                             AND company_code = @CompanyCode 
                             AND branch_code = @branch_code ";


                            using (var updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@Number", nextId.number);
                                updateCommand.Parameters.AddWithValue("@document_no", nextId.document_no);
                                updateCommand.Parameters.AddWithValue("@Year", id.year);
                                updateCommand.Parameters.AddWithValue("@MonthNo", id.month_no);
                                updateCommand.Parameters.AddWithValue("@DocumentType", id.document_type);
                                updateCommand.Parameters.AddWithValue("@CompanyCode", id.company_code);
                                updateCommand.Parameters.AddWithValue("@branch_code", branch_code);

                                updateCommand.ExecuteNonQuery();
                            }

                            return result;
                        }
                    }
                }
            }

            return result;
        }

        public async Task<string> GenIdCustomer(string document_type, string company_code, string branch_code , string phone)
        {

            List<MasDocumentControl> result = new List<MasDocumentControl>();


            using (var connection_cus = new MySqlConnection(_connectionString))
            {
                await connection_cus.OpenAsync();

                // ดึงข้อมูล GUID ที่มีอยู่แล้ว
                string query_cus = @"SELECT code FROM mas_customers WHERE phone = @Phone AND branch_code = @BranchCode LIMIT 1";

                using (var command_cus = new MySqlCommand(query_cus, connection_cus))
                {
                    command_cus.Parameters.AddWithValue("@Phone", phone);
                    command_cus.Parameters.AddWithValue("@BranchCode", branch_code);

                    var existingGuid = await command_cus.ExecuteScalarAsync();

                    if (existingGuid != null && existingGuid != DBNull.Value)
                    {
                        // คืนค่าที่มีอยู่แล้ว
                        return existingGuid.ToString();
                    }
                    else
                    {
                        using (var connection = new MySqlConnection(_connectionString))
                        {
                            connection.Open();

                            string query = @"SELECT 
                document_type, branch_code, company_code, string_format, year_lang, year_digit,
                prefix, `year`, `month`, `number`, document_no, description, module, record_status, created_by, 
                created_time, updated_by, updated_time, month_no 
                FROM mas_document_controls WHERE document_type = @DocumentType 
                AND company_code = @CompanyCode AND branch_code = @branch_code LIMIT 1";

                            using (var command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@DocumentType", document_type);
                                command.Parameters.AddWithValue("@CompanyCode", company_code);
                                command.Parameters.AddWithValue("@branch_code", branch_code);

                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        MasDocumentControl id = new MasDocumentControl
                                        {
                                            document_type = reader["document_type"].ToString(),
                                            company_code = reader["company_code"].ToString(),
                                            year = reader["year"]?.ToString(),
                                            year_digit = reader["year_digit"] != DBNull.Value ? Convert.ToInt32(reader["year_digit"]) : 0,
                                            year_lang = reader["year_lang"]?.ToString() ?? "en",
                                            month = reader["month"]?.ToString(),
                                            month_no = reader["month_no"]?.ToString(),
                                            number = Convert.ToInt32(reader["number"]),
                                            prefix = reader["prefix"]?.ToString(),
                                            string_format = reader["string_format"]?.ToString()
                                        };

                                        bool hasyear = !string.IsNullOrEmpty(id.year);
                                        int yeardigit = id.year_digit > 0 ? id.year_digit.Value : 2;
                                        bool hasmonth = id.month == "Yes";
                                        string lang = id.year_lang;
                                        string prepairYearMonth = "";
                                        bool isNewyear = false;

                                        if (hasyear)
                                        {
                                            int y1 = lang == "th" ? ConvertDateToThaiYear(DateTime.Now) : ConvertDateToEngYear(DateTime.Now);

                                            if (!string.IsNullOrEmpty(id.year) && Convert.ToInt32(id.year) < y1)
                                            {
                                                isNewyear = true;
                                                id.number = 1;
                                                id.year = y1.ToString();
                                            }
                                            else if (string.IsNullOrEmpty(id.year))
                                            {
                                                id.year = y1.ToString();
                                            }

                                            prepairYearMonth += id.year.Substring(id.year.Length - yeardigit);
                                        }

                                        if (hasmonth)
                                        {
                                            int m1 = DateTime.Now.Month;
                                            if (!isNewyear)
                                            {
                                                if (id.month == "Yes" && Convert.ToInt32(id.month_no) < m1)
                                                {
                                                    id.number = 1;
                                                    id.month_no = m1.ToString();
                                                }

                                                prepairYearMonth += Convert.ToInt32(id.month_no).ToString("00");
                                            }
                                            else
                                            {
                                                prepairYearMonth += "01";
                                                id.month_no = "01";
                                            }
                                        }

                                        id.document_no = id.prefix + prepairYearMonth + id.number.Value.ToString(id.string_format);

                                        MasDocumentControl nextId = new MasDocumentControl
                                        {
                                            document_type = id.document_type,
                                            string_format = id.string_format,
                                            prefix = id.prefix,
                                            year = id.year,
                                            month = id.month,
                                            month_no = id.month_no,
                                            number = id.number + 1,
                                            document_no = id.prefix + prepairYearMonth + (id.number + 1).Value.ToString(id.string_format)
                                        };

                                        result.Add(id);
                                        result.Add(nextId);


                                        // Close the reader before performing another operation
                                        reader.Close();

                                        //  string updateQuery = "UPDATE mas_document_controls SET number = @Number, year = @Year, month_no = @MonthNo WHERE document_type = @DocumentType AND company_code = @CompanyCode";
                                        string updateQuery = @"UPDATE mas_document_controls 
                             SET number = @number,document_no=@document_no  
                             WHERE document_type = @DocumentType 
                             AND company_code = @CompanyCode 
                             AND branch_code = @branch_code ";


                                        using (var updateCommand = new MySqlCommand(updateQuery, connection))
                                        {
                                            updateCommand.Parameters.AddWithValue("@Number", nextId.number);
                                            updateCommand.Parameters.AddWithValue("@document_no", nextId.document_no);
                                            updateCommand.Parameters.AddWithValue("@Year", id.year);
                                            updateCommand.Parameters.AddWithValue("@MonthNo", id.month_no);
                                            updateCommand.Parameters.AddWithValue("@DocumentType", id.document_type);
                                            updateCommand.Parameters.AddWithValue("@CompanyCode", id.company_code);
                                            updateCommand.Parameters.AddWithValue("@branch_code", branch_code);

                                            updateCommand.ExecuteNonQuery();
                                        }

                                        return branch_code+"-" + id.document_no;
                                    }
                                }
                            }
                        }
                        return "";
                    }
                }
            }
        }



        public int ConvertDateToThaiYear(DateTime getDate)
        {
            return getDate.Year < 2500 ? getDate.Year + 543 : getDate.Year;
        }

        public int ConvertDateToEngYear(DateTime getDate)
        {
            return getDate.Year > 2500 ? getDate.Year - 543 : getDate.Year;
        }
    }
}
