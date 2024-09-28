namespace BlazorApp_TeleCRM.Service
{
    using System.Security.Claims;
    using BlazorApp_TeleCRM.Models;
    using Microsoft.AspNetCore.Components.Authorization;

    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private User _currentUser;

        public async Task MarkUserAsAuthenticated(User user)
        {
            _currentUser = user;
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.username),
            new Claim(ClaimTypes.Email, user.email),
            new Claim(ClaimTypes.GivenName, user.firstname + " "+ user.lastname),
            new Claim(ClaimTypes.Role, user.role),
            new Claim(ClaimTypes.Locality, user.organization)
        }, "apiauth_type");

            var userPrincipal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userPrincipal)));
       
        }

        public void MarkUserAsLoggedOut()
        {
            _currentUser = null;
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_currentUser != null)
            {
                var identity = new ClaimsIdentity(new[]
                {
                  new Claim(ClaimTypes.Name, _currentUser.username),
                  new Claim(ClaimTypes.Role,_currentUser.role),
                  new Claim(ClaimTypes.Locality,_currentUser.organization)
            }, "apiauth_type");

                var userPrincipal = new ClaimsPrincipal(identity);
                return Task.FromResult(new AuthenticationState(userPrincipal));
            }
            else
            {
                var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
                return Task.FromResult(new AuthenticationState(anonymous));
            }
        }
    }
}
