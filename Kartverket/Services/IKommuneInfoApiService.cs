using Kartverket.API_Models;

namespace Kartverket.Services
{
    public interface IKommuneInfoApiService
    {
        Task<KommuneInfo> GetKommuneInfoAsync(string kommuneNr);
    }
}
