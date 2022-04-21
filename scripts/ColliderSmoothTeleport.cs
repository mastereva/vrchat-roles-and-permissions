
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ColliderSmoothTeleport : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public string[] allowedRoles;

    public Transform ChildOfTarget;
    public Transform ChildOfOrigin;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
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
    }

    public void TurnOn()
    {
        VRCPlayerApi player = Networking.LocalPlayer;

        ChildOfOrigin.SetPositionAndRotation(player.GetPosition(), player.GetRotation());
        ChildOfTarget.localPosition = ChildOfOrigin.localPosition;
        ChildOfTarget.localRotation = ChildOfOrigin.localRotation;
        player.TeleportTo(ChildOfTarget.position, ChildOfTarget.rotation);
    }
}
