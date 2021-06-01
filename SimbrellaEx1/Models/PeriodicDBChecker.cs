using DistributionService.Entities;
using DistributionService.Lib;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimbrellaEx1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace DistributionService.Models
{

  

    public class PeriodicDBChecker : IPeriodicDBChecker
    {
        private readonly object balanceLock = new object();

        private readonly AppDbContext db;    
        public PeriodicDBChecker(AppDbContext _db)
        {
            db = _db;
        }

    public  void Check()
        {


            
              
                try
                {
                    var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                    ILogger logger = loggerFactory.CreateLogger<Program>();
                    logger.LogInformation("DBChecker is start to check db ...");

                    var userName = ConfigHelper.GetConfigVal("User", "UserName");
                    var password = ConfigHelper.GetConfigVal("User", "Password");

                    List<Text> texts = db.Texts.ToList();
                    List<Numeric> numerics = db.Numerics.ToList();
                    List<Date> dates = db.Dates.ToList();

                    if (texts != null)
                    {
                        if (texts.Count > 0)
                        {

                            string[] dataTable = new string[texts.Count];
                            int counter = 0;
                            foreach (var text in texts)
                            {
                                dataTable[counter] = text.TextData;
                                counter++;
                            }
                            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    db.Texts.RemoveRange(texts);
                                    db.SaveChanges();

                                    string textFtpIp = "ftp://172.18.210.6/Text";
                                    FtpClient ftpClient = new FtpClient(textFtpIp, userName, password);
                                    ftpClient.upload(textFtpIp, Guid.NewGuid().ToString() + "TextData.txt", dataTable);

                                    transaction.Commit();
                                    logger.LogInformation("Text table is cleared ...");
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    logger.LogInformation("Text table is rollbacked ...");
                                }
                            }


                        }
                    }

                    if (numerics != null)
                    {
                        if (numerics.Count > 0)
                        {
                            string[] dataTable = new string[numerics.Count];
                            int counter = 0;
                            foreach (var numeric in numerics)
                            {
                                dataTable[counter] = numeric.NumericData;
                                counter++;
                            }

                            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    db.Numerics.RemoveRange(numerics);
                                    db.SaveChanges();

                                    string numericFtpIp = "ftp://172.18.210.6/Numeric";
                                    FtpClient ftpClient = new FtpClient(numericFtpIp, userName, password);
                                    ftpClient.upload(numericFtpIp, Guid.NewGuid().ToString() + "NumericData.txt", dataTable);

                                    transaction.Commit();
                                    logger.LogInformation("Numeric table is cleared ...");

                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    logger.LogInformation("Numeric table is rollbacked ...");
                                }
                            }

                        }

                    }

                    if (dates != null)
                    {
                        if (dates.Count > 0)
                        {
                            string[] dataTable = new string[dates.Count];
                            int counter = 0;
                            foreach (var date in dates)
                            {
                                dataTable[counter] = date.DateData;
                                counter++;
                            }
                            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    db.Dates.RemoveRange(dates);
                                    db.SaveChanges();

                                    string dateFtpIp = "ftp://172.18.210.6/Date";
                                    FtpClient ftpClient = new FtpClient(dateFtpIp, userName, password);
                                    ftpClient.upload(dateFtpIp, Guid.NewGuid().ToString() + "DateData.txt", dataTable);

                                    transaction.Commit();
                                    logger.LogInformation("Date table is cleared ...");
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    logger.LogInformation("Numeric table is rollbacked ...");
                                }
                            }


                        }
                    }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    db.Dispose();
                }

            


        }

     
    }
}
