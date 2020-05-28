using System;
using System.ComponentModel.DataAnnotations;

namespace Bookstore_API.DTOs
{
    public class UserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "password shoud be {2} to {1} characters", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
