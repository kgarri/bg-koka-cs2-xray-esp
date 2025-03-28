using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace AdminESP;

public class Config : BasePluginConfig
{
    [JsonPropertyName("debug_mode")] public bool DebugMode { get; set; } = true;
    [JsonPropertyName("chat_prefix")] public string ChatPrefix { get; set; } = "{GREEN}[AdminESP]";
    [JsonPropertyName("admin_flag")] public string AdminFlag { get; set; } = "@css/ban";
    [JsonPropertyName("hide_from_spectator_list")] public bool HideAdminSpectators { get; set; } = false;
    [JsonPropertyName("allow_esp_when_dead")] public bool AllowDeadAdminESP { get; set; } = true;
    [JsonPropertyName("skip_spectating_espers")] public bool SkipSpectatingEsps { get; set; } = true;

}