using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace BookStoresWebApi.Models
{
    [Table("Author")]
    public partial class Author
    {
        public Author()
        {
            BookAuthors = new HashSet<BookAuthor>();
        }

        [Key]
        [Column("author_id")]
        public int AuthorId { get; set; }
        [Required]
        [Column("last_name")]
        [StringLength(40)]
        public string LastName { get; set; }
        [Required]
        [Column("first_name")]
        [StringLength(20)]
        public string FirstName { get; set; }
        [Required]
        [Column("phone")]
        [StringLength(12)]
        public string Phone { get; set; }
        [Column("address")]
        [StringLength(40)]
        public string Address { get; set; }
        [Column("city")]
        [StringLength(20)]
        public string City { get; set; }
        [Column("state")]
        [StringLength(2)]
        public string State { get; set; }
        [Column("zip")]
        [StringLength(5)]
        public string Zip { get; set; }
        [Column("email_address")]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [InverseProperty(nameof(BookAuthor.Author))]
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
    }
}
