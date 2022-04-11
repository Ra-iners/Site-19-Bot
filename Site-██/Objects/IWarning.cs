using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct IWarning
{
    [JsonProperty("Reason")]
    public string Reason;
    [JsonProperty("WarnedBy")]
    public string WarnedBy;
    [JsonProperty("WarnedAt")]
    public DateTime WarnedAt;
    [JsonProperty("ExpiresAt")]
    public DateTime Expires;
}