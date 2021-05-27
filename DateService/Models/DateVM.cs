using System;
using System.Collections.Generic;
using System.Text;

namespace DateService.Models
{
   public class DateVM
    {
        public string FileName = "Date.txt";

        public List<string> Data;

        public DateVM()
        {
            this.Data = new List<string>();
        }
    }
}
