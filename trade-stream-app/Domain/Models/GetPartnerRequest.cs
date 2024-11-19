using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class GetPartnerRequest
    {
        [Required]
        public int Age { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
