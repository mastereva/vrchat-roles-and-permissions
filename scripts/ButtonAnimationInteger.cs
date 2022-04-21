
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


public class ButtonAnimationInteger : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject[] gameObjectsWithAnimators;
    public string animationParameterName;
    public bool isGlobal;
    public string[] allowedRoles;

    public int activeValue;
    public int disableValue;

    public override void Interact()
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
                TurnOn();
            }
        }
    }

    public void TurnOnNetworked()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "TurnOn");
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
                    int currentInteger = animator.GetInteger(animationParameterName);

                    if (currentInteger == activeValue)
                    {
                        animator.SetInteger(animationParameterName, disableValue);
                    } else
                    {
                        animator.SetInteger(animationParameterName, activeValue);
                    }
                }
            }
        }
    }
}
