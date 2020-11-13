namespace Hacking_Rest_SqlInjetor.Form
{
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
}