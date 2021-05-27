using System;
using System.Collections.Generic;
using System.Text;

namespace TextService.Models
{
  public  class TextVM
    {
        public string FileName = "Text.txt";

        public List<string> Data;

        public TextVM()
        {
            this.Data = new List<string>();
        }
    }
}
