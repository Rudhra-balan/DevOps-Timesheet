using HI.DevOps.DomainCore.Models.Response;

namespace HI.DevOps.Application.Common.Interfaces.IRequestBroker
{
    public interface IRequestBrokerService
    {
        public T SendRequest<T>(string url, bool requiresAuthentication);

        public WebClientResponse PostRequest<T>(string postUrlWithDataObject, object dataObject);

        public WebClientResponse PostRequest<T>(string postUrl);
    }
}