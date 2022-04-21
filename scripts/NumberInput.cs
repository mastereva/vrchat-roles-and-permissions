
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class NumberInput : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject text;
    public string input;


    public override void Interact()
    {
        NumberInputExecute();
    }

    private void NumberInputExecute()
    {
        UdonBehaviour roleMasterBehaviour = (UdonBehaviour)roleMaster.GetComponent(typeof(UdonBehaviour));
        string playerPasscode = (string)roleMasterBehaviour.GetProgramVariable("playerPasscode");

        playerPasscode = playerPasscode + input;

        roleMasterBehaviour.SetProgramVariable("playerPasscode", playerPasscode);
        Text textBehaviour = (Text)text.GetComponent(typeof(Text));

        string currentText = textBehaviour.text;
        if (currentText.Length > 0)
        {
            string lastCharacter = currentText.Substring(currentText.Length - 1);
            if (lastCharacter == "|")
            {
                textBehaviour.text = input;
            } else
            {
                textBehaviour.text = textBehaviour.text + input;
            }
        } else
        {
            textBehaviour.text = textBehaviour.text + input;
        }
    }
}
