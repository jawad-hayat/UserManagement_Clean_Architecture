using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ContactUs
    {
        [Key]
        public long Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [EmailAddress]
        [Column(TypeName = "nvarchar(50)")]
        public string EmailAddress { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Message { get; set; }

        public bool? IsDataStored { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
    }
}
