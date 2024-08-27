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
                        Description = "������ǰ�˵ʹ���ƽ̨��WebAPI��Ŀ"
                    });
                });

                #region ΪSwagger����XML�ĵ�ע��·��

                // ��ȡӦ�ó�������Ŀ¼
                string basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                // ƴ������XML�ĵ�ע��·��
                string xmlPath = Path.Combine(basePath, "Hjmos.Lcdp.VisualEditorServer.WebAPI.xml");
                // ��Swagger���������XML�ĵ�ע��·�����ڶ���������ʾ����Controller��ע��
                c.IncludeXmlComments(xmlPath,true);

                #endregion
            });

            // TODO���ĳ��������䣬��Ŀȡ������
            services.AddTransient<ICommon.IConfiguration, Common.Configuration>();
            services.AddTransient<ICommon.IConnectionFactory, Common.ConnectionFactory>();
            services.AddTransient<ICommon.IUtils, Common.Utils>();
            services.AddTransient<IService.ILoginService, Service.LoginService>();
            services.AddTransient<IService.IMenuService, Service.MenuService>();
            services.AddTransient<IService.IPageService, Service.PageService>();

            #region ���е�API֧�ֿ���

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

            #region ֧�ֿ���

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
