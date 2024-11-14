using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using System;
using System.Xml.Linq;

namespace ModelReplacement
{
    [BepInPlugin("com.majentic.owowlk", "OWOwlkModel", "0.2.0")]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigFile config;
        //public static ConfigEntry<float> valueA { get; private set; }
        //public static ConfigEntry<float> valueB { get; private set; }
        //public static ConfigEntry<float> valueC { get; private set; }
        //public static ConfigEntry<float> valueD { get; private set; }
        //public static ConfigEntry<float> valueE { get; private set; }
        public static ConfigEntry<bool> bigBird { get; private set; }
        private void Awake()
        {
            config = base.Config;
            //valueA = config.Bind<float>("Value A", "Value A", 1f, "guh");
            //valueB = config.Bind<float>("Value B", "Value B", 1f, "guh");
            //valueC = config.Bind<float>("Value C", "Value C", 1.33f, "guh");
            //valueD = config.Bind<float>("Value D", "Value D", -0.5f, "guh");
            //valueE = config.Bind<float>("Value E", "Value E", -0.25f, "guh");
            bigBird = config.Bind<bool>("Height Change", "Height Change", false, "Makes the Prisoner taller than usual, adjusting camera height accordingly. (This is experimental and buggy. (It will also change your hitbox size. (You will not be able to traverse the mines. (No, I do not know how to fix this.))))");

            Assets.PopulateAssets();
            
            ModelReplacementAPI.RegisterSuitModelReplacement("Owlk", typeof(MROWPRISONER));
                

            Harmony harmony = new Harmony("com.majentic.owowlk");
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {"com.majentic.owowlk"} is loaded!");
        }
    }
    public static class Assets
    {
        // Replace mbundle with the Asset Bundle Name from your unity project 
        public static string mainAssetBundleName = "owprisoner";
        public static AssetBundle MainAssetBundle = null;

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().GetName().Name.Replace(" ","_");
        public static void PopulateAssets()
        {
            if (MainAssetBundle == null)
            {
                Console.WriteLine(GetAssemblyName() + "." + mainAssetBundleName);
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + mainAssetBundleName))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }

            }
        }
    }

}