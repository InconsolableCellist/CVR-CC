namespace CVR_CC {
    public class TimelineEvent  { 
        public enum EVENT_TYPE : ushort {
            SPEECH_START,       // speech caption beginning
            SPEECH_END,         // speech caption ending      
            DESC_START,         // action descriptor beginning
            DESC_END,           // action descriptor ending
            CC_START,           // generic caption beginning (unknown type)
            CC_END,             // generic caption ending (unknown type)
        }

        public TimelineEvent(EVENT_TYPE type, string eventText, int subNumber, long time) {
            this.type = type;
            this.eventText = eventText;
            this.subNumber = subNumber;
            this.time = time;
        }

        public long time;
        public EVENT_TYPE type;
        public string eventText;
        public int subNumber;
    }
}
