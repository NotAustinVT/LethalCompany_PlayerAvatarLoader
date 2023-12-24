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

        protected override void AddModelScripts()
        {
            //Bruh how the fuck do you read a json?

            /*PlayerControllerB component = ((Component)this).gameObject.GetComponent<PlayerControllerB>();
            ulong playerSteamId = component.playerSteamId;
            string playerSteamName = component.playerUsername;
            int suitId = component.currentSuitID;
            string model_name = StartOfRound.Instance.unlockablesList.unlockables[suitId].unlockableName;
            if (model_name == "Default") { model_name = "Orange suit"; }

            string jsonName = playerSteamId.ToString() + ".json";

            if (File.Exists(Path.Combine(Paths.GameRootPath, "Avatars", jsonName)))
            {
                Debug.LogWarning("Dynamic Bones Found For " + playerSteamName + "(" + playerSteamId.ToString() + "). Checking Settings For '" + model_name + "'...");
                string jsonText = File.ReadAllText(Path.Combine(Paths.GameRootPath, "Avatars", jsonName));
                var jsonData = JsonConvert.DeserializeObject<DynBones>(jsonText);
                Debug.LogWarning(jsonData.Suits.ToString());
                Debug.LogWarning(jsonData.SuitData.ToString());
            }
            else
            {
                Debug.LogError("No Dynamic Bones Found For " + playerSteamName + "(" + playerSteamId.ToString() + "). Skipping Step.");
            }*/
        }

    }

}
