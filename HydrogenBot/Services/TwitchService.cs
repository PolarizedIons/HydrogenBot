using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using HydrogenBot.Extentions;
using HydrogenBot.Models;
using HydrogenBot.Models.Twitch;
using Microsoft.Extensions.Configuration;

namespace HydrogenBot.Services
{
    public class TwitchService : ISingletonDiService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public TwitchService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v5+json"));
            _httpClient.DefaultRequestHeaders.Add("Client-ID", _config["Twitch:ClientId"]);
        }

        public async Task<TwitchUserDto?> FromName(string loginName)
        {
            var res = await _httpClient.GetAsync($"https://api.twitch.tv/kraken/users?login={loginName}");
            var json = await res.Content.ReadAsStringAsync();
            var userSearchDto = JsonSerializer.Deserialize<TwitchUserSearchDto>(json);
            return userSearchDto.Users.Any() ? userSearchDto.Users.First() : null;
        }

        public async Task<TwitchUserDto?> FromId(uint channelId)
        {
            var res = await _httpClient.GetAsync($"https://api.twitch.tv/kraken/users/{channelId}");
            var json = await res.Content.ReadAsStringAsync();
            var userDto = JsonSerializer.Deserialize<TwitchUserDto>(json);
            return userDto;
        }

        public async Task<IEnumerable<TwitchStreamInfo>> BatchStreamInfo(IEnumerable<uint> channelIds)
        {
            var batches = channelIds
                .Batch(100)
                .Select(async batchChannelIds =>
                {
                    var res = await _httpClient.GetAsync($"https://api.twitch.tv/kraken/streams?channel={string.Join(",", batchChannelIds)}");
                    var json = await res.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TwitchSteamsInfoResponse>(json);
                });

            var completedBatches = await Task.WhenAll(batches);
            return completedBatches
                .Aggregate(new TwitchStreamInfo[0] as IEnumerable<TwitchStreamInfo>, 
                    (current, completedBatch) => current.Concat(completedBatch.Steams)
                );
        }
    }
}
