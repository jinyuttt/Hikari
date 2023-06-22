using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp3
{
    public class PrintTimeService: IHostedService
    {
        public string _Topic { get; set; }
        public string _ConsumerGroup { get; set; }

       
        public async Task StartAsync(CancellationToken cancellationToken)
        {
           
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 处理消息的方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual bool Process(string message)
        {
            throw new NotImplementedException();
        }
    }
}
