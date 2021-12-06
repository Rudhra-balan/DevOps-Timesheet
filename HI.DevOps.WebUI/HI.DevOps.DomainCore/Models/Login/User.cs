using System.ComponentModel.DataAnnotations;
using HI.DevOps.DomainCore.Enumeration.UserRoleEnum;

namespace HI.DevOps.DomainCore.Models.Login
{
    public class User : BaseViewModel

    {
        public string UserIdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; }

        public UserRoleEnum UserRole { get; set; }
        public bool IsActive { get; set; }
    }
}