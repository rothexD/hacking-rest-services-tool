﻿using System.Collections.Generic;

namespace Hacking_Rest_SqlInjetor.Form
{
    public interface IFormData
    {
        public string Method { get; set; }
        public string Action { get; set; }
        public IEnumerable<InputInformation> InputFields { get; set; }
        public IEnumerable<SelectInformation> SelectFields { get; set; }
    }
}