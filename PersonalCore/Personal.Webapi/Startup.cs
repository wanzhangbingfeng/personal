using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Personal.Webapi.Entity;

namespace Personal.Webapi
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
            //跨域处理
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    );
            });

            //添加数据自定义数据转换
            services.AddControllers()
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.Converters.Add(new GuidConvert());
                    option.JsonSerializerOptions.Converters.Add(new datetimeConvert());
                    option.JsonSerializerOptions.Converters.Add(new decimalConvert());
                });

            //数据库连接字符串
            services.AddDbContext<PersonalContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("personal"));
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            //身份验证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(10),

                    ValidateIssuer = true,//是否验证issuer
                    ValidIssuer = Configuration["issuer"],

                    ValidateAudience = true,
                    ValidAudience = Configuration["audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))

                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenSignatureKeyNotFoundException))
                        {
                            context.Response.Headers.Add("act", "expried");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddHttpContextAccessor();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
