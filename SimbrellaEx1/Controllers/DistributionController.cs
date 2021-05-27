using DistributionService.Entities;
using DistributionService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DistributionController : ControllerBase
    {
        private readonly AppDbContext db;

        public DistributionController(AppDbContext _db)
        {
            db = _db;
        }
        [HttpGet]

        public string CreateData()
        {
            return "Create a data like: Numeric, String or Date";
        }

        [HttpPost]

        public IActionResult CreateData(Data data)
        {

            if (data != null)
            {
                int number;
                DateTime dateTime;
                bool isParsable = Int32.TryParse(data.Value, out number);
                if (isParsable)
                {
                    Numeric numeric = new Numeric()
                    {
                        NumericData = data.Value
                    };
                    db.Numerics.Add(numeric);
                    db.SaveChanges();
                    return Ok();
                }
                else if (DateTime.TryParse(data.Value, out dateTime))
                {
                    Date date = new Date()
                    {
                        DateData = data.Value
                    };
                    db.Dates.Add(date);
                    db.SaveChanges();
                }
                else
                {
                    Text text = new Text()
                    {
                        TextData = data.Value
                    };

                    db.Texts.Add(text);
                    db.SaveChanges();
                }
            }
            return Ok();

        }
    }
}
