using System;


namespace ClaimsAuth.Infrastructure.Identity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClaimsGroupAttribute : Attribute
    {
        public String Name { get;  private set; }

        public ClaimsGroupAttribute(String name)
        {
            Name = name;
        }
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ClaimsActionAttribute : Attribute
    {
        public ClaimsActions Claim { get; private set; }

        public ClaimsActionAttribute(ClaimsActions claim)
        {
            Claim = claim;
        }
    }

    public enum ClaimsActions
    {
        Index,
        View,
        Create,
        Edit,
        Delete
    }
}