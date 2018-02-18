using API_brollop.Client;
using API_brollop.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Net.Http.Formatting;
using System.Net.Http;

namespace API_brollop.Controllers
{
    [RoutePrefix("spotify"), EnableCors(origins: "http://localhost:9000", headers: "*", methods: "*")]
    public class SpotifyController : ApiController
    {
        private ISpotifyApi _api;
        public SpotifyController(ISpotifyApi spotifyApi)
        {
            _api = spotifyApi;
        }

        [Route("search")]
        public async Task<IHttpActionResult> Get(string s)
        {
            var response = await _api.Get(s);
            if (response.StatusCode != HttpStatusCode.OK)
                return ResponseMessage(Request.CreateResponse(response.StatusCode, response.Content));
            var result = await response.Content.ReadAsStreamAsync();
            var data = JsonConvert.DeserializeObject<SpotifyDto>(await response.Content.ReadAsStringAsync());

            var spotify = new Spotify { Tracks = new List<Track>() };
            IEnumerable<Dictionary<string, dynamic>> jsonTracks = JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, dynamic>>>(data.Tracks["items"].ToString());
            foreach (var item in jsonTracks)
            {
                spotify.Tracks.Add(new Track
                {
                    Href = item["href"],
                    Name = item["name"],
                    Id = item["id"],
                    Duration_ms = Convert.ToInt32(item["duration_ms"]),
                    Type = item["type"],
                    Album = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(item["album"].ToString())["name"]
                });
            }

            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(spotify);
            else
                return BadRequest(response.Content.ToString());
        }
    }
}