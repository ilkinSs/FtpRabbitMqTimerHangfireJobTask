using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TextService.Models;
using TextService.Utilities;

namespace TextService
{
    class Program
    {          
        private static System.Timers.Timer aTimer;
      

        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("TextService is started");


        

            aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 25000;
            aTimer.Enabled = true;
            Console.ReadLine();   
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("TextService is checking  ftp folder...");

           string userName = ConfigHelper.GetConfigVal("User", "UserName");
           string passWord = ConfigHelper.GetConfigVal("User", "Password");
           string ip = ConfigHelper.GetConfigVal("FtpIp", "Text");
            // Get the object used to communicate with the server.
            FtpWebRequest fullRequest = (FtpWebRequest)WebRequest.Create(ip);
            fullRequest.Method = WebRequestMethods.Ftp.ListDirectory;

            // This example assumes the FTP site uses anonymous logon.
            fullRequest.Credentials = new NetworkCredential(userName, passWord);
            var fyleNames = new List<string>();

            try
            {
                FtpWebResponse fullResponse = (FtpWebResponse)fullRequest.GetResponse();
                Stream fullResponseStream = fullResponse.GetResponseStream();
                StreamReader fullReader = new StreamReader(fullResponseStream);


                while (!fullReader.EndOfStream)
                {
                    fyleNames.Add(fullReader.ReadLine());              
                }
                fullReader.Close();
                fullResponse.Close();          
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            foreach (var name in fyleNames)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ip}{name}");
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(userName, passWord);
                try
                {
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    Stream responseStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(responseStream);

                    TextVM vm = new TextVM();

                    while (reader.Peek() >= 0)
                    {
                        vm.Data.Add(reader.ReadLine());
                    }
                    vm.Data.Sort();
                    reader.Close();
                    response.Close();

                    FtpWebRequest requestForDeleting = (FtpWebRequest)WebRequest.Create($"{ip}{name}");
                    requestForDeleting.Credentials = new NetworkCredential(userName,  passWord);
                    requestForDeleting.UseBinary = true;
                    requestForDeleting.UsePassive = true;
                    requestForDeleting.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    requestForDeleting.Method = WebRequestMethods.Ftp.DeleteFile;
                    /* Establish Return Communication with the FTP Server */
                    var ftpResponse = (FtpWebResponse)requestForDeleting.GetResponse();

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

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(vm));

                    channel.BasicPublish("", "demo-queue", null, body);
                    logger.LogInformation("TextService is sending data to RabbitMQ...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }
    }
}
