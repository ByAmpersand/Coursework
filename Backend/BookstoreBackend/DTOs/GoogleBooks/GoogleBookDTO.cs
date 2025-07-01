﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace BookstoreBackend.DTOs.GoogleBooks
{
    public class GoogleApiSearchResult
    {
        [JsonPropertyName("items")]
        public List<GoogleBookItem> Items { get; set; }
    }

    public class GoogleBookItem
    {
        [JsonPropertyName("volumeInfo")]
        public VolumeInfo VolumeInfo { get; set; }
    }

    public class VolumeInfo
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("authors")]
        public List<string> Authors { get; set; }

        [JsonPropertyName("publisher")]
        public string Publisher { get; set; }

        [JsonPropertyName("publishedDate")]
        public string PublishedDate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("pageCount")]
        public int? PageCount { get; set; }

        [JsonPropertyName("industryIdentifiers")]
        public List<IndustryIdentifier> IndustryIdentifiers { get; set; }

        [JsonPropertyName("imageLinks")]
        public ImageLinks ImageLinks { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }
    }

    public class IndustryIdentifier
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }
    }

    public class ImageLinks
    {
        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }
    }
}