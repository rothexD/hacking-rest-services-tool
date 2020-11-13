using System;
using System.Collections.Generic;
using System.Text;
using Hacking_Rest_SqlInjetor.FormDatas;

namespace Hacking_Rest_SqlInjetor.DatabaseInformations
{
    public interface IDatabaseInformation
    {
        public string TargetUri { get; set; }
        public DatabseInforamtionEnumeration DatabaseArchitecture { get; set; }
        public List<FormData> FormDataList { get; set; }
    }
}
