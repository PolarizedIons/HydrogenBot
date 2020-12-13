using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HydrogenBot.Models.Twitter
{
    public struct TweetSearchResultDto
    {
        [JsonPropertyName("data")]
        public IEnumerable<TweetSearchResultData>? Data { get; set; }

        [JsonPropertyName("includes")]
        public TweetSearchResultIncludes Includes { get; set; }

        [JsonPropertyName("meta")]
        public TweetSearchResultMeta Meta { get; set; }
    }

    public struct TweetSearchResultData
    {
        [JsonPropertyName("author_id")]
        public string AuthorId { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public struct TweetSearchResultIncludes
    {
        [JsonPropertyName("users")]
        public IEnumerable<TweetSearchResultIncludesUsers> Users { get; set; }
    }

    public struct TweetSearchResultIncludesUsers
    {
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }

    public struct TweetSearchResultMeta
    {
        [JsonPropertyName("newest_id")]
        public string? NewestId { get; set; }
        
        [JsonPropertyName("result_count")]
        public int ResultCount { get; set; }
    }
}
