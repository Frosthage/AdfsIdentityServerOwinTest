using Microsoft.Owin;
using Owin;
using Configuration;
using IdentityServer3.Core.Configuration;
using Serilog;
using IdentityServer3.Host.Config;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.WsFederation;

[assembly: OwinStartup(typeof(WebHost.Startup))]

namespace WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace(outputTemplate: "{Timestamp} [{Level}] ({Name}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            var factory = new IdentityServerServiceFactory()
                        .UseInMemoryUsers(Users.Get())
                        .UseInMemoryClients(Clients.Get())
                        .UseInMemoryScopes(Scopes.Get());

            var options = new IdentityServerOptions
            {
                SigningCertificate = Certificate.Load(),
                Factory = factory,
                AuthenticationOptions = new AuthenticationOptions()
                {
                    
                }
               
            };

            app.UseCors(CorsOptions.AllowAll);
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(@"log.txt")
                .CreateLogger();
            
            ConfigureIdentityProviders(app, "Cookie");
            
            app.Map("/core", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(options);
            });
        }

        public static void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
           
            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com",
                ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo"
            };
            app.UseGoogleAuthentication(google);
            
            app.UseWsFederationAuthentication(
                new WsFederationAuthenticationOptions
                {
                    Wtrealm = "https://priveravardgivaremikael.vgregion.se/",
                    MetadataAddress =
                        "https://win-i5vs66s3gnb.priveramikael.com/federationmetadata/2007-06/federationmetadata.xml",
                    AuthenticationType = "adfs",
                    //   CallbackPath = new PathString("/core/callback"),

                    Caption = "ADFS",
                    SignInAsAuthenticationType = signInAsType
                });

            
        }
    }
}