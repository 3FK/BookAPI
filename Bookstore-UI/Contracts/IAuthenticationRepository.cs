﻿using System;
using System.Threading.Tasks;
using Bookstore_UI.Models;

namespace Bookstore_UI.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<bool> Register(RegistrationModel user);
        public Task<bool> Login(LoginModel user);
        public Task Logout();
    }
}
