using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CVR_CC {
    [SuppressMessage("ReSharper", "UnassignedField.Global")]    // Shaddap, Rider
    public class Subtitle {
        public string MovieName;
        public string LanguageName;
        public string SubHearingImpaired;
        public string SubDownloadLink;
        public double Score;
        public string MovieYear;
        public string MovieByteSize;
        public List<Subtitle> Alternatives;
    }
}
