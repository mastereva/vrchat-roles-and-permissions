
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


public class ButtonGameObjectActive : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject[] gameObjects;
    public bool isGlobal;
    public string[] allowedRoles;

    public bool activateValue;
    public bool deactivateValue;

    public bool automaticReset;
    public float secondsToResetAfter;

    [UdonSynced] private bool activatedGlobal;
    private string activatedByPlayerName;
    private bool activated;
    private float secondsToWait;

    private float syncingTimer;
    private bool syncComplete;

    void Start()
    {
        secondsToWait = 0;
        syncingTimer = 0;
    }

    private void Update()
    {
        if (automaticReset == true && activated == true)
        {
            int seconds = (int)(secondsToWait % 60);
            if (seconds < secondsToResetAfter)
            {
                secondsToWait += Time.deltaTime;
            }
            if (seconds >= secondsToResetAfter)
            {
                TurnOn();

                secondsToWait = 0;
            }
        }

        if (isGlobal == true)
        {
            int syncingSeconds = (int)(syncingTimer % 60);
            if (syncingSeconds < 10)
            {
                syncingTimer += Time.deltaTime;
            }
            else
            {
                if (syncComplete == false)
                {
                    syncComplete = true;
                    foreach (GameObject arrayGameObject in gameObjects)
                    {
                        if (arrayGameObject != null && arrayGameObject.name != "ToDestroy")
                        {
                            if (activatedGlobal == true)
                            {
                                activated = true;
                                arrayGameObject.SetActive(activateValue);
                            }
                            else
                            {
                                activated = false;
                                arrayGameObject.SetActive(deactivateValue);
                            }
                        }
                    }
                }
            }
        }
    }

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
            if (automaticReset == false || activated == false)
            {
                activatedByPlayerName = Networking.LocalPlayer.displayName;
                if (isGlobal == true)
                {
                    TurnOnNetworked();
                }
                else
                {
                    TurnOn();
                }
            }
        }
    }

    public void TurnOnNetworked()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "TurnOn");
    }

    public void TurnOn()
    {
        foreach (GameObject arrayGameObject in gameObjects)
        {
            if (arrayGameObject != null && arrayGameObject.name != "ToDestroy")
            {
                if (activated == true)
                {
                    int seconds = (int)(secondsToWait % 60);
                    if (automaticReset == false || seconds >= secondsToResetAfter)
                    {
                        arrayGameObject.SetActive(deactivateValue);
                    }
                }
                else
                {
                    arrayGameObject.SetActive(activateValue);
                }
            }
        }

        if (activated == true)
        {
            activated = false;
        }
        else
        {
            activated = true;
        }


        if (isGlobal == true)
        {
            if (activatedByPlayerName == Networking.LocalPlayer.displayName)
            {
                activatedByPlayerName = "";

                VRCPlayerApi localPlayer = Networking.LocalPlayer;
                Networking.SetOwner(localPlayer, gameObject);
                if (activatedGlobal == true)
                {
                    activatedGlobal = false;
                }
                else
                {
                    activatedGlobal = true;
                }
            }
        }
    }
}
