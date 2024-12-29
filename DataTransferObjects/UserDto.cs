using System.ComponentModel.DataAnnotations;

namespace Home_accounting.DataTransferObjects
{
    public class UserDto
    {
        [Required, MinLength(3)] public string? Name { get; set; }
        [Required, MinLength(3)] public string? Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
