using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Hacking_Rest_SqlInjetor.DatabaseInformations;
using System.Text.RegularExpressions;
using Hacking_Rest_SqlInjetor.FormDatas;
using Hacking_Rest_SqlInjetor.WebClient;

namespace Hacking_Rest_SqlInjetor.ServiceHandlers
{
    public abstract class AbstractServiceHandler 
    {
        public AbstractServiceHandler(){

        }
        abstract public void StartAttack(string targetUri,ICustomHttpClient Client);

        public List<FormData> GetFormDataIntoDatabaseInformation(HtmlAgilityPack.HtmlDocument Document,string uri)
        {
            //Console.WriteLine(Document.Text);
            
            List<FormData> listOfFormData = new List<FormData>();            
            var form = Document.DocumentNode.Descendants("form");
            
            foreach(var dataFormField in form)
            {
                FormData formData = new FormData();
                try
                {
                    formData.Method = dataFormField.Attributes["method"].Value;
                }
                catch
                {
                    formData.Method = "GET";
                }
                try
                {
                    formData.Action = dataFormField.Attributes["action"].Value;
                }
                catch
                {
                    formData.Action = uri;
                }

                // get all Input Nodes in a specfic Form of the htmldocument and parse them as indiviual to get specific key value pairs
                foreach (var dataInputFiled in dataFormField.Descendants("input"))
                {
                    InputInformation singleInput = new InputInformation();
                    try
                    {
                        singleInput.Type = dataInputFiled.Attributes["type"].Value;
                    }
                    catch
                    {
                        singleInput.Type = null;
                    }
                    try
                    {
                        singleInput.Name = dataInputFiled.Attributes["name"].Value;
                    }
                    catch
                    {
                        continue;
                    }
                    try
                    {
                        singleInput.Id = dataInputFiled.Attributes["id"].Value;
                    }
                    catch
                    {
                        singleInput.Id = null;
                    }
                    
                    try
                    {
                        singleInput.Value = dataInputFiled.Attributes["value"].Value;
                    }
                    catch
                    {
                        singleInput.Value = null;
                    }
                    Console.WriteLine($"{singleInput.Type},{singleInput.Id},{singleInput.Name},{singleInput.Value}\r\n");

                    formData.InputFields.Add(singleInput);
                                     
                }
                // get all Select Nodes in a specfic Form of the htmldocument and parse them as indiviual to get specific key value pairs
                foreach (var dataSelectField in dataFormField.Descendants("select"))
                {
                    SelectInformation singleSelect = new SelectInformation();
                    try
                    {
                        singleSelect.Name = dataSelectField.Attributes["name"].Value;
                    }
                    catch
                    {
                        continue;
                    }
                    try
                    {
                        singleSelect.Id = dataSelectField.Attributes["id"].Value;
                    }
                    catch
                    {
                        singleSelect.Id = null;
                    }
                    foreach(var dataOptionField in dataSelectField.Descendants("option"))
                    {
                        try
                        {
                            singleSelect.OptionValues.Add(dataSelectField.Attributes["value"].Value);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    formData.SelectFields.Add(singleSelect);
                }
                foreach(var dataButtonField in dataFormField.Descendants("button"))
                {

                }
                listOfFormData.Add(formData);
            }
            return listOfFormData;
        }
        public void printFormDataList(List<FormData> data)
        {
            foreach (var item in data)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine(item.Action);
                Console.WriteLine(item.Method);
                foreach (var input in item.InputFields)
                {
                    Console.WriteLine($"(Input) ID:{input.Id} Name:{input.Name} Value:{input.Value} Type:{input.Type}");
                }
                foreach (var Selects in item.SelectFields)
                {
                    Console.WriteLine(Selects.Id);
                    Console.WriteLine(Selects.Name);
                    foreach (var Options in Selects.OptionValues)
                    {
                        Console.WriteLine(Options);
                    }
                }
                Console.WriteLine("--------------------");
            }
        }
    }
}
