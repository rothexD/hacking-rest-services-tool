using System;
using System.Collections.Generic;
using System.Text;

namespace Hacking_Rest_SqlInjetor.FormDatas
{
    public class FormData : IFormData
    {
        public string Method { get; set; }
        public string Action { get; set; }
        public List<InputInformation> InputFields { get; set; }
        public List<SelectInformation> SelectFields { get; set; }

        //radio buttons currenlty not in use, Request overwrites multiple same key value pair.
        private List<RadioButtonInformation> RadioButtons { get; set; }
        public FormData()
        {
            InputFields = new List<InputInformation>();
            SelectFields = new List<SelectInformation>();
        }
    }
    public class InputInformation
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public InputInformation()
        {
            Type = null;
            Id = null;
            Name = null;
            Value = null;
        }
    }
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
    public class RadioButtonInformation
    {
        //currently not in use
        public string Name { get; set; }
        public string Type { get; set; }

        public RadioButtonInformation()
        {
            Type = "radio";
        }

    }
}
