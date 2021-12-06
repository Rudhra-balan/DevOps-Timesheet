using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HI.DevOps.DomainCore.Models.DevOps
{
    public partial class TeamProjectViewModel
    {
        [JsonProperty("count")] public long Count { get; set; }

        [JsonProperty("value")] public List<TeamProject> TeamProjectList { get; set; }
    }

    public class TeamProject
    {
        [JsonProperty("id")] public Guid Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("url")] public Uri Url { get; set; }

        [JsonProperty("state")] public string State { get; set; }

        [JsonProperty("revision")] public long Revision { get; set; }

        [JsonProperty("visibility")] public string Visibility { get; set; }

        [JsonProperty("lastUpdateTime")] public DateTimeOffset LastUpdateTime { get; set; }
    }

    public partial class TeamProjectViewModel
    {
        public static TeamProjectViewModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<TeamProjectViewModel>(json, Converter.Settings);
        }
    }

    public static partial class Serialize
    {
        public static string ToJson(this TeamProjectViewModel self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            }
        };
    }
}