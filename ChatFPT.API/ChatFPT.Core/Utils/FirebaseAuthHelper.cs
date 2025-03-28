

using Google.Apis.Auth.OAuth2;

namespace ChatFPT.Core.Utils
{
   
        public class FirebaseAuthHelper
        {
            private readonly GoogleCredential _credential;

            public FirebaseAuthHelper(GoogleCredential credential)
            {
                _credential = credential ?? throw new ArgumentNullException(nameof(credential));
            }

            public async Task<string> GetAccessTokenAsync()
            {
                return await _credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            }
        }
    }
