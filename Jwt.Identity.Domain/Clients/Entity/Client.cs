using System.ComponentModel.DataAnnotations;
using Jwt.Identity.Domain.User.Enum;
using MediatR;

namespace Jwt.Identity.Domain.Clients.Entity
{
    public class Client
    {
        private string _clientName;
        private string _emailConfirmPage;
        private string _emailResetPage;
        private string _lockout;

        private string _loginUrl;

        private string _signInExternal;
        private string _signOut;

        [Key][Required]  public int ClientId { get; set; }

        public string ClientName
        {
            get => _clientName;
            set => _clientName = value.ToUpper();
        }

        public string EmailConfirmPage
        {
            get => _emailConfirmPage;
            set => _emailConfirmPage = BaseUrl + value;
        }

        public string EmailResetPage
        {
            get => _emailResetPage;
            set => _emailResetPage = BaseUrl + value;
        }

        public string BaseUrl { get; set; }

        public string LoginUrl
        {
            get => _loginUrl;
            set => _loginUrl = BaseUrl + value;
        }

        public string SignInExternal
        {
            get => _signInExternal;
            set => _signInExternal = BaseUrl + value;
        }

        public string SignOut
        {
            get => _signOut;
            set => _signOut = BaseUrl + value;
        }

        public string Lockout
        {
            get => _lockout;
            set => _lockout = BaseUrl + value;
        }

        public LoginType LoginType { get; set; }
    }
}