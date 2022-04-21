
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


public class ButtonTeleport : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject teleportGoal;
    public string[] allowedRoles;

    [UdonSynced] public bool locked;

    public override void Interact()
    {
        UdonBehaviour roleMasterBehaviour = (UdonBehaviour)roleMaster.GetComponent(typeof(UdonBehaviour));
        string playerRole = (string)roleMasterBehaviour.GetProgramVariable("playerRole");

        bool isAllowedToUse = false;
        foreach (string allowedRole in allowedRoles)
        {
            if (allowedRole == playerRole || allowedRole == "guest")
            {
                isAllowedToUse = true;
            }
        }

        if (isAllowedToUse == true)
        {
            TurnOn();
        }
    }

    public void TurnOn()
    {
        if (teleportGoal != null && teleportGoal.name != "ToDestroy")
        {
            if (locked == false)
            {
                VRCPlayerApi localPlayer = Networking.LocalPlayer;

                localPlayer.TeleportTo(teleportGoal.transform.position, teleportGoal.transform.rotation);
            }
        }
    }
}
