using EventLink.API.Schema;
using EventLink.API.Schema.Types;
using EventLink.API.Schema.Types.LogTypes;
using EventLink.API.Schema.Types.UserTypes;
using EventLink.API.Services;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventService = EventLink.DataAccess.Services.EventService;
using LogService = EventLink.DataAccess.Services.LogService;
using PaymentService = EventLink.DataAccess.Services.PaymentService;
using UserService = EventLink.DataAccess.Services.UserService;

namespace EventLink.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            /* Add DataAccess services */
            services.AddScoped<EventService>();
            services.AddScoped<LogService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<UserService>();

            /* Add API backend services */
            services.AddSingleton<IEventService, Services.EventService>();
            services.AddSingleton<ILogService, Services.LogService>();
            services.AddSingleton<IPaymentService, Services.PaymentService>();
            services.AddSingleton<IUserService, Services.UserService>();

            /* Add GraphQL Types */
            services.AddTransient<EventType>();
            services.AddTransient<LogType>();
            services.AddTransient<PaymentType>();
            services.AddTransient<UserType>();

            services.AddTransient<LogCreateInputType>();
            services.AddTransient<LogCreateInput>();

            services.AddTransient<UserCreateInputType>();
            services.AddTransient<UserCreateInput>();

            services.AddTransient<UserUpdateInputType>();
            services.AddTransient<UserUpdateInput>();

            /* Queries */
            services.AddTransient<EventLinkQuery>();

            /* Mutations */
            services.AddTransient<EventLinkMutation>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseGraphiQLServer(new GraphiQLOptions
            {
                GraphiQLPath = "/ui/graphiql",
                GraphQLEndPoint = "/graphql"
            });

            app.UseGraphQLVoyager(new GraphQLVoyagerOptions()
            {
                GraphQLEndPoint = "/graphql",
                Path = "/ui/voyager"
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}