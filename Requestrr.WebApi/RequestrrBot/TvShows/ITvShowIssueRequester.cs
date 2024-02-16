
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.TvShows
{
    public interface ITvShowIssueRequester
    {
        Task<bool> SubmitTvShowIssueAsync(TvShowRequest request, int thTvDbId, string issueName, string issueDescription);
    }
}