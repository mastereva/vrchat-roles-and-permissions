
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ButtonAnimationBool : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject[] gameObjectsWithAnimators;
    public string animationParameterName;
    public bool isGlobal;
    public string[] allowedRoles;

    public bool activateValue;
    public bool deactivateValue;

    public bool automaticReset;
    public bool disableAutomaticResetOfDefaultValue;
    public float secondsToResetAfter;

    private string activatedByPlayerName;
    private bool activated;
    [UdonSynced] private bool activatedGlobal;
    private float secondsToWait;

    public string[] additionalAnimatorVariablesToSet;
    public bool additionalAnimatorVariablesToSetValue;

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
            } else
            {
                if (syncComplete == false)
                {
                    syncComplete = true;
                    foreach (GameObject arrayGameObject in gameObjectsWithAnimators)
                    {
                        if (arrayGameObject != null && arrayGameObject.name != "ToDestroy")
                        {
                            Animator animator = (Animator)arrayGameObject.GetComponent(typeof(Animator));

                            if (animator != null)
                            {
                                if (activatedGlobal == true)
                                {
                                    activated = true;
                                    animator.SetBool(animationParameterName, activateValue);
                                }
                                else
                                {
                                    activated = false;
                                    animator.SetBool(animationParameterName, deactivateValue);
                                }
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
        foreach (GameObject arrayGameObject in gameObjectsWithAnimators)
        {
            if (arrayGameObject != null && arrayGameObject.name != "ToDestroy")
            {
                Animator animator = (Animator)arrayGameObject.GetComponent(typeof(Animator));
                if (animator != null)
                {
                    if (activated == true)
                    {
                        int seconds = (int)(secondsToWait % 60);
                        if (automaticReset == false || (seconds >= secondsToResetAfter || disableAutomaticResetOfDefaultValue == true))
                        {
                            if (disableAutomaticResetOfDefaultValue == false || automaticReset == false)
                            {
                                animator.SetBool(animationParameterName, deactivateValue);
                            }

                            if (additionalAnimatorVariablesToSet.Length > 0)
                            {
                                foreach (string additionalVariableToReset in additionalAnimatorVariablesToSet)
                                {
                                    animator.SetBool(additionalVariableToReset, additionalAnimatorVariablesToSetValue);
                                }                                
                            }
                        }
                    } else
                    {
                        animator.SetBool(animationParameterName, activateValue);
                    }
                }
            }
        }

        if (activated == true)
        {
            activated = false;
        } else
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
