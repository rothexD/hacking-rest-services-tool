using System;
using System.Collections.Generic;
using System.Text;

namespace Hacking_Rest_SqlInjetor.FormDatas
{
    public interface IFormData
    {
        public string Method { get; set; }
        public string Action { get; set; }
        public List<InputInformation> InputFields { get; set; }
        public List<SelectInformation> SelectFields { get; set; }
    }
}
