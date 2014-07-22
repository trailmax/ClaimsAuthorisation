using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;
using ClaimsAuth.Infrastructure.Configuration;
using ClaimsAuth.Infrastructure.Identity;
using ClaimsAuth.Models;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ClaimsAuth.Infrastructure
{
    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext()
            : base(ConfigurationContext.Current.GetDatabaseConnectionString(), throwIfV1Schema: false)
        {
        }


        public IDbSet<RoleClaim> RoleClaims { get; set; }



        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException exception)
            {
                var sb = new StringBuilder();

                sb.AppendLine(exception.Message);

                foreach (var validationError in exception.EntityValidationErrors)
                {
                    sb.AppendLine(
                        String.Format("Entity \"{0}\" in state \"{1}\", errors:",
                                      validationError.Entry.Entity.GetType().Name,
                                      validationError.Entry.State));

                    foreach (var error in validationError.ValidationErrors)
                    {
                        sb.AppendLine(
                            String.Format("\t(Property: \"{0}\", Error: \"{1}\")",
                                          error.PropertyName, error.ErrorMessage));
                    }
                }

                throw new DbEntityValidationException(sb.ToString(), exception);
            }
        }
    }
}