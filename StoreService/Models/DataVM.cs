using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService.Models
{
   public class DataVM
    {
       
            public string FileName = "";

            public List<string> Data;

            public DataVM()
            {
                this.Data = new List<string>();
            }
        
    }
}
