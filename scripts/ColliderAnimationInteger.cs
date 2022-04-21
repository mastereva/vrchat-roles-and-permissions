
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


public class ColliderAnimationInteger : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject[] gameObjectsWithAnimators;
    
    public string animationParameterName;
    public bool isGlobal;
    public string[] allowedRoles;

    public int onEnterValue;
    public int onExitValue;

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
            }
            else
            {
                if (other.playerId == Networking.LocalPlayer.playerId) {
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
        foreach (GameObject arrayGameObject in gameObjectsWithAnimators)
        {
            if (arrayGameObject != null && arrayGameObject.name != "ToDestroy")
            {
                Animator animator = (Animator)arrayGameObject.GetComponent(typeof(Animator));
                if (animator != null)
                {
                    animator.SetInteger(animationParameterName, onEnterValue);
                }
            }
        }
    }

    public void TurnOff()
    {
        foreach (GameObject arrayGameObject in gameObjectsWithAnimators)
        {
            if (arrayGameObject != null && arrayGameObject.name != "ToDestroy")
            {
                Animator animator = (Animator)arrayGameObject.GetComponent(typeof(Animator));
                if (animator != null)
                {
                    animator.SetInteger(animationParameterName, onExitValue);
                }
            }
        }
    }
}
