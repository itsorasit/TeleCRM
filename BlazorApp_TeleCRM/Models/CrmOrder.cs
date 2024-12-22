namespace BlazorApp_TeleCRM.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CrmOrder
    {
        public string? guid { get; set; }

        public string? customer_code { get; set; }

        public string? branch_code { get; set; }

        public string? order_no { get; set; }

        public DateTime? order_date { get; set; }

        public string? channel { get; set; }

        public string? tracking_no { get; set; }

        public decimal? amount { get; set; }

        public string? payment_type { get; set; }

        public string? product_codes { get; set; }

        public string? product_names { get; set; }

        public string? product_qtys { get; set; }

        public string? modified_by { get; set; }

        public DateTime? modified_date { get; set; }

        public string? seller { get; set; }
    }

}
