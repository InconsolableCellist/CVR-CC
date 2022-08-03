using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CVR_CC { 
    public static class SRTDecoder  {
        private enum States : ushort { 
            CARD_NUM,
            TIMECODE,
            CONTENT,
        }
        
        private static List<TimelineError> _errors;
        
        /**
         * <summary>Returns a List of TimelineErrors (if any) encountered on the previous decoding call.
         * Each TimelineError may contain more information in `additionalInfo`.</summary>
         * 
         * <returns>The List of TimelineError Objects</returns>
         */
        public static List<TimelineError> GETErrorsEncounteredDuringDecoding() { 
                return _errors;
        }
        
        /**
         * <summary>Given a valid SRT file in srtString, converts it to a List of TimelineEvents representing the
         * closed caption data. Errors encountered during the parsing attempt will be accessible by calling
         * GETErrorsEncounteredDuringDecoding() after calling this function, and will be returned in the form of
         * a List of TimelineError objects. Calling this function will clear any previously stored errors.
         *
         * Regardless of the type of expected error, parsing will attempt to continue, but may result in malformed
         * TimelineEvents or undefined behavior.
         *
         * Parsing will not change or strip any input markup, such as HTML, with the exception of leading or
         * trailing whitespace, which will be stripped.</summary>
         *
         * <param name="srtString">The string containing the valid SRT data</param>
         * <returns>A List of TimelineEvents, in order</returns>
         */
        public static List<TimelineEvent> DecodeSrtIntoTimelineEvents(String srtString) {
            List<TimelineEvent> events = new List<TimelineEvent>();
            _errors = new List<TimelineError>();
            
            StringReader sr = new StringReader(srtString);
            States state = States.CARD_NUM; 
            string line = "";
            int cardNum = 0;
            long startTime = 0;
            long endTime = 0;
            string content = "";
            TimelineEvent te = null;
            Regex timecodeRegex = new Regex(@"^([\d:,\.]+) --> ([\d:,\.]+)$");
                                                // 00:00:06,000 --> 00:00:12.074
            Regex timeformatRegex = new Regex(@"^(\d\d):(\d\d):(\d\d)[,\.](\d\d\d)$");
                                                // 00:00:06,000 
                                                // HH:MM:SS,sss or HH:MM:SS.sss
            
            while ((line = sr.ReadLine()) != null) { 
                switch (state) { 
                    case States.CARD_NUM: 
                        if (!int.TryParse(line.Trim(), out cardNum))
                            _errors.Add(new TimelineError(TimelineError.Type.CANT_PARSE_CARDNUM));
                        state = States.TIMECODE;
                        break;
                    
                    case States.TIMECODE:
                        try {
                            Match match = timecodeRegex.Match(line);
                            Match match2 = timeformatRegex.Match(match.Groups[1].Value);
                            startTime = int.Parse(match2.Groups[1].Value) * 60 * 60 * 1000; // Hours to ms
                            startTime += int.Parse(match2.Groups[2].Value) * 60 * 1000;     // Mins to ms
                            startTime += int.Parse(match2.Groups[3].Value) * 1000;          // Secs to ms
                            startTime += int.Parse(match2.Groups[4].Value);                 // ms
                            
                            match2 = timeformatRegex.Match(match.Groups[2].Value);
                            endTime = int.Parse(match2.Groups[1].Value) * 60 * 60 * 1000;   // ditto
                            endTime += int.Parse(match2.Groups[2].Value) * 60 * 1000;
                            endTime += int.Parse(match2.Groups[3].Value) * 1000;
                            endTime += int.Parse(match2.Groups[4].Value);
                        } catch (Exception e) { 
                            _errors.Add(new TimelineError(TimelineError.Type.TIMECODE_REGEX_OR_FORMAT_FAILURE, e.ToString()));
                        }
                        state = States.CONTENT;
                        break;
                    
                    case States.CONTENT:
                        if (line.Trim() == "") { 
                            te = new TimelineEvent(TimelineEvent.EVENT_TYPE.CC_START, content,
                                cardNum, startTime);
                            events.Add(te);
                            
                            te = new TimelineEvent(TimelineEvent.EVENT_TYPE.CC_END, "",
                                cardNum, endTime);
                            events.Add(te);
                            state = States.CARD_NUM;
                            content = "";
                        } else { 
                            content += '\n' + line;
                        }
                        break;
                    default:
                        break;
                }
            }
            return events;
        }
    }
}
