using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABI_RC.Core.Networking.IO.UserGeneratedContent;
using ABI.CCK.Components;
using MelonLoader;
using UnityEngine;

namespace CVR_CC { 
    public class TrackedPlayer : IDisposable { 
        public enum PlayerState { 
            Play,
            Pause,
            Stop,
            Unknown
        }
        
        private readonly GenericPlayer _storedPlayer;
        private object _coroutineToken;

        public PlayerState currentState;
        private long _msOffset;

        private long CurrentTimeInMs => (long) Math.Round(_storedPlayer.GetTime(), 2)*1000;
        private Timeline _tl;
        
        public string currentMovieName = "";
        
        public TrackedPlayer(CVRVideoPlayer original) { 
            _storedPlayer = new GenericPlayer(original);
            currentState = _storedPlayer.GetPlayerState();
        }
        
        /**
         * <summary>Display of the subtitles can be offset by some number of ms, to account for differences between
         * the content the closed caption file is encoded for, and the actual content being viewed. This function
         * retrieves the current offset (initially 0ms).</summary>
         * <returns>The current offset in ms</returns>
         */
        public long GetCurrentOffsetMs() => _msOffset;

        /**
         * <summary>Increments (or decrements) the current offset by `ms` milliseconds.</summary>
         * <param nmae="ms">A signed long indicating the number of ms to change the offset by. Set to negative to
         * decrement the offset.</param>
         */
        public void IncrementOrDecrementOffset(long ms) => _msOffset += ms;

        public void OnStateChange(PlayerState newState)
        {
            currentState = newState;
            if (currentState == PlayerState.Play)
                _coroutineToken = MelonCoroutines.Start(UpdateSubtitles());
            else if (_coroutineToken != null) 
                MelonCoroutines.Stop(_coroutineToken);
        }
        
        public async void OnURLChange(string newURl)
        {
            MelonLogger.Msg("OnURLChange.");
            var (movieName, timelineEvents) = await FetchSubtitlesForNewUrl(newURl);
            if (timelineEvents == null)
            {
                _tl = null;
                return;
            }
            
            _tl = new Timeline(timelineEvents);
            CVR_CC.MainThreadExecutionQueue.Add(() => MelonCoroutines.Start(
                UITextArea.DisplayAlert($"Starting Subtitle Playback for: {movieName}", 3)));
            currentMovieName = movieName;
        }
        
        /**
         * What happens when you try to swap a timeline that's being accessed in a coroutine? Undefined!
         */
        public void UnsafeSwapTimeline(string movieName, List<TimelineEvent> timelineEvents) { 
            _tl = new Timeline(timelineEvents);
            CVR_CC.MainThreadExecutionQueue.Add(() => MelonCoroutines.Start(
                UITextArea.DisplayAlert($"Starting Subtitle Playback for: {movieName}", 3)));
            currentMovieName = movieName;
        }

        /**
         * Takes a playback URL from a movie world and tries to query for it.
         */
        private static async Task<(string, List<TimelineEvent>)> FetchSubtitlesForNewUrl(string newUrl)
        {
            var uri = new VideoUri(newUrl);
            var title = await SubtitlesApi.QuerySubtitle(uri.GetFileName());
            if (title == null)
            {
                MelonLogger.Msg("Failed to find movie");
                return ("", null);
            }

            var subFile = await SubtitlesApi.FetchSub(title.SubDownloadLink);
            return subFile.Length < 512 ? ("", null) : 
                (title.MovieName, SRTDecoder.DecodeSrtIntoTimelineEvents(subFile));
        }

        private IEnumerator UpdateSubtitles()
        {
            while (_storedPlayer != null)
            {
                try {
                    if (_tl != null && currentState == PlayerState.Play)
                    {
                        UITextArea.ToggleUI(true);
                        List<TimelineEvent> events = _tl.ScrubToTime(CurrentTimeInMs + _msOffset);
                        // Large gaps in time (resync, lag, a big seek) can result in a massive list of missed events.
                        // To prevent this, we only care about the two most recent events
                        if (events.Count > 2) 
                            events.RemoveRange(0, events.Count - 3);
                        
                        foreach (var eventObj in events.Where(eventObj => !eventObj.eventText.Contains("OpenSubtitles")))
                        {
                            MelonLogger.Msg(eventObj.eventText);
                            UITextArea.Text = eventObj.eventText;
                        }
                    }
                    else
                    {
                        UITextArea.Text = "";
                        UITextArea.ToggleUI(false);
                    }
                } catch (Exception e) {
                    MelonLogger.Error("Error in UpdateSubtitles", e);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        public bool Equals(GameObject gameObject) => _storedPlayer.Equals(gameObject);

        ~TrackedPlayer() => Dispose();

        public void Dispose()
        {
            UITextArea.Text = "";
            // if (_coroutineToken != null) MelonCoroutines.Stop(_coroutineToken);
        }
    }
}