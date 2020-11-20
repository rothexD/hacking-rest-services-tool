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
using Microsoft.OData.Edm;

namespace ODataWebApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(mvcOptions => 
                mvcOptions.EnableEndpointRouting = false);

            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            //Settings for Hiding odata /metadata
            var defaultConventions = ODataRoutingConventions.CreateDefault();
            var conventions = defaultConventions.Except(
                defaultConventions.OfType<MetadataRoutingConvention>());
            
            
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