using API_brollop.Common;
using API_brollop.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace API_brollop.Client
{
    public class SpotifyApi : ISpotifyApi
    {
        private Uri _baseUri;
        private string _clientID;
        private string _clientSecret;
        private HttpClient _client;
        private Token _token;
        private byte[] _clientByteArray;

        public SpotifyApi()
        {
            _baseUri = new Uri("https://api.spotify.com");
            _clientID = "a39a0a48191c4f76a171a64aab87a912";
            _clientSecret = "e3c42cb307304e6f9c1e7c047a9fbde4";
            _client = new HttpClient();
            _clientByteArray = new UTF8Encoding().GetBytes($"{_clientID}:{_clientSecret}");
            GetToken();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token.Access_token}");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
        }


        public async Task<HttpResponseMessage> Get(string searchString)
        {
            HttpResponseMessage response;
            for (int i = 0; i < 2; i++)
            {
                var uri = _baseUri.Append($"v1/search?q={searchString}&type=track%2Cartist&market=SE");
                response = _client.GetAsync(uri).Result;
                if (response.StatusCode != HttpStatusCode.OK)
                    RefreshToken();
                else
                    return response;
            }
            return null;
        }

        private void RefreshToken()
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Athorization",
                    $"Basic {Convert.ToBase64String(_clientByteArray)}");
                var col = new NameValueCollection
                {
                    {"grant_type", "refresh_token" },
                    {"refresh_token", _token.Access_token }
                };
                var data = webClient.UploadValues("https//accounts.spotify.com/api/token", "POST", col);
                _token = JsonConvert.DeserializeObject<Token>(Encoding.UTF8.GetString(data));
            }
        }

        private void GetToken()
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Authorization",
                   "Basic " + Convert.ToBase64String(_clientByteArray));
                var col = new NameValueCollection
                {
                    {"grant_type", "client_credentials"},
                    {"scope", ""}
                };
                byte[] data = webClient.UploadValues("https://accounts.spotify.com/api/token", "POST", col);
                _token = JsonConvert.DeserializeObject<Token>(Encoding.UTF8.GetString(data));
            }
        }
    }
}