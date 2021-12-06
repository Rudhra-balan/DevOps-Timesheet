namespace HI.DevOps.DomainCore.Models.Error
{
    public static class ErrorViewModelExtensions
    {
        public static bool HasError(this ErrorViewModel errorViewModel)
        {
            if (errorViewModel == null) return false;
            return errorViewModel.Id > 0;
        }
    }
}