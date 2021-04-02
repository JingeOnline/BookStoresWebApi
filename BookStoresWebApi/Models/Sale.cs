using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace BookStoresWebApi.Models
{
    [Table("Sale")]
    public partial class Sale
    {
        [Key]
        [Column("sale_id")]
        public int SaleId { get; set; }
        [Required]
        [Column("store_id")]
        [StringLength(4)]
        public string StoreId { get; set; }
        [Required]
        [Column("order_num")]
        [StringLength(20)]
        public string OrderNum { get; set; }
        [Column("order_date", TypeName = "datetime")]
        public DateTime OrderDate { get; set; }
        [Column("quantity")]
        public short Quantity { get; set; }
        [Required]
        [Column("pay_terms")]
        [StringLength(12)]
        public string PayTerms { get; set; }
        [Column("book_id")]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        [InverseProperty("Sales")]
        public virtual Book Book { get; set; }
        [ForeignKey(nameof(StoreId))]
        [InverseProperty("Sales")]
        public virtual Store Store { get; set; }
    }
}
