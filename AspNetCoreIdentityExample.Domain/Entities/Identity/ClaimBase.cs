﻿namespace AspNetCoreIdentityExample.Domain.Entities.Identity
{
    public abstract class ClaimBase
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
