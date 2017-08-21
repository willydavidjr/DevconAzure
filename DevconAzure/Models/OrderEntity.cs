using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace DevconAzure.Models
{
    public class OrderEntity : TableEntity
    {
        public OrderEntity()
        {

        }

        public OrderEntity(int orderid, string email)
        {
            this.RowKey = orderid.ToString();
            this.PartitionKey = email;
        }

        public int OrderId { get; set; }
        [Required(ErrorMessage = "Order name is required.")]

        [Display(Name ="Order")]
        public string OrderName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Display(Name = "Order Path")]
        public string OrderPath { get; set; }

    }
}