

using ChatFPT.Core.Constaints;
using ChatFPT.Core.ExceptionCustom;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using System.Linq.Dynamic.Core.Tokenizer;

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
            try
            {
                var token = await _credential.UnderlyingCredential.GetAccessTokenForRequestAsync("https://www.googleapis.com/auth/firebase.messaging");
                return token;
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstaints.INTERNAL_SERVER_ERROR, "Không lấy được access token");
                throw;
            }
        }
    }
}
