using Hjmos.Lcdp.VisualEditorServer.WebAPI.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI
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
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(v =>
                {
                    c.SwaggerDoc(v, new OpenApiInfo()
                    {
                        Title = "Lcdp.WebAPI",
                        Version = v,
                        Description = "服务于前端低代码平台的WebAPI项目"
                    });
                });

                #region 为Swagger配置XML文档注释路径

                // 获取应用程序所在目录
                string basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                // 拼接完整XML文档注释路径
                string xmlPath = Path.Combine(basePath, "Hjmos.Lcdp.VisualEditorServer.WebAPI.xml");
                // 在Swagger配置中添加XML文档注释路径，第二个参数表示包含Controller的注释
                c.IncludeXmlComments(xmlPath,true);

                #endregion
            });

            // TODO：改成批量反射，项目取消引用
            services.AddTransient<ICommon.IConfiguration, Common.Configuration>();
            services.AddTransient<ICommon.IConnectionFactory, Common.ConnectionFactory>();
            services.AddTransient<ICommon.IUtils, Common.Utils>();
            services.AddTransient<IService.ILoginService, Service.LoginService>();
            services.AddTransient<IService.IMenuService, Service.MenuService>();
            services.AddTransient<IService.IPageService, Service.PageService>();

            #region 所有的API支持跨域

            //services.AddCors(option => option.AddPolicy("AllowCors", _build => _build.AllowAnyOrigin().AllowAnyMethod()));

            #endregion

            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    typeof(ApiVersions).GetEnumNames().ToList().ForEach(v =>
                    {
                        c.SwaggerEndpoint($"/swagger/{v}/swagger.json", $"Lcdp.WebAPI {v}");
                    });
                });
            }

            app.UseRouting();

            #region 支持跨域

            //app.UseCors("AllowCors");

            #endregion

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
