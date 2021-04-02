using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace BookStoresWebApi.Models
{
    [Table("Book")]
    public partial class Book
    {
        public Book()
        {
            BookAuthors = new HashSet<BookAuthor>();
            Sales = new HashSet<Sale>();
        }

        [Key]
        [Column("book_id")]
        public int BookId { get; set; }
        [Required]
        [Column("title")]
        [StringLength(80)]
        public string Title { get; set; }
        [Required]
        [Column("type")]
        [StringLength(12)]
        public string Type { get; set; }
        [Column("pub_id")]
        public int PubId { get; set; }
        [Column("price", TypeName = "money")]
        public decimal? Price { get; set; }
        [Column("advance", TypeName = "money")]
        public decimal? Advance { get; set; }
        [Column("royalty")]
        public int? Royalty { get; set; }
        [Column("ytd_sales")]
        public int? YtdSales { get; set; }
        [Column("notes")]
        [StringLength(200)]
        public string Notes { get; set; }
        [Column("published_date", TypeName = "datetime")]
        public DateTime PublishedDate { get; set; }

        [ForeignKey(nameof(PubId))]
        [InverseProperty(nameof(Publisher.Books))]
        public virtual Publisher Pub { get; set; }
        [InverseProperty(nameof(BookAuthor.Book))]
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
        [InverseProperty(nameof(Sale.Book))]
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
