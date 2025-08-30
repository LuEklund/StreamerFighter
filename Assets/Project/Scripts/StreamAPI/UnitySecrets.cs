using System.Diagnostics.CodeAnalysis;
namespace StreamAPI {
    [SuppressMessage( "ReSharper", "InconsistentNaming" )]
    [System.Serializable] 
    public class UnitySecrets {
        public string USERNAME;
        public string ACCESS_TOKEN;
        public string REFRESH_TOKEN;
        public string CLIENT_ID;
        public string CLIENT_SECRET;
    }
}