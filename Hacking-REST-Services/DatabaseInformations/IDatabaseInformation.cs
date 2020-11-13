using System.Collections.Generic;
using Hacking_Rest_SqlInjetor.Form;

namespace Hacking_Rest_SqlInjetor.DatabaseInformations
{
    public interface IDatabaseInformation
    {
        public string TargetUri { get; set; }
        public DatabaseInformationEnumeration DatabaseArchitecture { get; set; }
        public List<FormData> FormDataList { get; set; }
    }
}
