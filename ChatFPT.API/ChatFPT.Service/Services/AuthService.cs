

using ChatFPT.Core.Models.User;
using ChatFPT.Service.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;

namespace ChatFPT.Service.Services
{
    public class AuthService : IAuthService
    {
        private bool _isInitialized;
        public AuthService()
        {
            if (!_isInitialized)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });

                _isInitialized = true;
            }
        }
        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<UserInfoModel> GetUserInfo()
        {
            throw new NotImplementedException();
        }

        public Task Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task LoginGoogle(string token)
        {
            
            }
        }      
}   

