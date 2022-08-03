namespace CVR_CC { 
    public class VideoUri {
        private readonly string _videoUri;

        public VideoUri(string uri) => _videoUri = uri;

        public string GetFileName() => System.IO.Path.GetFileNameWithoutExtension(_videoUri);
    }
}
