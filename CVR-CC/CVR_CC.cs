using System;
using System.Collections.Generic;
using System.Reflection;
using ABI.CCK.Components;
using Harmony;
using MelonLoader;
using UnityEngine;
using HarmonyMethod = HarmonyLib.HarmonyMethod;
using Object = System.Object;

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonInfo(typeof(CVR_CC.CVR_CC), "CVR-CC", "1.0", "Foxipso and BenacleJames")]

namespace CVR_CC 
{
    public class CVR_CC : MelonMod
    {
        public static readonly List<TrackedPlayer> TrackedPlayers = new List<TrackedPlayer>();
        public static readonly List<Action> MainThreadExecutionQueue = new List<Action>();
        

        public override void OnApplicationStart()
        {
            MelonLogger.Msg("CVR-CC Starting");
            Hooks.SetupHooks();
            
            base.OnApplicationStart();
        }
        
        public override void OnSceneWasInitialized(int buildIndex, string sceneName) {
            foreach (var lingeringPlayer in TrackedPlayers)
                lingeringPlayer.Dispose();
            TrackedPlayers.Clear();
            
            foreach (var discoveredPlayer in GameObject.FindObjectsOfType<CVRVideoPlayer>()) {
                MelonLogger.Msg("Discovered CVRVideoPlayer");
                TrackedPlayers.Add(new TrackedPlayer(discoveredPlayer));
            }
        }
        
        public override void OnUpdate() { 
            if (MainThreadExecutionQueue.Count <= 0) return;
            
            MainThreadExecutionQueue[0].Invoke();
            MainThreadExecutionQueue.RemoveAt(0);
        }
    }
}