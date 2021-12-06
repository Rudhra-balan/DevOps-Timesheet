using System.ComponentModel.DataAnnotations;

namespace HI.DevOps.DomainCore.Models.Login
{
    public class LoginUserViewModel : BaseViewModel
    {
        #region Public Members

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string DevOpsUserId { get; set; }

        #endregion
    }
}