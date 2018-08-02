namespace AspNetCoreIdentityExample.Domain.Entities.Identity
{
    public class UserToken : UserTokenKey
    {
        public string Value { get; set; }
    }

    public class UserTokenKey
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
    }
}
