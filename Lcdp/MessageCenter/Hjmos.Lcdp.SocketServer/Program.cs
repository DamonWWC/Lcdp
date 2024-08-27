using System;

namespace Hjmos.Lcdp.SocketServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // 启动服务
                SuperSocketService.Init();
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
