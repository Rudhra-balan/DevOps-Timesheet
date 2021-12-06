using System;
using Newtonsoft.Json;

namespace HI.DevOps.DomainCore.Models.DevOps
{
    public partial class DevOpsUserProfile
    {
        [JsonProperty("displayName")] public string DisplayName { get; set; }

        [JsonProperty("publicAlias")] public Guid PublicAlias { get; set; }

        [JsonProperty("emailAddress")] public string EmailAddress { get; set; }

        [JsonProperty("coreRevision")] public long CoreRevision { get; set; }

        [JsonProperty("timeStamp")] public DateTimeOffset TimeStamp { get; set; }

        [JsonProperty("id")] public Guid Id { get; set; }

        [JsonProperty("revision")] public long Revision { get; set; }
    }

    public partial class DevOpsUserProfile
    {
        public static DevOpsUserProfile FromJson(string json)
        {
            return JsonConvert.DeserializeObject<DevOpsUserProfile>(json, Converter.Settings);
        }
    }

    public static partial class Serialize
    {
        public static string ToJson(this DevOpsUserProfile self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }
}