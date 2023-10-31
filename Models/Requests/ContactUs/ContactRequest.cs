

using System.ComponentModel.DataAnnotations;

namespace Models.Requests.ContactUs
{
    public class ContactRequest
    {
        public long? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Message { get; set; }

        public bool? IsDataStored { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
    }
}
