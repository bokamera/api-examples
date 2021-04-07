using System.Runtime.Serialization;

namespace BokaMera.API.Samples.Model
{
    [DataContract]
    public class TokenResponse
    {
        [DataMember(Name="access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name="expires_in")]
        public int ExpiresIn { get; set; }
        
        [DataMember(Name="refresh_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
        
        [DataMember(Name="refresh_token")]
        public string RefreshToken { get; set; }
        
        [DataMember(Name="token_type")]
        public string TokenType { get; set; }
        
        [DataMember(Name="not-before-policy")]
        public int NotBeforePolicy { get; set; }
        
        [DataMember(Name="session_state")]
        public string SessionState { get; set; }
        
        [DataMember(Name="scope")]
        public string Scope { get; set; }
    }
}