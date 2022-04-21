
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.Udon.Common.Interfaces;

public class PasscodeSubmit : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject text;

    public override void Interact()
    {
        PasscodeSubmitExecute();
    }

    private void PasscodeSubmitExecute()
    {
        UdonBehaviour roleMasterBehaviour = (UdonBehaviour)roleMaster.GetComponent(typeof(UdonBehaviour));
        string playerPasscode = (string)roleMasterBehaviour.GetProgramVariable("playerPasscode");
        string[] roleName = (string[])roleMasterBehaviour.GetProgramVariable("roleName");
        string[] rolePasscodes = (string[])roleMasterBehaviour.GetProgramVariable("rolePasscode"); ;

        for (int index = 0; index < rolePasscodes.Length; index++)
        {
            string rolePasscode = rolePasscodes[index];

            if (playerPasscode == rolePasscode && rolePasscode != "")
            {
                VRCPlayerApi localPlayer = Networking.LocalPlayer;
                Networking.SetOwner(localPlayer, roleMaster);
                roleMasterBehaviour.SetProgramVariable("playerPasscode", "");

                Text textBehaviour = (Text)text.GetComponent(typeof(Text));

                string playerRole = roleName[index];
                string capitalizedRole = playerRole.ToUpper();

                textBehaviour.text = "|Set Role: " + capitalizedRole + "|";

                roleMasterBehaviour.SetProgramVariable("playerRole", playerRole);


                roleMasterBehaviour.SetProgramVariable("updatedPlayer", localPlayer.playerId);
                roleMasterBehaviour.SetProgramVariable("updatedPlayerRole", capitalizedRole);
            }
        }
    }
}
