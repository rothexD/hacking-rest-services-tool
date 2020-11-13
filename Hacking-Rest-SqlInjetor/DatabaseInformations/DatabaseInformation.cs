using System;
using System.Collections.Generic;
using System.Text;
using Hacking_Rest_SqlInjetor.FormDatas;

namespace Hacking_Rest_SqlInjetor.DatabaseInformations
{
    class DatabaseInformation : IDatabaseInformation
    {
        public string TargetUri { get; set; }
        public DatabseInforamtionEnumeration DatabaseArchitecture { get; set; }
        public bool[] QueryParameters { get; set; }
        public List<FormData> FormDataList{get;set;}
        public DatabaseInformation()
        {
            DatabaseArchitecture = DatabseInforamtionEnumeration.NOTTESTED;
            QueryParameters = null;
            FormDataList = new List<FormData>();
        }
    }
}
