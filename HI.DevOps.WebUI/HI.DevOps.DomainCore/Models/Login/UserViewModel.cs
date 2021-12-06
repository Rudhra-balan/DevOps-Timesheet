using HI.DevOps.DomainCore.Models.Error;

namespace HI.DevOps.DomainCore.Models.Login
{
    public class UserViewModel : BaseViewModel
    {
        public UserViewModel()
        {
            Errors = new ErrorViewModel();
            AccessToken = new AccessToken();
        }


        #region Public Members

        public string UserId { get; set; }
        public User User { get; set; }
        public AccessToken AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public ErrorViewModel Errors { get; set; }

        #endregion
    }
}