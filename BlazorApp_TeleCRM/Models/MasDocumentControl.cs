namespace BlazorApp_TeleCRM.Models
{
    public class MasDocumentControl
    {
        public string? document_type { get; set; } // Corresponds to [document_type]
        public string? branch_code { get; set; } // Corresponds to [branch_code]
        public string? company_code { get; set; } // Corresponds to [company_code]
        public string? string_format { get; set; } // Corresponds to [string_format]
        public string? year_lang { get; set; } = "en"; // Default value corresponds to [year_lang]
        public int? year_digit { get; set; } = 0; // Default value corresponds to [year_digit]
        public string? prefix { get; set; } // Corresponds to [prefix]
        public string? year { get; set; } // Corresponds to [year]
        public string? month { get; set; } // Corresponds to [month]
        public int? number { get; set; } // Corresponds to [number]
        public string? document_no { get; set; } // Corresponds to [document_no]
        public string? description { get; set; } // Corresponds to [description]
        public string? module { get; set; } // Corresponds to [module]
        public bool? record_status { get; set; } = true; // Default value corresponds to [record_status]
        public string? created_by { get; set; } // Corresponds to [created_by]
        public DateTime? created_time { get; set; } // Corresponds to [created_time]
        public string? updated_by { get; set; } // Corresponds to [updated_by]
        public DateTime? updated_time { get; set; } // Corresponds to [updated_time]
        public string? month_no { get; set; } // Corresponds to [month_no]
    }
}
