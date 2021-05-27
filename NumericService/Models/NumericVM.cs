using System;
using System.Collections.Generic;
using System.Text;

namespace NumericService.Models
{
   public class NumericVM
    {
        public string FileName = "Numeric.txt";

        public List<string> Data;

        public NumericVM()
        {
            this.Data = new List<string>();
        }
    }
}
