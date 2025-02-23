using System.ComponentModel.DataAnnotations;

namespace RegularExpressionTask3.Models
{
    public class PhoneNumber
    {
        public int Id { get; set; }

        [Required]
        [Phone]
        public string Number { get; set; }

        public string OwnerName { get; set; }
    }
}
