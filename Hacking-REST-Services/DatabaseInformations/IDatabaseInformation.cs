using System.Collections.Generic;
using Hacking_REST_Services.Form;

namespace Hacking_REST_Services.DatabaseInformations
{
    public interface IDatabaseInformation
    {
        public string TargetUri { get; set; }
        public DatabaseInformationEnumeration DatabaseArchitecture { get; set; }
        public List<FormData> FormDataList { get; set; }
    }
}
