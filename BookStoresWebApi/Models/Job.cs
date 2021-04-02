using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace BookStoresWebApi.Models
{
    [Table("Job")]
    public partial class Job
    {
        [Key]
        [Column("job_id")]
        public short JobId { get; set; }
        [Required]
        [Column("job_desc")]
        [StringLength(50)]
        public string JobDesc { get; set; }
    }
}
