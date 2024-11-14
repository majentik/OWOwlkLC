using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using ModelReplacement;
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

        //public static bool usingCameraVisuals;

        //public static float[] colliderDims = { };

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

        //private static void TreePrintObjects(GameObject cur, int i)
        //{
        //    mls.LogInfo(i + ": " + cur.name + " (" + cur.transform.parent.name + ')');
        //    foreach (Transform t in cur.transform)
        //        TreePrintObjects(t.gameObject, i + 1);
        //}

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        public static void DetectCamera(PlayerControllerB __instance)
        {
            GameObject gameObject = ((Component)__instance).gameObject;
            Transform val = gameObject.transform.Find("ScavengerModel").Find("metarig").Find("CameraContainer");
            if ((UnityEngine.Object)(object)val != null)
            {
                GameObject gameObject2 = ((Component)val).gameObject;
                //CharacterController playerCollider = player.thisController;
                //if (colliderDims.Length == 0 && !usingCameraVisuals)
                //{
                //    colliderDims = new float[] { playerCollider.radius, playerCollider.stepOffset, playerCollider.skinWidth };
                //    mls.LogInfo(colliderDims.ToString());
                //    TreePrintObjects(gameObject, 0);
                //}
                if ((UnityEngine.Object)(object)gameObject.GetComponent<MROWPRISONER>() && Plugin.bigBird.Value)
                {
                    gameObject.transform.Find("ScavengerModel").Find("metarig").Find("CameraContainer")
                        .Find("MainCamera")
                        .Find("HUDHelmetPosition")
                        .localPosition = new Vector3(0.01f, -0.036f, -0.063f);
                    //gameObject.transform.localScale = Vector3.one * Plugin.valueA.Value;
                    gameObject.GetComponent<BodyReplacementBase>().replacementModel.transform.localScale = Vector3.one/* * Plugin.valueB.Value*/;
                    gameObject.transform.localScale = Vector3.one * 1.33f/* Plugin.valueC.Value*/;
                    //usingCameraVisuals = true;
                }
                else if (gameObject.transform.localScale == Vector3.one * 1.33f)
                {
                    gameObject.transform.Find("ScavengerModel").Find("metarig").Find("CameraContainer")
                        .Find("MainCamera")
                        .Find("HUDHelmetPosition")
                        .localPosition = new Vector3(0.01f, -0.048f, -0.063f);
                    if ((UnityEngine.Object)(object)gameObject.GetComponent<MROWPRISONER>())
                        gameObject.GetComponent<BodyReplacementBase>().replacementModel.transform.localScale = Vector3.one;
                    gameObject.transform.localScale = Vector3.one;
                    //usingCameraVisuals = false;
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
            gameObject.transform.localScale = Vector3.one;
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
            BodyReplacementBase bodyReplacementBase = ((Component)localPlayerController).gameObject.GetComponent<BodyReplacementBase>() ?? null;
            if (bodyReplacementBase != null && bodyReplacementBase.transform.localScale == Vector3.one * 1.33f/*Plugin.valueC.Value*/)
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