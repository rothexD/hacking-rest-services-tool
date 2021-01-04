using System.Collections.Generic;

namespace Hacking_REST_Services.Form
{
    public class SelectInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        
        public List<string> OptionValues { get; set; }
        public SelectInformation()
        {
            Id = null;
            Name = null;
            OptionValues = new List<string>();
        }
    }
}