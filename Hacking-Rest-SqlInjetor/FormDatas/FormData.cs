using System;
using System.Collections.Generic;
using System.Text;

namespace Hacking_Rest_SqlInjetor.FormDatas
{
    public class FormData : IFormData
    {
        public string Method { get; set; }
        public string Action { get; set; }
        public List<string> Inputs { get; set; }
        public Dictionary<string, List<string>> SelectValues;
        public Dictionary<string, string> ButtonValues;
        public FormData()
        {
            Inputs = new List<string>();
        }
    }
}
