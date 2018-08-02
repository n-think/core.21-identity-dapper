﻿namespace AspNetCoreIdentityExample.Domain.Entities.Identity
{
    public class UserLogin : UserLoginKey
    {
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }
    }

    public class UserLoginKey
    {
        public string LoginProvider;
        public string ProviderKey;
    }
}
