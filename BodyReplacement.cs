using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zeekerss;
using Zeekerss.Core;
using Zeekerss.Core.Singletons;
using Newtonsoft.Json;
using System.Linq;
using System.Xml;
using System.IO;
using BepInEx;
using GameNetcodeStuff;
using UnityEngine.PlayerLoop;
using Unity.Netcode;
using System.Reflection;
using HarmonyLib;
using ModelReplacement;

namespace PlayerAvatarLoader
{
    public class BodyReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            PlayerControllerB component = ((Component)this).gameObject.GetComponent<PlayerControllerB>();
            ulong playerSteamId = component.playerSteamId;
            string playerSteamName = component.playerUsername;
            int suitId = component.currentSuitID;
            string model_name = StartOfRound.Instance.unlockablesList.unlockables[suitId].unlockableName;

            if (model_name == "Default")
            {
                Debug.LogWarning("Detected An Attempt To Load Suit 'Default'. This Is Likely Because MoreSuits Is Installed. Falling Back To 'Orange suit'.");
                model_name = "Orange suit";
            }
            
            Debug.LogWarning("Attempting To Load '" + model_name + "' For " + playerSteamName + "(" + playerSteamId.ToString() + ")");

            if (Assets.loadedBundleIds.FirstOrDefault(stringToCheck => stringToCheck.Contains(playerSteamId.ToString())) != null)
            {
                Debug.LogWarning("Found Previously Loaded Bundle For " + playerSteamName + "(" + playerSteamId.ToString() + ")");

                int j = Assets.loadedBundleIds.FindIndex(x => x.Contains(playerSteamId.ToString()));
                GameObject modelPre = Assets.loadedAssetBundles[j].LoadAsset<GameObject>(model_name);

                if (modelPre != null)
                {
                    return modelPre;
                }
                else
                {
                    Debug.LogError("No '" + model_name + "' Model Found For " + playerSteamId.ToString() + "!!! Falling Back To Default");
                    modelPre = Assets.initAssetBundle.LoadAsset<GameObject>(model_name);

                    if (modelPre == null)
                    {
                        Debug.LogError("No '" + model_name + "' Model Found Inside Default Bundle!!! Falling Back To 'Orange suit'");
                        return Assets.initAssetBundle.LoadAsset<GameObject>("Orange suit");
                    }

                    return modelPre;
                }

            }
            else
            {
                Debug.LogWarning("Bundle Not Yet Loaded For " + playerSteamName + "(" + playerSteamId.ToString() + ")" + ". Attempting To Load New Bundle...");
                AssetBundle userAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Avatars", playerSteamId.ToString()));

                if (userAssetBundle != null)
                {
                    Assets.PopulateAssets(true, playerSteamId.ToString(), userAssetBundle);
                    GameObject modelPre = userAssetBundle.LoadAsset<GameObject>(model_name);

                    if (modelPre != null)
                    {
                        return modelPre;
                    }
                    else
                    {
                        Debug.LogError("No '" + model_name + "' Model Found For " + playerSteamId.ToString() + "!!! Falling Back To Default");
                        return Assets.initAssetBundle.LoadAsset<GameObject>(model_name);
                    }
                }
                else
                {
                    Debug.LogError(playerSteamName + "(" + playerSteamId.ToString() + ")" + " Does Not Have A Bundle!!! Falling Back To Default.");
                    return Assets.initAssetBundle.LoadAsset<GameObject>(model_name);
                }
                
            }
        }

        /*
        protected override void AddModelScripts()
        {
            //Fuck Json, XML Is My New Best Friend

            PlayerControllerB component = ((Component)this).gameObject.GetComponent<PlayerControllerB>();
            ulong playerSteamId = component.playerSteamId;
            string playerSteamName = component.playerUsername;
            int suitId = component.currentSuitID;
            string model_name = StartOfRound.Instance.unlockablesList.unlockables[suitId].unlockableName;
            if (model_name == "Default") { model_name = "Orange suit"; }

            string xmlName = playerSteamId.ToString() + ".xml";

            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Avatars", xmlName)))
            {
                Debug.LogWarning("Dynamic Bones Found For " + playerSteamName + "(" + playerSteamId.ToString() + "). Checking Settings For '" + model_name + "'...");
                var xmlFile = new XmlDocument();
                xmlFile.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Avatars", xmlName));

                string suitName = String.Concat(model_name.Where(c => !Char.IsWhiteSpace(c)));
                var suitData = xmlFile.SelectSingleNode("/root/" + suitName);

                if (suitData != null)
                {
                    //Dynamic Bone Adding
                    var dynBone = xmlFile.SelectSingleNode("/root/" + suitName + "/DynBones");
                    if (dynBone != null)
                    {
                        Debug.LogWarning("Loading Dynamic Bones For '" + model_name + "'...");
                        bool stop = false;
                        int i = 1;
                        while (stop == false)
                        {
                            //Loop Through DynBones/BoneX until BoneX cannot be found
                            var dynBoneData = xmlFile.SelectSingleNode("/root/" + suitName + "/DynBones/Bone" + i.ToString());
                            if (dynBoneData != null)
                            {
                                string boneName = dynBoneData["sD_boneName"].InnerText;
                                int updateRate = Int32.Parse(dynBoneData["iD_updateRate"].InnerText);
                                float damping = float.Parse(dynBoneData["fD_damping"].InnerText);
                                float elasticity = float.Parse(dynBoneData["fD_elasticity"].InnerText);
                                float stiffness = float.Parse(dynBoneData["fD_stiffness"].InnerText);
                                float inert = float.Parse(dynBoneData["fD_inert"].InnerText);
                                float radius = float.Parse(dynBoneData["fD_radius"].InnerText);

                                string[] preGravity = dynBoneData["vD_gravity"].InnerText.Split("%/");
                                var gravity = new Vector3(float.Parse(preGravity[0]), float.Parse(preGravity[1]), float.Parse(preGravity[2]));

                                string pre1Exclude = dynBoneData["sD_exclude"].InnerText;
                                string[] pre2Exclude = null;
                                List<Transform> exclude = new List<Transform>();

                                Debug.LogWarning(
                                    boneName + ", " 
                                    + updateRate + ", " 
                                    + damping + ", " 
                                    + elasticity + ", " 
                                    + stiffness + ", " 
                                    + inert + ", " 
                                    + radius + ", " 
                                    + gravity);

                                if (pre1Exclude != "")
                                {
                                    pre2Exclude = pre1Exclude.Split("%/");
                                    
                                    string excludeDebugInfo = "Excluded Bones: ";
                                    for (int j = 0; j < pre2Exclude.Length; j++)
                                    {
                                        if (pre2Exclude.Length > j)
                                        {
                                            excludeDebugInfo += pre2Exclude[j] + ", ";
                                        }
                                        else { excludeDebugInfo += pre2Exclude[j]; }
                                    }
                                    Debug.LogWarning(excludeDebugInfo);

                                    for (int j = 0; j < pre2Exclude.Length; j++)
                                    {
                                        exclude.Add(replacementModel.GetComponentsInChildren<Transform>().Where(x => x.name.Contains(pre2Exclude[j])).First());
                                    }
                                }
                                //else { Debug.LogWarning("No Bones Were Excluded From '" + boneName + "'. This Can Safely Be Ignored If No Bones Are Meant To Be Excluded"); }

                                //Start Using Info Gathered To Apply Dynamic Bones
                                var getDynBone = replacementModel.GetComponentsInChildren<Transform>().Where(x => x.name.Contains(boneName)).First();
                                DynamicBone setDynBone = getDynBone.gameObject.AddComponent<DynamicBone>();
                                setDynBone.m_Root = getDynBone;
                                setDynBone.m_UpdateRate = updateRate;
                                setDynBone.m_Damping = damping;
                                setDynBone.m_Elasticity = elasticity;
                                setDynBone.m_Stiffness = stiffness;
                                setDynBone.m_Inert = inert;
                                setDynBone.m_Radius = radius;
                                setDynBone.m_Gravity = gravity;
                                if (exclude != null) 
                                { 
                                    setDynBone.m_Exclusions = exclude; 
                                }
                                else { Debug.LogWarning("No Bones Were Excluded From '" + boneName + "'. This Can Safely Be Ignored If No Bones Are Meant To Be Excluded"); }


                                i += 1;
                            }
                            else { 
                                stop = true;
                                Debug.LogWarning("All Dynamic Bones Found!");
                            }
                            
                        }
                    }
                    else 
                    { 
                        Debug.LogError("Err: dynBone = null"); 
                    }

                    //Collider Bone Adding
                    var colBone = xmlFile.SelectSingleNode("/root/" + suitName + "/ColBones");
                    if (colBone != null)
                    {
                        Debug.LogWarning("Loading Collider Bones For '" + model_name + "'...");
                        bool stop = false;
                        int i = 1;
                        while (stop == false)
                        {
                            var colBoneData = xmlFile.SelectSingleNode("/root/" + suitName + "/ColBones/Bone" + i.ToString());
                            if (colBoneData != null)
                            {
                                string boneName = colBoneData["sC_boneName"].InnerText;

                                string pre1Center = colBoneData["vC_center"].InnerText;
                                string[] pre2Center = pre1Center.Split("%/");
                                Vector3 center = new Vector3(float.Parse(pre2Center[0]), float.Parse(pre2Center[1]), float.Parse(pre2Center[2]));

                                float radius = float.Parse(colBoneData["fC_radius"].InnerText);
                                float height = float.Parse(colBoneData["fC_height"].InnerText);

                                string preDirection = colBoneData["sC_direction"].InnerText;
                                var direction = DynamicBoneCollider.Direction.X;
                                if (preDirection == "Y" || preDirection == "y")
                                {
                                    direction = DynamicBoneCollider.Direction.Y;
                                } else if (preDirection == "Z" || preDirection == "z")
                                {
                                    direction = DynamicBoneCollider.Direction.Z;
                                }

                                string preBound = colBoneData["sC_bound"].InnerText;
                                var bound = DynamicBoneCollider.Bound.Outside;
                                if (preBound == "Inside" || preBound == "inside")
                                {
                                    bound = DynamicBoneCollider.Bound.Inside;
                                }

                                Debug.LogWarning(boneName + ", "
                                    + radius + ", "
                                    + height + ", "
                                    + preDirection + ", "
                                    + preBound);

                                Debug.LogWarning("Center Of Collider At: " + pre2Center[0] + ", " + pre2Center[1] + ", " + pre2Center[2]);

                                //Start Using Info Gathered To Apply Collider Bones
                                var getColBone = replacementModel.GetComponentsInChildren<Transform>().Where(x => x.name.Contains(boneName)).First();
                                var setColBone = getColBone.gameObject.AddComponent<DynamicBoneCollider>();
                                setColBone.m_Center = center;
                                setColBone.m_Radius = radius;
                                setColBone.m_Height = height;
                                setColBone.m_Direction = direction;
                                setColBone.m_Bound = bound;

                                i += 1;
                            }
                            else
                            {
                                stop = true;
                                Debug.LogWarning("All Collider Bones Found!");
                            }

                        }
                    }
                    else
                    {
                        Debug.LogError("No Collider Bones For " + model_name + ". This Is Safe To Ignore If You Did Not Set Colliders. Err: colBone = null");
                    }

                } else
                {
                    Debug.LogError("No Dynamic Bones Found For " + model_name + ". This Is Safe To Ignore If You Did Not Set Dynamic Bones. Err: suitData = null");
                }

            }
            else
            {
                Debug.LogError("No Dynamic Bones Found For " + playerSteamName + "(" + playerSteamId.ToString() + "). Skipping Step.");
            }

        }
        */

        protected override void OnEmoteStart(int emoteId)
        {
            PlayerControllerB component = ((Component)this).gameObject.GetComponent<PlayerControllerB>();
            ulong playerSteamId = component.playerSteamId;
            string playerSteamName = component.playerUsername;
            int suitId = component.currentSuitID;
            string model_name = StartOfRound.Instance.unlockablesList.unlockables[suitId].unlockableName;
            if (model_name == "Default") { model_name = "Orange suit"; }

            string xmlName = playerSteamId.ToString() + ".xml";

            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Avatars", xmlName)))
            {
                Debug.LogWarning("Blend Shapes Found For " + playerSteamName + "(" + playerSteamId.ToString() + "). Checking Settings For '" + model_name + "'...");
                var xmlFile = new XmlDocument();
                xmlFile.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Avatars", xmlName));

                string suitName = String.Concat(model_name.Where(c => !Char.IsWhiteSpace(c)));
                var suitData = xmlFile.SelectSingleNode("/root/" + suitName);
                if (suitData != null)
                {
                    var blendData = xmlFile.SelectSingleNode("/root/" + suitName + "/BlendShapes");
                    if (blendData != null)
                    {
                        bool stop = false;
                        int i = 1;
                        while (stop == false)
                        {
                            var blendShapeData = xmlFile.SelectSingleNode("/root/" + suitName + "/BlendShapes/Default/Blend" + i.ToString());
                            if (blendShapeData != null)
                            {
                                string[] blendShape = blendShapeData.InnerText.Split("%/");
                                replacementModel.GetComponentInChildren<SkinnedMeshRenderer>().SetBlendShapeWeight(Int32.Parse(blendShape[0]), float.Parse(blendShape[1]));
                                i++;
                            } else
                            {
                                stop = true;
                            }
                        }

                        if (emoteId == 1)
                        {
                            bool stop2 = false;
                            int j = 1;
                            while (stop2 == false)
                            {
                                var blendShapeEmoteData = xmlFile.SelectSingleNode("/root/" + suitName + "/BlendShapes/Emote1/Blend" + j.ToString());
                                if (blendShapeEmoteData != null)
                                {
                                    string[] blendShape = blendShapeEmoteData.InnerText.Split("%/");
                                    replacementModel.GetComponentInChildren<SkinnedMeshRenderer>().SetBlendShapeWeight(Int32.Parse(blendShape[0]), float.Parse(blendShape[1]));
                                    j++;
                                }
                                else
                                {
                                    stop2 = true;
                                }
                            }
                        }
                        if (emoteId == 2)
                        {
                            bool stop2 = false;
                            int j = 1;
                            while (stop2 == false)
                            {
                                var blendShapeEmoteData = xmlFile.SelectSingleNode("/root/" + suitName + "/BlendShapes/Emote2/Blend" + j.ToString());
                                if (blendShapeEmoteData != null)
                                {
                                    string[] blendShape = blendShapeEmoteData.InnerText.Split("%/");
                                    replacementModel.GetComponentInChildren<SkinnedMeshRenderer>().SetBlendShapeWeight(Int32.Parse(blendShape[0]), float.Parse(blendShape[1]));
                                    j++;
                                }
                                else
                                {
                                    stop2 = true;
                                }
                            }
                        }
                    } else
                    {
                        Debug.LogWarning("No BlendShapes Found For '" + model_name + "'");
                    }
                }
            }
        }

        protected override void OnEmoteEnd()
        {
            PlayerControllerB component = ((Component)this).gameObject.GetComponent<PlayerControllerB>();
            ulong playerSteamId = component.playerSteamId;
            string playerSteamName = component.playerUsername;
            int suitId = component.currentSuitID;
            string model_name = StartOfRound.Instance.unlockablesList.unlockables[suitId].unlockableName;
            if (model_name == "Default") { model_name = "Orange suit"; }

            string xmlName = playerSteamId.ToString() + ".xml";

            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Avatars", xmlName)))
            {
                var xmlFile = new XmlDocument();
                xmlFile.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Avatars", xmlName));

                string suitName = String.Concat(model_name.Where(c => !Char.IsWhiteSpace(c)));
                var suitData = xmlFile.SelectSingleNode("/root/" + suitName);

                if (suitData != null)
                {
                    var blendData = xmlFile.SelectNodes("/root/" + suitName + "/BlendShapes");
                    if (blendData != null)
                    {
                        bool stop = false;
                        int i = 1;
                        while (stop == false)
                        {
                            var blendShapeData = xmlFile.SelectSingleNode("/root/" + suitName + "/BlendShapes/Default/Blend" + i.ToString());
                            if (blendShapeData != null)
                            {
                                string[] blendShape = blendShapeData.InnerText.Split("%/");
                                replacementModel.GetComponentInChildren<SkinnedMeshRenderer>().SetBlendShapeWeight(Int32.Parse(blendShape[0]), float.Parse(blendShape[1]));
                                i++;
                            }
                            else
                            {
                                stop = true;
                            }
                        }
                    }
                }
            }
        }

    }
}
