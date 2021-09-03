using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanBag.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TRANSACTION_ID { get; set; }
        public DateTime? DATE { get; set; }
        public string PAY_REQUEST_ID { get; set; }
        public int AMOUNT { get; set; }
        public string REFERENCE { get; set; }
        public string TRANSACTION_STATUS { get; set; }
        public string RESULT_DESC { get; set; }
        public string CUSTOMER_EMAIL_ADDRESS { get; set;}
        public Guid TENANT_ID { get; set; }
        public string SUBSCRIPTION { get; set; }
        
    }
}