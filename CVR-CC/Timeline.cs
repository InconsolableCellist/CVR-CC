using System.Collections.Generic;

namespace CVR_CC {
    /**
     * Implements a timeline that contains a series of events, to control displaying/hiding closed captions
     * Uses a List of TimelineEvents and expects to be called with the current playtime in ms
     * Will return the next Event and its text, if applicable
     */
    public class Timeline
    {
        /* Theory of operation:
          0:00     0:05                                 0:25
          | - - x - | x - - - - - - - - - - - - - - - - - |
          a b   c   d e                                   f
        
         a - first event in listOfEvents
         b - an interval of time, but with no event (not in list)
         c - second event in listOfEvents
         d - current value of lastTickMS
         e - the next event in listOfEvents
         f - the last event in listOfEvents
         
         TimelineEvents are essentially "show this text <text>" or "hide all text"
         
         The timeline is initialized with data
         listOfEvents is populated
         
         Execution of the timeline begins with a call to scrubToTime(time)
         The function detects `a` and sets lastTick to the current elapsed time in milliseconds
         It sets playOffset to lastTick
         It gets the next item in the list and sets nextEvent
         It returns the first TimelineEvent 
         
         Execution continues when scrubToTime(time) is called again (now at `b`)
         The function changes lastTick to the current elapsed time in ms
         The function checks if lastTick - playOffset >= nextEvent.time
            No, so null is returned
            
         Execution continues when scrubToTime(now) is called again and again, until `c`
         The function changes lastTick to the current elapsed time in ms
         The function checks if lastTimeMS - playOffset >= nextEvent.time
            Yes. It gets the next item in the list and sets nextEvent
            That event is returned
         */
        
        private long _lastTick = 0;
        private long _playOffset = 0;
        private int _lastIndex = -1;
        private List<TimelineEvent> listOfEvents = new List<TimelineEvent>();
        
        public Timeline(List<TimelineEvent> listOfEvents) { 
            this.listOfEvents = listOfEvents;
        }
        
        /**
         * <summary>Returns a list of TimelineEvents that occurred between the last position in the timeline and
         * the provided time, in order. If this is the first time it's called, it bases the start time off
         * the beginning of the timeline.</summary>
         * <param name="currentElapsedTimeMS">The new time in the timeline to go to</param>
         * <returns>A List containing all the events between the previously run position and the provided position
         * </returns>
         */
        public List<TimelineEvent> ScrubToTime(long currentElapsedTimeMS) { 
            List<TimelineEvent> events = new List<TimelineEvent>();
            _lastTick = currentElapsedTimeMS;
           
            // If user jumped back, start the search from the beginning
            if (_lastIndex >= 0 && currentElapsedTimeMS < listOfEvents[_lastIndex].time) { 
                _lastIndex = -1;
                // TODO: could be improved with a recursive function doing a binary search to find the event with the 
                // closest timecode, then scrub back 1 
            }
           
            for (int i=_lastIndex+1; i < listOfEvents.Count-1; i++) { 
                if (currentElapsedTimeMS >= listOfEvents[i].time) { 
                    events.Add(listOfEvents[i]);
                    _lastIndex = i;
                } else {
                    break;
                }
            }
            return events;
        }
        
        public void ResetTimeline() { 
            _lastIndex = -1;    
            _playOffset = 0;
            _lastTick = 0;
        }
        
    }

}
