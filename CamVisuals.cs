using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Dissonance;
using GameNetcodeStuff;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using ModelReplacement;
using ModelReplacement.AvatarBodyUpdater;
using UnityEngine;

namespace CamVisuals
{
    [BepInPlugin("com.majentic.owowlkcamera", "OWOwlkModel", "0.2.0")]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    public class CameraVisuals : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("CameraVisuals");

        private static CameraVisuals Instance;

        public static Vector3 baseSize;

        public static ManualLogSource mls;

        public static bool usingCameraVisuals;

        private void Awake()
        {
            if ((UnityEngine.Object)(object)Instance == null)
            {
                Instance = this;
            }
            mls = this.Logger;
            harmony.PatchAll(typeof(CameraVisuals));
            mls.LogInfo((object)"CAMERA HEIGHTS LOADED");
        }

        //[HarmonyPatch(typeof(AvatarUpdater), "LoadModelReplacement")]
        //[HarmonyPostfix]
        //public static void SetModelHeight(ref AvatarUpdater __instance)
        //{
        //    __instance.rootPositionOffset.transform.localPosition = new Vector3(0f, Plugin.valueC.Value, 0f);
        //    mls.LogInfo("Set the thing");
        //}

        //[HarmonyPatch(typeof(ModelReplacement.Scripts.Player.ViewModelUpdater), "AssignViewModelReplacement")]
        //[HarmonyPostfix]
        //public static void SetModelHeight(GameObject replacementViewModel)
        //{
        //    replacementViewModel.transform.localPosition = new Vector3(0f, Plugin.valueA.Value, 0f);
        //    mls.LogInfo("Set the thing");
        //}

        [HarmonyPatch(typeof(ModelReplacementAPI), "SetPlayerModelReplacement")]
        [HarmonyPostfix]
        public static void DetectCamera(PlayerControllerB player)
        {
            GameObject gameObject = ((Component)player).gameObject;
            Transform val = gameObject.transform.Find("ScavengerModel").Find("metarig").Find("CameraContainer");
            if ((UnityEngine.Object)(object)val != null)
            {
                GameObject gameObject2 = ((Component)val).gameObject;
                if ((UnityEngine.Object)(object)gameObject.GetComponent<MROWPRISONER>())
                {
                    gameObject.transform.Find("ScavengerModel").Find("metarig").Find("CameraContainer")
                        .Find("MainCamera")
                        .Find("HUDHelmetPosition")
                        .localPosition = new Vector3(0.01f, -0.036f, -0.063f);
                    //gameObject.transform.localScale = Vector3.one * Plugin.valueA.Value;
                    gameObject.GetComponent<BodyReplacementBase>().replacementModel.transform.localScale = Vector3.one/* * Plugin.valueB.Value*/;
                    gameObject.GetComponent<BodyReplacementBase>().transform.localScale = Vector3.one * 1.33f/* Plugin.valueC.Value*/;
                    usingCameraVisuals = true;
                }
                else if (!(UnityEngine.Object)(object)gameObject.GetComponent<MROWPRISONER>() && usingCameraVisuals)
                {
                    gameObject.transform.Find("ScavengerModel").Find("metarig").Find("CameraContainer")
                        .Find("MainCamera")
                        .Find("HUDHelmetPosition")
                        .localPosition = new Vector3(0.01f, -0.048f, -0.063f);
                    //gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    usingCameraVisuals = false;
                }
            }
            else
            {
                mls.LogInfo((object)"Child GameObject not found.");
            }
        }

        [HarmonyPatch(typeof(ModelReplacementAPI), "RemovePlayerModelReplacement")]
        [HarmonyPostfix]
        public static void ResetCamera(PlayerControllerB player)
        {
            GameObject gameObject = ((Component)player).gameObject;
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            gameObject.transform.Find("ScavengerModel").Find("metarig").Find("CameraContainer")
                .Find("MainCamera")
                .Find("HUDHelmetPosition")
                .localPosition = new Vector3(0.01f, -0.048f, -0.063f);
            gameObject.transform.Find("ScavengerModel").localEulerAngles = new Vector3(0f, 0f, 0f);
        }

        [HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
        [HarmonyPostfix]
        public static void DetectCamera()
        {
            PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
            baseSize = ((Component)localPlayerController).gameObject.GetComponent<BodyReplacementBase>().transform.localScale;
            if (baseSize == Vector3.one * Plugin.valueC.Value)
            {
                ((Component)localPlayerController).gameObject.transform.Find("ScavengerModel/metarig/CameraContainer/MainCamera").localPosition = new Vector3(0f, -0.5f, -0.25f/*Plugin.valueD.Value, Plugin.valueE.Value*/);
            }
        }

        [HarmonyPatch(typeof(Terminal), "QuitTerminal")]
        [HarmonyPostfix]
        public static void ResetCamera()
        {
            PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
            ((Component)localPlayerController).gameObject.transform.Find("ScavengerModel/metarig/CameraContainer/MainCamera").localPosition = new Vector3(0f, 0f, 0f);
        }
    }
}