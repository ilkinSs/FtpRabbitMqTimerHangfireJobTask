using DistributionService.Entities;
using DistributionService.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        

        private readonly AppDbContext db;

        public PeriodicDBChecker(AppDbContext _db)
        {
            db = _db;
        }
        public  void Check()
        {
            var pathTxt = "C:\\Users\\ilkin\\source\\repos\\SimbrellaEx1\\SimbrellaEx1\\DataFiles\\Texts.txt";
            var pathNum = "C:\\Users\\ilkin\\source\\repos\\SimbrellaEx1\\SimbrellaEx1\\DataFiles\\Numerics.txt";
            var pathDate = "C:\\Users\\ilkin\\source\\repos\\SimbrellaEx1\\SimbrellaEx1\\DataFiles\\Dates.txt";

            if (db.Texts.Count() > 0)
            {
                List<Text> texts = db.Texts.ToList();

                using (StreamWriter sw = System.IO.File.CreateText(pathTxt))
                {
                    foreach (var text in texts)
                    {
                        sw.WriteLine(text.TextData);
                    }
                    sw.Close();
                }

                //db.Texts.FromSqlRaw("DELETE * Texts  OUTPUT DELETED.* into Numerics where Id > 1; ");


                db.Texts.RemoveRange(texts);
                db.SaveChanges();

                string textFtpIp = "ftp://172.18.210.6/Text";
                FtpClient ftpClient = new FtpClient(textFtpIp, "ilkin145", "ilkin145");
                ftpClient.upload(textFtpIp, Guid.NewGuid().ToString() + "TextData.txt", pathTxt);

                

            }

            if (db.Numerics.Count() > 0)
            {
                List<Numeric> numerics = db.Numerics.ToList();

                using (StreamWriter sw = System.IO.File.CreateText(pathNum))
                {
                    foreach (var num in numerics)
                    {
                        sw.WriteLine(num.NumericData);
                    }
                    sw.Close();
                }
                db.Numerics.RemoveRange(numerics);
                db.SaveChanges();

                string numericFtpIp = "ftp://172.18.210.6/Numeric";

                FtpClient ftpClient = new FtpClient(numericFtpIp, "ilkin145", "ilkin145");
                ftpClient.upload(numericFtpIp, Guid.NewGuid().ToString() + "NumericData.txt", pathNum);
            }

            if (db.Dates.Count() > 0)
            { 
                List<Date> dates = db.Dates.ToList();
                using (StreamWriter sw = System.IO.File.CreateText(pathDate))
                {
                    foreach (var text in dates)
                    {
                        sw.WriteLine(text.DateData);
                    }
                    sw.Close();
                }



                db.Dates.RemoveRange(dates);
                db.SaveChanges();

                string dateFtpIp = "ftp://172.18.210.6/Date";
                FtpClient ftpClient = new FtpClient(dateFtpIp, "ilkin145", "ilkin145");
                ftpClient.upload(dateFtpIp, Guid.NewGuid().ToString() +  "DateData.txt", pathDate);
            }

        }
    }
}
