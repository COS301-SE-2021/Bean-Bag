﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class Transactions
    {
        [Key] 
        public string TransactionId { get; set; }
        public string TenantId { get; set; }
        public string PaymentRequestId { get; set; }
        public string Reference { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Amount { get; set; }
        
    }
}