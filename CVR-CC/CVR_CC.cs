using System;
using System.Reflection;
using ABI.CCK.Components;
using Harmony;
using MelonLoader;
using HarmonyMethod = HarmonyLib.HarmonyMethod;

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonInfo(typeof(CVR_CC.CVR_CC), "CVR-CC", "0.1", "Foxipso and BenacleJames")]

namespace CVR_CC 
{
    public class CVR_CC : MelonMod
    {
        private HarmonyInstance _instance = new HarmonyInstance(Guid.NewGuid().ToString());
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            
        }

        public override void OnApplicationStart()
        {
            MelonLogger.Msg("CVR-CC has been loaded!");
            
            _instance.Patch(typeof(CVRVideoPlayer).GetMethod(nameof(CVRVideoPlayer.SetVideoUrl)),
                null,
                typeof(CVR_CC).GetMethod(nameof(OnSetVideoUrl),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .ToNewHarmonyMethod());
            
            MelonLogger.Msg("Patching complete");
            base.OnApplicationStart();
        }

        private static void OnSetVideoUrl(String url, bool broadcast, string objPath, string username, bool isPaused) { 
            MelonLogger.Msg("OnSetVideoUrl called");
            MelonLogger.Msg("URL set to: " + url);
            MelonLogger.Msg("Broadcast: " + broadcast);
            MelonLogger.Msg("ObjPath: " + objPath);
            MelonLogger.Msg("Username: " + username);
            MelonLogger.Msg("IsPaused: " + isPaused);
            
        }
        
        public static void OnStartedPlaying() { 
            MelonLogger.Msg("On Started Playing called");
        }
        
        public static void OnStart() { 
            MelonLogger.Msg("On Start called");
        }

    }
}