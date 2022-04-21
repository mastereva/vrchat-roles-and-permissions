
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.Udon.Common.Interfaces;

public class ColliderAutomaticRole : UdonSharpBehaviour
{
    public GameObject roleMaster;

    public string[] usernames;
    public string[] rolesToAssign;

    public bool assignOnlyOnce;

    private bool roleAssigned;

    public override void OnPlayerTriggerEnter(VRCPlayerApi other)
    {
        if (roleAssigned == false || assignOnlyOnce == false)
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;
            if (localPlayer.displayName == other.displayName && usernames.Length == rolesToAssign.Length)
            {
                roleAssigned = true;

                UdonBehaviour roleMasterBehaviour = (UdonBehaviour)roleMaster.GetComponent(typeof(UdonBehaviour));

                string foundRole = null;

                for(int index = 0;index < usernames.Length; index++)
                {
                    string username = usernames[index];

                    if (localPlayer.displayName == username)
                    {
                        foundRole = rolesToAssign[index];
                    }
                }
            
                if (foundRole != null)
                {
                    Networking.SetOwner(localPlayer, roleMaster);
                    
                    string capitalizedRole = foundRole.ToUpper();

                    roleMasterBehaviour.SetProgramVariable("playerRole", foundRole);

                    roleMasterBehaviour.SetProgramVariable("updatedPlayer", localPlayer.playerId);
                    roleMasterBehaviour.SetProgramVariable("updatedPlayerRole", capitalizedRole);
                }
            }
        }
    }
}
