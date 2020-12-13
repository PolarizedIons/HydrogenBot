using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using HydrogenBot.Models;
using HydrogenBot.Models.Twitter;
using Microsoft.Extensions.Configuration;

namespace HydrogenBot.Services
{
    public class TwitterService : ISingletonDiService
    {
        private const string FetchUrl = "https://api.twitter.com/2/tweets/search/recent?query={query}&expansions=author_id&max_results=100";
        private const string QueryTemplate = "from: {username}";
        private const string QueryJoin = " OR ";
        private const int QueryMaxLength = 512;

        private readonly HttpClient _httpClient;
        private string _lastRequestedId = "";

        public TwitterService(IConfiguration config)
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {config["Twitter:BearerToken"]}");
        }

        private IEnumerable<string> BatchUsernames(IEnumerable<string> usernames)
        {
            var queryLists = new List<string>();
            var currentQuery = "";
            foreach (var username in usernames)
            {
                var attemptedQuery = "";
                if (!string.IsNullOrEmpty(currentQuery))
                {
                    attemptedQuery += QueryJoin;
                }

                attemptedQuery += QueryTemplate.Replace("{username}", username);

                if (currentQuery.Length + attemptedQuery.Length < QueryMaxLength)
                {
                    currentQuery += attemptedQuery;
                }
                else
                {
                    Console.WriteLine($"new query {currentQuery}");
                    queryLists.Add(currentQuery);
                    currentQuery = "";
                }
            }
            queryLists.Add(currentQuery);

            return queryLists;
        }

        public async Task<Dictionary<string, IList<TweetSearchResultData>>> BatchUserRequest(IEnumerable<string> usernames)
        {
            if (!usernames.Any())
            {
                return new Dictionary<string, IList<TweetSearchResultData>>();
            }

            var batches = BatchUsernames(usernames)
                .Select(async query =>
                {
                    var url = FetchUrl
                        .Replace("{query}", query);
                    if (!string.IsNullOrEmpty(_lastRequestedId))
                    {
                        url += $"&since_id={_lastRequestedId}";
                    }

                    var res = await _httpClient.GetAsync(url);
                    Console.WriteLine($"request {url}");
                    var json = await res.Content.ReadAsStringAsync();
                    Console.WriteLine($"got {json}");
                    return JsonSerializer.Deserialize<TweetSearchResultDto>(json);
            });

            var completedBatches = await Task.WhenAll(batches);
            var tweetDict = new Dictionary<string, IList<TweetSearchResultData>>();
            var idDict = new Dictionary<string, string>();

            foreach (var batch in completedBatches)
            {
                if (batch.Meta.ResultCount == 0)
                {
                    continue;
                }

                if (batch.Includes.Users != null)
                {
                    foreach (var user in batch.Includes.Users)
                    {
                        tweetDict.Add(user.Username, new List<TweetSearchResultData>());
                        idDict.Add(user.Id, user.Username);
                    }
                }

                if (batch.Data != null)
                {
                    foreach (var tweet in batch.Data)
                    {
                        var username = idDict[tweet.AuthorId];
                        if (tweet.Text.StartsWith("@"))
                        {
                            continue;
                        }
                        
                        tweetDict[username].Add(tweet);
                    }
                }

                if (batch.Meta.NewestId != null)
                {
                    _lastRequestedId = batch.Meta.NewestId;
                }
            }

            return tweetDict;
        }
    }
}
