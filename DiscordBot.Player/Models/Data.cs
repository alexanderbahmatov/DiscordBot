﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Player.Models
{
    public class Data
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Duration { get; set; }
        public Data (string id, string url, string title, string author, string thumbnailUrl, string duration)
        {
            Id = id;
            Url = url;
            Title = title;
            Author = author;
            ThumbnailUrl = thumbnailUrl;
            Duration = duration;
        }

        public static Data ParseYtdlResponse (JObject ytdlResponseObject)
        {
            string id = ytdlResponseObject["id"].Value<string>();
            string url = ytdlResponseObject["webpage_url"].Value<string>();
            string title = ytdlResponseObject["title"].Value<string>();
            string author = ytdlResponseObject["uploader"].Value<string>();
            string thumbnailUrl = ytdlResponseObject["thumbnails"][0]["url"].Value<string>();
            string duration = ytdlResponseObject["duration"].Value<string>();

            return new Data(id, url, title, author, thumbnailUrl, duration);
        }
    }
}
