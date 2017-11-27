// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace QuickstartIdentityServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();
            
            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());
            
            
            services.AddAuthentication()
                .AddWsFederation(options =>
                {
                    options.Wtrealm = "https://priveravardgivaremikael.vgregion.se/";
                    options.MetadataAddress = "https://win-i5vs66s3gnb.priveramikael.com/federationmetadata/2007-06/federationmetadata.xml";
                    options.UseTokenLifetime = true;
                    //options.CallbackPath = new PathString("/ExternalLoginCallback");
                });
            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x.AllowAnyOrigin());
            app.UseIdentityServer();
            
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}