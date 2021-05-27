using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TextService.Models;

namespace TextService
{
    class Program
    {
        private static System.Timers.Timer aTimer;

        static void Main(string[] args)
        {
            aTimer = new System.Timers.Timer(10000);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Set the Interval to 2 seconds (2000 milliseconds).
            aTimer.Interval = 25000;
            aTimer.Enabled = true;
            Console.ReadLine();   
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Get the object used to communicate with the server.

            FtpWebRequest fullRequest = (FtpWebRequest)WebRequest.Create("ftp://172.18.210.6/Text/");
            fullRequest.Method = WebRequestMethods.Ftp.ListDirectory;

            // This example assumes the FTP site uses anonymous logon.
            fullRequest.Credentials = new NetworkCredential("ilkin145", "ilkin145");
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
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://172.18.210.6/Text/{name}");
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("ilkin145", "ilkin145");

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

                    FtpWebRequest requestForDeleting = (FtpWebRequest)WebRequest.Create($"ftp://172.18.210.6/Text/{name}");
                    requestForDeleting.Credentials = new NetworkCredential("ilkin145", "ilkin145");
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
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }


            }

        }
    }
}
