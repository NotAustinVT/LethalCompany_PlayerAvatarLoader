using BepInEx;
using BepInEx.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using Zeekerss;
using Zeekerss.Core;
using Zeekerss.Core.Singletons;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;
using GameNetcodeStuff;
using ModelReplacement;

//using System.Numerics;

namespace PlayerAvatarLoader
{



    [BepInPlugin("NotAustinVT.PlayerAvatarLoader", "Player Avatar Loader", "1.0")]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {

        private void Awake()
        {
            // Plugin startup logic
            Plugin.SuitsToReg = base.Config.Bind<string>("Suits - ONLY EDIT FOR SUIT MOD COMPATIBILITY", "Suits", "Orange suit,Green suit,Hazard suit,Pajama suit", "Include all suits being loaded. (If using More_Suits, DO NOT INCLUDE 'Default' IN THE LIST OF SUITS TO LOAD");
            char[] commaSplit = new char[] { ',' };
            string[] suitRegistry = SuitsToReg.Value.Split(commaSplit);

            ModelReplacementAPI.RegisterSuitModelReplacement("Default", typeof(BodyReplacement));
            for ( int i = 0; i < suitRegistry.Length; i++)
            {
                if (suitRegistry[i] != "Default") { ModelReplacementAPI.RegisterSuitModelReplacement(suitRegistry[i], typeof(BodyReplacement)); }
            }

            //ModelReplacementAPI.RegisterSuitModelReplacement("Green suit", typeof(BodyReplacement));
            //ModelReplacementAPI.RegisterSuitModelReplacement("Orange suit", typeof(BodyReplacement));
            //ModelReplacementAPI.RegisterSuitModelReplacement("Pajama suit", typeof(BodyReplacement));
            //ModelReplacementAPI.RegisterSuitModelReplacement("Hazard suit", typeof(BodyReplacement));

            Assets.PopulateAssets(false, "if you see this you're gay", null);

            Harmony harmony = new Harmony("NotAustinVT.AvatarLoader");
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {"NotAustinVT.PlayerAvatarLoader"} is loaded!");
        }



        [HarmonyPatch(typeof(PlayerControllerB))]
        public class PlayerControllerBPatch
        {

            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            public static void UpdatePatch(ref PlayerControllerB __instance)
            {
                if (__instance.playerSteamId == 0) { return; }

            }

        }

        public static readonly Harmony Harmony = new Harmony("NotAustinVT.PlayerAvatarLoader");
        public static ConfigEntry<string> SuitsToReg = null;

    }
    public static class Assets
    {
        public static AssetBundle initAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Avatars", "default"));

        public static List<string> loadedBundleIds = new List<string>();
        public static List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static void PopulateAssets(bool check, string playerSteamId, AssetBundle playerAssetBundle)
        {
            if (check)
            {
                loadedBundleIds.Add(playerSteamId);
                loadedAssetBundles.Add(playerAssetBundle);

            }
        }

    }

    public class DynBones
    {
        /*
        //All Referenced Suits
        public List<string> Suits { get; set; }

        //Motion Data
        public List<string> SuitData { get; set; }

        //Collider Data
        */
    }
}