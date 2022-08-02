using System;
using System.Reflection;
using ABI.CCK.Components;
using HarmonyLib;
using MelonLoader;

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonInfo(typeof(CVR_CC.CVR_CC), "CVR-CC", "0.1", "Foxipso and BenacleJames")]

namespace CVR_CC 
{
    public class CVR_CC : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("CVR-CC has been loaded!");
            base.OnApplicationStart();
            
            HarmonyInstance.Patch(typeof(CVRVideoPlayer).GetMethod("SetUrl", new Type[] { typeof(string) }), 
                prefix: new HarmonyMethod(typeof(CVR_CC).GetMethod("OnSetUrl", BindingFlags.Static | BindingFlags.Public))); // never calls it
            
            HarmonyInstance.Patch(typeof(CVRVideoPlayer).GetMethod("StartedPlaying"), 
                prefix: new HarmonyMethod(typeof(CVR_CC).GetMethod("OnStartedPlaying", BindingFlags.Static | BindingFlags.Public)));
            
            HarmonyInstance.Patch(typeof(CVRVideoPlayer).GetMethod("Start"), 
                prefix: new HarmonyMethod(typeof(CVR_CC).GetMethod("OnStart", BindingFlags.Static | BindingFlags.Public)));
            
           MethodInfo info = typeof(CVRVideoPlayer).GetMethod("SetUrl", new Type[] { typeof(string) }); // found
           MelonLogger.Msg(info == null 
               ? "CVR-CC: Could not find SetUrl method!" 
               : "CVR-CC: Found SetUrl method!");

           info = typeof(CVRVideoPlayer).GetMethod("SetUrl"); // also found
           MelonLogger.Msg(info == null
               ? "CVR-CC: Could not find SetUrl method again!"
               : "CVR-CC: Found SetUrl method again!");
        }
        
        public static void OnSetUrl(String url) { 
            MelonLogger.Msg("On Set URL called");
            MelonLogger.Msg("URL set to: " + url);
        }
        
        public static void OnStartedPlaying() { 
            MelonLogger.Msg("On Started Playing called");
        }
        
        public static void OnStart() { 
            MelonLogger.Msg("On Start called");
        }

    }
}