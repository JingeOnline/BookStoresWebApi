using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace BookStoresWebApi.Models
{
    [Table("Store")]
    public partial class Store
    {
        public Store()
        {
            Sales = new HashSet<Sale>();
        }

        [Key]
        [Column("store_id")]
        [StringLength(4)]
        public string StoreId { get; set; }
        [Column("store_name")]
        [StringLength(40)]
        public string StoreName { get; set; }
        [Column("store_address")]
        [StringLength(40)]
        public string StoreAddress { get; set; }
        [Column("city")]
        [StringLength(20)]
        public string City { get; set; }
        [Column("state")]
        [StringLength(2)]
        public string State { get; set; }
        [Column("zip")]
        [StringLength(5)]
        public string Zip { get; set; }

        [InverseProperty(nameof(Sale.Store))]
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
