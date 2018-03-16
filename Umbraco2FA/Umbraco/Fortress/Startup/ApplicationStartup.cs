using Orc.Fortress.Database.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Orc.Fortress.Startup
{
    public class ApplicationStartup : ApplicationEventHandler
    {
        //This happens everytime the Umbraco Application starts
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            //Get the Umbraco Database context
            var ctx = applicationContext.DatabaseContext;
            var db = new DatabaseSchemaHelper(ctx.Database, applicationContext.ProfilingLogger.Logger, ctx.SqlSyntax);
            //Check if the DB table does NOT exist
            if (!db.TableExist(FortressConstants.TableNames.FortressUser2FASettings))
            {
                //Create DB table - and set overwrite to false
                db.CreateTable<FortressUser2FASettings>(false);
            }
         /*   if (!db.TableExist(FortressConstants.TableNames.FortressLoginEvents))
            {
                //Create DB table - and set overwrite to false
                db.CreateTable<FortressLoginEvent>(false); 
            }*/
            if (!db.TableExist(FortressConstants.TableNames.FortressSettings))
            {
                //Create DB table - and set overwrite to false
                db.CreateTable<FortressSettingEntry>(false);

                FortressSettingEntry.InsertInitialSettings(ctx.Database, ApplicationContext.Current.DatabaseContext.SqlSyntax);
            }
          /*  if (!db.TableExist(FortressConstants.TableNames.FortressFirewallEntry))
            {
                //Create DB table - and set overwrite to false
                db.CreateTable<FortressFirewallEntry>(false);
            }*/
            FortressContext.Initialize();
        }
    }
}