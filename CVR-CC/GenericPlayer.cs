using System;
using ABI_RC.VideoPlayer.Scripts;
using ABI.CCK.Components;
using UnityEngine;

namespace CVR_CC { 
    public class GenericPlayer { 
        private readonly CVRVideoPlayer _videoPlayer;
        
        public GenericPlayer(CVRVideoPlayer videoPlayer) {
            _videoPlayer = videoPlayer;
        }
        
        public double GetTime() {
            if (_videoPlayer != null) {
                return _videoPlayer.VideoPlayer.Time;
            }
            
            return -1;
        }
        
        public TrackedPlayer.PlayerState GetPlayerState() { 
            if (_videoPlayer != null) {
                return (_videoPlayer.VideoPlayer.Info.GetState() == VideoPlayerUtils.PlayerState.Playing) 
                    ? TrackedPlayer.PlayerState.Play 
                    : TrackedPlayer.PlayerState.Pause;
            }
            
            return TrackedPlayer.PlayerState.Unknown;
        }
        
        public bool Equals(GameObject gameObject) {
            return ReferenceEquals(_videoPlayer.gameObject, gameObject);
        }
    }
}
