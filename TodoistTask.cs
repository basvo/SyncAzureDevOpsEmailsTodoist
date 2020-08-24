using System;
using Newtonsoft.Json;

namespace Basvo.Function
{
    public partial class TodoistTask
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("project_id")]
        public long ProjectId { get; set; }

        [JsonProperty("section_id")]
        public long SectionId { get; set; }

        [JsonProperty("order")]
        public long Order { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("completed")]
        public bool Completed { get; set; }

        [JsonProperty("label_ids")]
        public long[] LabelIds { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("comment_count")]
        public long CommentCount { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("due", NullValueHandling = NullValueHandling.Ignore)]
        public Due Due { get; set; }
    }

    public partial class Due
    {
        [JsonProperty("recurring")]
        public bool Recurring { get; set; }

        [JsonProperty("string")]
        public string String { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }
    }
}