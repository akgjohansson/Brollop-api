using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace API_brollop.Client
{
    public interface ISpotifyApi
    {
        Task<HttpResponseMessage> Get(string searchString);
    }
}