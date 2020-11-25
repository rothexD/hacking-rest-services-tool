using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;

namespace SecurityHeaderWebApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(mvcOptions => 
                mvcOptions.EnableEndpointRouting = false);

            //Configurating  hsts/ssl rules
            services.AddHsts(options =>    
            {    
                options.IncludeSubDomains = true;    
                options.MaxAge = TimeSpan.FromDays(365);    
            });     
            
            
            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //When in Production we want to have SSL
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            //Settings for Hiding odata /metadata
            var defaultConventions = ODataRoutingConventions.CreateDefault();
            var conventions = defaultConventions.Except(
                defaultConventions.OfType<MetadataRoutingConvention>());

            //Because we are using it as an Api we dont need dependencies from other websites.
            app.UseCsp(opts => opts  
                .BlockAllMixedContent()  
                .DefaultSources(s => s.Self())  
                .ScriptSources(s => s.Self())  
                .StyleSources(s => s.Self())  
                .FontSources(s => s.Self())  
            );

            // preventing Clickjacking attacks
            app.UseXfo(options => options.Deny());  
            
            //Setting XContentType to no sniff to prevent mime sniffing
            app.UseXContentTypeOptions();
            
            //Because we are an API an usually responding with json Data
            //Always send ContentType as json
            //Dont store responses --> sensitive information
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Type", "application/json");
                context.Response.Headers.Add("Cache-Control", "no-store");
                await next.Invoke();
            });
            
            //Set parameters for hiding odata / metdata
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Select().Filter();
                routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel(), 
                    pathHandler: new DefaultODataPathHandler(),    routingConventions:conventions);
            });
        }
        IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            var StudentSet = odataBuilder.EntitySet<Student>("Students");
            //Hide Score from User
            StudentSet.EntityType.Ignore(x=>x.Score);

            return odataBuilder.GetEdmModel();
        }
    }
}