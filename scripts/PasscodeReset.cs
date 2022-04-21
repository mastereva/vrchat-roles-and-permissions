
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class PasscodeReset : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject text;

    public override void Interact()
    {
        PasscodeResetExecute();
    }

    private void PasscodeResetExecute()
    {
        UdonBehaviour roleMasterBehaviour = (UdonBehaviour)roleMaster.GetComponent(typeof(UdonBehaviour));

        roleMasterBehaviour.SetProgramVariable("playerPasscode", "");
        Text textBehaviour = (Text)text.GetComponent(typeof(Text));
        textBehaviour.text = "";
    }

}
