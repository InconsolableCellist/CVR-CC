using System;
using System.Reflection;
using System.Security.Policy;
using ABI_RC.VideoPlayer.Scripts.Players;
using ABI.CCK.Components;
using Harmony;
using MelonLoader;

namespace CVR_CC { 
    class Hooks { 
        private static HarmonyInstance _instance = new HarmonyInstance(Guid.NewGuid().ToString());
        
        public static void SetupHooks() {
            // Play
            _instance.Patch(typeof(CVRVideoPlayer).GetMethod("Play", new Type[] { typeof(bool), typeof(string) }), 
                null,
               typeof(Hooks).GetMethod(nameof(OnPlay), 
            BindingFlags.NonPublic | BindingFlags.Static)
                    .ToNewHarmonyMethod());
            
            // StartedPlaying
            _instance.Patch(typeof(CVRVideoPlayer).GetMethod("StartedPlaying", new Type[] { }), 
                null,
               typeof(Hooks).GetMethod(nameof(OnStartedPlaying), 
            BindingFlags.NonPublic | BindingFlags.Static)
                    .ToNewHarmonyMethod());
            
            
            // Pause
            _instance.Patch(typeof(CVRVideoPlayer).GetMethod("Pause", new Type[] { typeof(bool), typeof(string) }), 
                null,
               typeof(Hooks).GetMethod(nameof(OnPause), 
            BindingFlags.NonPublic | BindingFlags.Static)
                    .ToNewHarmonyMethod());
            
            // SetVideoURL
            _instance.Patch(typeof(CVRVideoPlayer).GetMethod(nameof(CVRVideoPlayer.SetVideoUrl)),
                null,
                typeof(Hooks).GetMethod(nameof(OnSetVideoUrl),
            BindingFlags.NonPublic | BindingFlags.Static)
                    .ToNewHarmonyMethod());
            
            // FinishedPlaying
            _instance.Patch(typeof(CVRVideoPlayer).GetMethod("FinishedPlaying"),
                null,
                typeof(Hooks).GetMethod(nameof(OnFinishedPlaying),
            BindingFlags.NonPublic | BindingFlags.Static)
                    .ToNewHarmonyMethod());
            
        }
        
        private static void OnStartedPlaying(CVRVideoPlayer __instance) { 
            MelonLogger.Msg("On Started Playing called");
            var foundPlayer = CVR_CC.TrackedPlayers.Find(player => player.Equals(__instance.gameObject));
            foundPlayer?.OnStateChange(TrackedPlayer.PlayerState.Play);
            if (foundPlayer == null) { 
                MelonLogger.Error("Did not find player");
            }
        }
        
        private static void OnPlay(CVRVideoPlayer __instance, bool broadcast, string username) { 
            MelonLogger.Msg("On Play called");
            var foundPlayer = CVR_CC.TrackedPlayers.Find(player => player.Equals(__instance.gameObject));
            foundPlayer?.OnStateChange(TrackedPlayer.PlayerState.Play);
            if (foundPlayer == null) { 
                MelonLogger.Error("Did not find player");
            }
        }
        
        private static void OnPause(CVRVideoPlayer __instance, bool broadcast, string username) { 
            MelonLogger.Msg("On Pause called");
            var foundPlayer = CVR_CC.TrackedPlayers.Find(player => player.Equals(__instance.gameObject));
            foundPlayer?.OnStateChange(TrackedPlayer.PlayerState.Pause);
            if (foundPlayer == null) { 
                MelonLogger.Error("Did not find player");
            }
        }
        
        private static void OnFinishedPlaying(CVRVideoPlayer __instance) { 
            MelonLogger.Msg("On Finished Playing called");
            var foundPlayer = CVR_CC.TrackedPlayers.Find(player => player.Equals(__instance.gameObject));
            foundPlayer?.OnStateChange(TrackedPlayer.PlayerState.Stop);
            if (foundPlayer == null) { 
                MelonLogger.Error("Did not find player");
            }
        }
        
        private static void OnSetVideoUrl(CVRVideoPlayer __instance, String url, bool broadcast, string objPath, string username, bool isPaused) { 
            MelonLogger.Msg("OnSetVideoUrl called");
            MelonLogger.Msg("\tURL set to: " + url);
            var foundPlayer = CVR_CC.TrackedPlayers.Find(player => player.Equals(__instance.gameObject));
            foundPlayer?.OnURLChange(url);
            if (foundPlayer == null) { 
                MelonLogger.Error("Did not find player");
            }
            if (!isPaused) { 
                OnPlay(__instance, broadcast, username);
            }
        }
    }
}