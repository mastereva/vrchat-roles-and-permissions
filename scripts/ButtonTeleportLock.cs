
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEngine.UI;


public class ButtonTeleportLock : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject[] teleportButtons;
    public string[] allowedRoles;

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
        foreach (GameObject teleportButton in teleportButtons)
        {
            if (teleportButton != null && teleportButton.name != "ToDestroy")
            {
                VRCPlayerApi localPlayer = Networking.LocalPlayer;
                Networking.SetOwner(localPlayer, teleportButton);

                UdonBehaviour buttonBehaviour = (UdonBehaviour)teleportButton.GetComponent(typeof(UdonBehaviour));
                bool buttonLocked = (bool)buttonBehaviour.GetProgramVariable("locked");

                buttonBehaviour.SetProgramVariable("locked", !buttonLocked);
            }
        }
    }
}
