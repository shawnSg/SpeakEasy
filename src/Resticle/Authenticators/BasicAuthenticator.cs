﻿using System;
using System.Text;

namespace Resticle.Authenticators
{
    public class BasicAuthenticator : IAuthenticator
    {
        private readonly string username;

        private readonly string password;

        public BasicAuthenticator(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public void Authenticate(IRestRequest restRequest)
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Concat(username, ":", password)));
            var authorizationHeader = string.Concat("Basic ", token);

            restRequest.AddHeader("Authorization", authorizationHeader);
        }
    }
}
