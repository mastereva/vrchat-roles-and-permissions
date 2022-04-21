
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RoleMaster : UdonSharpBehaviour
{
    public string[] roleName;
    public string[] rolePasscode;
    public Color[] roleColor;

    public bool[] roleNameTagEnabled;
    public GameObject nameTagTemplate;
    public bool guestHasNametag;

    [UdonSynced] private string updatedPlayerRole;
    [UdonSynced] private int updatedPlayer;
    private string playerName;
    private string playerRole;
    private string playerPasscode;

    private int lastUpdatedPlayer;
    private string lastUpdatedPlayerRole;

    void Start()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        playerName = localPlayer.displayName;
        playerRole = "";
        playerPasscode = "";

        if (nameTagTemplate != null)
        {
            VRCPlayerApi[] prePlayers = new VRCPlayerApi[80];
            VRCPlayerApi[] players = VRCPlayerApi.GetPlayers(prePlayers);

            foreach (VRCPlayerApi player in players)
            {
                if (player != null && player.displayName == localPlayer.displayName)
                {
                    GameObject newPlayerNameTag = VRCInstantiate(nameTagTemplate);
                    UdonBehaviour newPlayerNameTagBehaviour = (UdonBehaviour) newPlayerNameTag.GetComponent(typeof(UdonBehaviour));
                    newPlayerNameTagBehaviour.SetProgramVariable("assignedPlayerName", player.playerId);
                    newPlayerNameTagBehaviour.SetProgramVariable("initiated", true);
                }
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player) 
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (nameTagTemplate != null && player.displayName != localPlayer.displayName)
        {
            GameObject newPlayerNameTag = VRCInstantiate(nameTagTemplate);
            UdonBehaviour newPlayerNameTagBehaviour = (UdonBehaviour)newPlayerNameTag.GetComponent(typeof(UdonBehaviour));
            newPlayerNameTagBehaviour.SetProgramVariable("assignedPlayerName", player.playerId);
            newPlayerNameTagBehaviour.SetProgramVariable("initiated", true);
        }
    }
}
