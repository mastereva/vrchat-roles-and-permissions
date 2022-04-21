
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEngine.UI;

public class MovingTag : UdonSharpBehaviour
{
    public GameObject roleMaster;
    public GameObject background;
    public GameObject text;
    
    public HumanBodyBones trackedBone;
    public float heightOverBone;

    private int assignedPlayerName;

    private bool initiated;

    private float secondsToWait;

    void Update()
    {

        if (initiated == true)
        {
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(assignedPlayerName);
            Text textBehaviour = (Text) text.GetComponent(typeof(Text));
            VRCPlayerApi localPlayer = Networking.LocalPlayer;

            if (localPlayer.playerId == assignedPlayerName)
            {
                int seconds = (int)(secondsToWait % 60);
                if (seconds < 30)
                {
                    secondsToWait += Time.deltaTime;
                } else
                {
                    Networking.SetOwner(localPlayer, roleMaster);
                    UdonBehaviour roleMasterBehaviour = (UdonBehaviour) roleMaster.GetComponent(typeof(UdonBehaviour));

                    roleMasterBehaviour.SetProgramVariable("updatedPlayer", assignedPlayerName);
                    roleMasterBehaviour.SetProgramVariable("updatedPlayerRole", textBehaviour.text);

                    secondsToWait = 0.0f;
                }
            }

            if (player != null)
            {
                Vector3 bonePosition = player.GetBonePosition(trackedBone);
                bonePosition.y += heightOverBone;

                Quaternion boneRotation = player.GetBoneRotation(trackedBone);
                boneRotation.x = 0;
                boneRotation.z = 0;

                gameObject.transform.SetPositionAndRotation(bonePosition, boneRotation);

                UdonBehaviour roleMasterBehaviour = (UdonBehaviour)roleMaster.GetComponent(typeof(UdonBehaviour));

                int updatedPlayer = (int)roleMasterBehaviour.GetProgramVariable("updatedPlayer");
                string updatedPlayerRole = (string)roleMasterBehaviour.GetProgramVariable("updatedPlayerRole");

                int lastUpdatedPlayer = (int)roleMasterBehaviour.GetProgramVariable("lastUpdatedPlayer");
                string lastUpdatedPlayerRole = (string)roleMasterBehaviour.GetProgramVariable("lastUpdatedPlayerRole");


                // Disable Tag if Guest Name Tag is disabled
                bool guestHasNametag = (bool) roleMasterBehaviour.GetProgramVariable("guestHasNametag");
                if (guestHasNametag == false && textBehaviour.text == "GUEST" && (background.activeSelf == true || text.activeSelf == true))
                {
                    background.SetActive(false);
                    text.SetActive(false);
                }

                if (updatedPlayer != lastUpdatedPlayer || updatedPlayerRole != lastUpdatedPlayerRole)
                {
                    if (updatedPlayer == assignedPlayerName)
                    {
                        roleMasterBehaviour.SetProgramVariable("lastUpdatedPlayer", updatedPlayer);
                        roleMasterBehaviour.SetProgramVariable("lastUpdatedPlayerRole", updatedPlayerRole);
                        textBehaviour.text = updatedPlayerRole;

                        if (roleMaster != null)
                        {
                            string[] roleName = (string[])roleMasterBehaviour.GetProgramVariable("roleName");
                            Color[] roleColors = (Color[])roleMasterBehaviour.GetProgramVariable("roleColor");

                            if (roleName.Length == roleColors.Length)
                            {
                                for (int index = 0; index < roleName.Length; index++)
                                {
                                    string role = roleName[index];
                                    role = role.ToUpper();

                                    if (role == updatedPlayerRole)
                                    {
                                        Color color = roleColors[index];
                                        if (color != null)
                                        {
                                            textBehaviour.color = color;
                                        }
                                        bool[] roleNameTagEnabled = (bool[]) roleMasterBehaviour.GetProgramVariable("roleNameTagEnabled");
                                        bool hasNameTagEnabled = roleNameTagEnabled[index];

                                        if (hasNameTagEnabled == true && (text.activeSelf == false || background.activeSelf == false))
                                        {
                                            text.SetActive(true);
                                            background.SetActive(true);
                                        }
                                        if (hasNameTagEnabled == false && (text.activeSelf == true || background.activeSelf == true))
                                        {
                                            text.SetActive(false);
                                            background.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } else
            {
                Destroy(gameObject);
            }
        }
    }
}
