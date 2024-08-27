using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;

namespace Hjmos.Lcdp.SocketServer
{
    public class SuperSocketService
    {
        public static void Init()
        {
            IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();

            // 读取配置文件
            if (!bootstrap.Initialize())
            {
                Console.WriteLine("初始化服务失败");
                Console.Read();
                return;
            }
            Console.WriteLine("开始启动服务");
            // 启动服务
            _ = bootstrap.Start();
            foreach (IWorkItem server in bootstrap.AppServers)
            {
                if (server.State == ServerState.Running)
                {
                    Console.WriteLine($"服务【{server.Name}】启动成功，端口：{server.Config.Port}");
                }
                else
                {
                    Console.WriteLine($"服务【{server.Name}】启动失败");
                }
            }
        }
    }
}
