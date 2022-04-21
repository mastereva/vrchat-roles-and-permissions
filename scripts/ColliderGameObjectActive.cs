
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ColliderGameObjectActive : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject[] gameObjects;

    public bool isGlobal;
    public string[] allowedRoles;

    public bool onEnterValue;
    public bool onExitValue;

    public override void OnPlayerTriggerEnter(VRCPlayerApi other)
    {
        UdonBehaviour roleMasterBehaviour = (UdonBehaviour)roleMaster.GetComponent(typeof(UdonBehaviour));
        string playerRole = (string) roleMasterBehaviour.GetProgramVariable("playerRole");

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
            if (isGlobal == true)
            {
                TurnOnNetworked();
            } else
            {
                if (other.playerId == Networking.LocalPlayer.playerId)
                {
                    TurnOn();
                }
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
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
            if (isGlobal == true)
            {
                TurnOffNetworked();
            }
            else
            {
                if (player.playerId == Networking.LocalPlayer.playerId)
                {
                    TurnOff();
                }
            }
        }
    }

    public void TurnOnNetworked()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "TurnOn");
    }
    public void TurnOffNetworked()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "TurnOff");
    }

    public void TurnOn()
    {
        foreach (GameObject arrayGameObject in gameObjects)
        {
            if (arrayGameObject != null && arrayGameObject.name != "ToDestroy")
            {
                arrayGameObject.SetActive(onEnterValue);
            }
        }
    }

    public void TurnOff()
    {
        foreach (GameObject arrayGameObject in gameObjects)
        {
            if (arrayGameObject != null && arrayGameObject.name != "ToDestroy")
            {
                arrayGameObject.SetActive(onExitValue);
            }
        }
    }
}
