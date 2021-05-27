using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StoreService.Lib;
using StoreService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StoreService
{
   static class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {

                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("demo-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            var path = "C:\\Users\\ilkin\\source\\repos\\SimbrellaEx1\\StoreService\\DataFiles\\Data.txt";


            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();

                DataVM vm = new DataVM();

               vm = JsonConvert.DeserializeObject< DataVM>(Encoding.UTF8.GetString(body));

                Console.WriteLine(vm);

                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    var data = new List<string>();

                    foreach (var text in vm.Data)
                    {
                        sw.WriteLine(text);


                    }  
                    sw.Close();
                }


                FtpClient ftpClient = new FtpClient("ftp://172.18.210.6/Store", "ilkin145", "ilkin145");


                ftpClient.upload(Guid.NewGuid().ToString() + vm.FileName, path); 
            };



            channel.BasicConsume("demo-queue", true, consumer);
            Console.ReadLine();




        }
    }
}
