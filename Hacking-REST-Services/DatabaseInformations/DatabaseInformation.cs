using System.Collections.Generic;
using Hacking_REST_Services.Form;

namespace Hacking_REST_Services.DatabaseInformations
{
    class DatabaseInformation : IDatabaseInformation
    {
        public string TargetUri { get; set; }
        public DatabaseInformationEnumeration DatabaseArchitecture { get; set; }
        public bool[] QueryParameters { get; set; }
        public List<FormData> FormDataList{get;set;}
        
        public DatabaseInformation()
        {
            DatabaseArchitecture = DatabaseInformationEnumeration.NotTested;
            QueryParameters = null;
            FormDataList = new List<FormData>();
        }
    }
}
