using System;
using System.Collections.Generic;
using System.Text;
using Hacking_REST_Services.Form;
using Hacking_REST_Services.WebClient;
using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Hacking_REST_Services.Helpers
{
    public class FormDataParser
    {
        public static List<HttpContext> BuildHttpContexts(IList<FormData> data,string fromTarget)
        {
            List <HttpContext> listOfContextes = new List<HttpContext>();
            foreach(var item in data)
            {
                HttpContext singleContext = new HttpContext();
                if(item.Method == "GET")
                {
                    singleContext.Method = HttpMethod.Get;
                }
                else if(item.Method == "POST"){
                    singleContext.Method = HttpMethod.Post;
                }
                else
                {
                    continue;
                }
                try
                {
                    if(Regex.IsMatch(item.Action, @"^http://.*")){
                        singleContext.RequestUri = new Uri(item.Action);
                    }
                    else
                    {
                        string temp = Regex.Match(fromTarget, "^(http://.*)/").Groups[1].Value;
                        temp += item.Action;
                        singleContext.RequestUri = new Uri(temp);
                    }                  
                }
                catch
                {
                    continue;
                }
                foreach (var Inputs in item.InputFields)
                {
                    if(Inputs.Type != null){
                        if (Inputs.Type.ToLower() == "date")
                        {
                            singleContext.AddField(Inputs.Name, Inputs.Value ?? "22.10.2020");
                        }
                        else
                        {
                            singleContext.AddField(Inputs.Name, Inputs.Value ?? "test");
                        }
                    }                  
                    else
                    {
                        singleContext.AddField(Inputs.Name,Inputs.Value ?? "test");
                    }                  
                }
                foreach (var Selects in item.SelectFields)
                {
                    try
                    {
                        singleContext.AddField(Selects.Name, Selects.OptionValues[0]);
                    }
                    catch
                    {

                    }
                }
                listOfContextes.Add(singleContext);
            }
            return listOfContextes;
        }
        public void PrintFormDataList(IEnumerable<FormData> data)
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
                foreach (var selects in item.SelectFields)
                {
                    Console.WriteLine(selects.Id);
                    Console.WriteLine(selects.Name);
                    foreach (string options in selects.OptionValues)
                    {
                        Console.WriteLine(options);
                    }
                }
                Console.WriteLine("--------------------");
            }
        }

        public static IList<FormData> GetFormsOfHtmlDocument(HtmlDocument document, string uri)
        {
            var forms = new List<FormData>();
            var formNodes = document.DocumentNode.Descendants("form");

            foreach (var formNode in formNodes)
            {
                var form = CreateForm(formNode);
                form.Action ??= uri;
                form.InputFields = GetInputChildren(formNode);
                form.SelectFields = GetSelectChildren(formNode);

                foreach(var singleNode in GetButtonChildren(formNode))
                {
                    form.InputFields.Add(singleNode);
                }
                forms.Add(form);
            }
            return forms;
        }

        // get all Select Nodes in a specific Form of the HTML document and parse them as individual
        // to get specific key value pairs
        private static IList<SelectInformation> GetSelectChildren(HtmlNode parent)
        {
            var selectList = new List<SelectInformation>();
            foreach (var selectNode in parent.Descendants("select"))
            {
                var select = CreateSelect(selectNode);

                if (select.Name is null)
                {
                    continue;
                }

                GetSelectOptions(selectNode)
                    .ForEach(option => select.OptionValues.Add(option));

                selectList.Add(select);
            }
            return selectList;
        }

        // get all Input Nodes in a specific Form of the HTML document and parse them as individual
        // to get specific key value pairs
        private static IList<InputInformation> GetInputChildren(HtmlNode parent)
        {
            var inputList = new List<InputInformation>();
            foreach (var inputNode in parent.Descendants("input"))
            {
                var input = CreateInput(inputNode);
                if (input.Name is null)
                {
                    continue;
                }
                inputList.Add(input);
                Console.WriteLine($"{input.Type},{input.Id},{input.Name},{input.Value}\r\n");
            }
            foreach (var inputNode in parent.Descendants("textarea"))
            {
                var input = CreateInput(inputNode);

                if (input.Name is null)
                {
                    continue;
                }

                inputList.Add(input);
                Console.WriteLine($"{input.Type},{input.Id},{input.Name},{input.Value}\r\n");
            }


            return inputList;
        }

        private static IList<InputInformation> GetButtonChildren(HtmlNode parent)
        {
            var buttonList = new List<InputInformation>();
            foreach (var buttonNode in parent.Descendants("button"))
            {
                var button = CreateInput(buttonNode);

                if (button.Name is null)
                {
                    continue;
                }

                buttonList.Add(button);
                Console.WriteLine($"{button.Type},{button.Id},{button.Name},{button.Value}\r\n");
            }
            return buttonList;
        }

        private static List<string> GetSelectOptions(HtmlNode selectNode)
        {
            var options = new List<string>();
            var optionNodes = selectNode.Descendants("option");
            foreach (var option in optionNodes)
            {
                if (option.Attributes.Contains("value"))
                {
                    options.Add(option.Attributes["value"].Value);
                }
            }
            return options;
        }

        private static InputInformation CreateInput(HtmlNode node)
        {
            var attributes = node.Attributes;
            return new InputInformation
            {
                Type = attributes.Contains("type") ? attributes["type"].Value : null,
                Name = attributes.Contains("name") ? attributes["name"].Value : null,
                Id = attributes.Contains("id") ? attributes["id"].Value : null,
                Value = attributes.Contains("value") ? attributes["value"].Value : null
            };
        }

        private static SelectInformation CreateSelect(HtmlNode node)
        {
            var attributes = node.Attributes;
            return new SelectInformation
            {
                Name = attributes.Contains("name") ? attributes["name"].Value : null,
                Id = attributes.Contains("id") ? attributes["id"].Value : null
            };
        }

        private static FormData CreateForm(HtmlNode node)
        {
            var attributes = node.Attributes;
            return new FormData
            {
                Method = attributes.Contains("method") ? attributes["method"].Value : "GET",
                Action = attributes.Contains("action") ? attributes["action"].Value : null
            };
        }
    }
}
