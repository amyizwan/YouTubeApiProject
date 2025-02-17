using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeApiProject.Models;

namespace YouTubeApiProject.Services
{
    public class YouTubeApiService
    {
        private readonly string _apiKey;

        public YouTubeApiService(IConfiguration configuration)
        {
            _apiKey = configuration["YouTubeApiKey"]; // Fetch API key from appsettings.json
        }

        public async Task<List<YouTubeVideoModel>> SearchVideosAsync(string query, string duration = "any", string uploadDate = "any")
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = "YoutubeProject"
            });

            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = query;  // User's query from form input
            searchRequest.MaxResults = 10;
            searchRequest.Type = "video";

            // **Apply video duration filter**
            if (duration != "any")
            {
                searchRequest.VideoDuration = duration switch
                {
                    "short" => SearchResource.ListRequest.VideoDurationEnum.Short__,
                    "medium" => SearchResource.ListRequest.VideoDurationEnum.Medium,
                    "long" => SearchResource.ListRequest.VideoDurationEnum.Long__,
                    _ => SearchResource.ListRequest.VideoDurationEnum.Any
                };
            }

            // **Apply upload date filter**
            if (uploadDate != "any")
            {
                DateTime publishedAfter = uploadDate switch
                {
                    "today" => DateTime.UtcNow.AddDays(-1),
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    _ => DateTime.MinValue
                };

                if (publishedAfter != DateTime.MinValue)
                    searchRequest.PublishedAfter = publishedAfter;
            }

            var searchResponse = await searchRequest.ExecuteAsync();

            var videos = searchResponse.Items.Select(item => new YouTubeVideoModel
            {
                Title = item.Snippet.Title,
                Description = item.Snippet.Description,
                ThumbnailUrl = item.Snippet.Thumbnails.Medium.Url,
                VideoUrl = "https://www.youtube.com/watch?v=" + item.Id.VideoId
            }).ToList();

            return videos;
        }
    }
}