using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimsAuth.Infrastructure.Configuration
{
    public interface IMyConfiguration
    {
        String GetDatabaseConnectionString();
    }
}