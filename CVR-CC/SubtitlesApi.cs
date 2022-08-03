#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;

namespace CVR_CC {
    public static class SubtitlesApi {
        private static readonly HttpClient WebClient = new HttpClient{DefaultRequestHeaders = {{"User-Agent", 
            "VRC-CC"}}};
        private const int BUFFER_SIZE_BYTES = 1024*1024; // 1MB
        
        private static readonly Dictionary<string, MemoryStream> CachedSRTs = new Dictionary<string, MemoryStream>();

        public static async Task<Subtitle?> QuerySubtitle(string movieName, bool getAlternatives = false) {
            MelonLogger.Msg($"Requesting {movieName}");
            var requestData = new HttpRequestMessage() {
                RequestUri = new Uri("https://8m9ahcccna.execute-api.us-east-1.amazonaws.com/prod"),
                Method = HttpMethod.Get
            };
            requestData.Headers.Add("movie", movieName);
            if (getAlternatives) requestData.Headers.Add("includeAlternatives", "true");
            var request = await 
                WebClient.SendAsync(requestData);
            try {
                if (!request.IsSuccessStatusCode) return null;
                var response = await request.Content.ReadAsStringAsync();
                return JsonUtility.FromJson<Subtitle>(response);
            } catch {
                MelonLogger.Msg("Failed to deserialize result string.");
                return null;
            }
        }
        
        private static MemoryStream? GetSubIfCached(string subtitleURL) {
            MemoryStream? ms = null;
           
            if (CachedSRTs.ContainsKey(subtitleURL)) 
                ms = CachedSRTs[subtitleURL];
            
            return ms;
        }
        
        /**
         * <summary>Given a string that's suspected to contain valid SRT data, returns true or false if it's
         * valid/invalid</summary>
         *
         * <param name="srtString">The SRT string to check</param>
         * <returns>A bool indicating a valid or invalid SRT string</returns>
         */
        private static bool VerifySrt(string srtString) { 
            bool isValid = false;
            
            // TODO: implement properly
            if (srtString.Length > 512) 
                isValid = true;
            
            return isValid;
        }
        
        public static async Task<string> FetchSub(string subtitleURL) {
            MelonLogger.Msg("1-1-1");
            var compressedMs = GetSubIfCached(subtitleURL);
            string srtString = "";
            
            if (compressedMs == null) {
                MelonLogger.Msg("1-1-2");
                HttpResponseMessage request = await WebClient.GetAsync(subtitleURL);
                MelonLogger.Msg("1-1-3");
                byte[] response = await request.Content.ReadAsByteArrayAsync();
                try {
                    MelonLogger.Msg("1-1-4");
                    compressedMs = new MemoryStream(response);
                    CachedSRTs[subtitleURL] = compressedMs;
                MelonLogger.Msg("1-1-5");
                } catch (Exception e) {
                    MelonLogger.Error("An exception occurred while trying to fetch or decode a subtitle file! " + e);
                }
            }
            MelonLogger.Error("CompressedMs is still null.");
            MelonLogger.Msg("1-1-6");
            if (compressedMs == null) return srtString;
            try {
                compressedMs.Seek(0,0);
                MelonLogger.Msg("1-1-7");
                var decompressedMs = new MemoryStream();
                var gzs = new BufferedStream(new GZipStream(compressedMs, CompressionMode.Decompress), 
                    BUFFER_SIZE_BYTES);
                MelonLogger.Msg("1-1-8");
                gzs.CopyTo(decompressedMs);
                srtString = Encoding.UTF8.GetString(decompressedMs.ToArray());
                MelonLogger.Msg("1-1-9");
            } catch (Exception e) { 
                MelonLogger.Error("An exception occurred while trying to decompress, decode, or verify an " +
                                  "encoded subtitle file! " + e);
            }
            
            MelonLogger.Msg("1-1-10");
            if (!VerifySrt(srtString)) { 
                MelonLogger.Error("Retrieved a subtitle file but it doesn't look like a valid SRT.");
                srtString = "";
            } else {
                compressedMs.Seek(0,0);
                MelonLogger.Msg("1-1-11");
                CachedSRTs[subtitleURL] = compressedMs;
                MelonLogger.Msg("1-1-12");
            }
            MelonLogger.Msg("1-1-13");
            return srtString;
        }

        public static async Task<long?> GetFileSize(string url)
        {
            var webResult = await WebClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            return webResult.Content.Headers.ContentLength;
        }
    }
}
