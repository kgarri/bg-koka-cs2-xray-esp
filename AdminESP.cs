using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

namespace AdminESP;

public sealed partial class AdminESP : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Admin ESP";
    public override string ModuleAuthor => "AquaVadis";
    public override string ModuleVersion => "1.2.0s";
    public override string ModuleDescription => "Plugin uses code borrowed from CS2Fixes / cs2kz-metamod / hl2sdk / unknown cheats and xstage from CS# discord";

    public bool[] toggleAdminESP = new bool[64];
    public bool togglePlayersGlowing = false;
    public Config Config { get; set; } = new();
    private static readonly ConVar? _forceCamera = ConVar.Find("mp_forcecamera");

    public override void Load(bool hotReload)
    {
        RegisterListeners();

        if (hotReload) {
      
            foreach (var player in Utilities.GetPlayers().Where(p => p is not null 
                                                                && p.IsValid is true
                                                                && p.Connected is PlayerConnectedState.PlayerConnected)) {

                if (cachedPlayers.Contains(player) is not true)
                    cachedPlayers.Add(player);

            }
        }

    }
    
    private CCSPlayerController GetPlayerFromUserName(String playerName)
    {

        List<CCSPlayerController> players = Utilities.GetPlayers();

        foreach (CCSPlayerController player in players)
        {
            Console.WriteLine(player.PlayerName);
            Console.WriteLine(player.Slot);
            Console.WriteLine(player.SteamID);
            if (string.Equals(player.PlayerName, playerName, StringComparison.CurrentCultureIgnoreCase))
            {
                return player;
            }
        }

        throw new Exception($"Player {playerName} not found");
        
    }

    [ConsoleCommand("css_esp_off", "Disables ESP for everyone")]
    [CommandHelper(minArgs: 0, "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnToggleAllEspOff(CCSPlayerController? adminPlayer, CommandInfo command)
    {
        for (int i = 0; i < toggleAdminESP.Length; i++) toggleAdminESP[i] = false;
        RemoveAllGlowingPlayers();
    }

    public override void Unload(bool hotReload)
    {

        DeregisterListeners();
    }

    [ConsoleCommand("css_esp", "Toggle Admin ESP")]
    [CommandHelper(minArgs: 2,"[name] [bool]", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnToggleAdminEsp(CCSPlayerController? adminPlayer, CommandInfo command)
    {
        String playerName = command.ArgByIndex(1);
        bool toggleESP = command.ArgByIndex(2) == "true";
        CCSPlayerController player = GetPlayerFromUserName(playerName);

        if (player is null || player.IsValid is not true) return;

        if (AdminManager.PlayerHasPermissions(adminPlayer, Config.AdminFlag) is not true) {

            SendMessageToSpecificChat(adminPlayer!, msg: "Admin ESP can only be used from {GREEN}admins{DEFAULT}!", print: PrintTo.Chat);
            return;  
        }

        switch (player.PawnIsAlive) {
            
            case true:
                
                    toggleAdminESP[player.Slot] = toggleESP;

                    if (toggleAdminESP[player.Slot] is true) {
                        
                        if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                            SetAllPlayersGlowing();
                        }
                        //player.ReplicateConVar("mp_forcecamera", "0");
                    }
                    else {

                        if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                            RemoveAllGlowingPlayers();
                        }
                        //player.ReplicateConVar("mp_forcecamera", _forceCamera is not null ? _forceCamera.GetPrimitiveValue<int>().ToString() : "1");
                    }
                    

                    SendMessageToSpecificChat(player, msg: $"Admin ESP has been " + (toggleAdminESP[player.Slot] ? "{GREEN}enabled!" : "{RED}disabled!"), print: PrintTo.Chat); 
                    SendMessageToSpecificChat(adminPlayer! , msg: $"Admin ESP has been " + (toggleAdminESP[player.Slot] ? "{GREEN}enabled!" : "{RED}disabled!"), print: PrintTo.Chat); 
                    return;
            
            case false:
                if (Config.AllowDeadAdminESP is true) {
                    SendMessageToSpecificChat(player, msg: "You should be {RED}dead {DEFAULT}to use Admin ESP!", print: PrintTo.Chat);
                    return;
                }
                SendMessageToSpecificChat(player, msg: "Admin ESP is only allowed while {RED}spectating{DEFAULT}!", print: PrintTo.Chat);

                break;
               
        

        }
    }
    public void OnConfigParsed(Config config)
    {
        Config = config;
    }

}
