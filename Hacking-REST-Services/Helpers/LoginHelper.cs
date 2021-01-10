using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.RegularExpressions;
using Hacking_REST_Services.Form;
using Hacking_REST_Services.WebClient;
using System.Net;

namespace Hacking_REST_Services.Helpers
{
    public class LoginHelper : FormDataParser
    {
        public LoginHelper()
        {
        }
        public bool tryLogin(ICustomHttpClient Client)
        {
            Console.WriteLine("Enter M for Manual Form generation or A for automaic or E for exit");
            string temp ="";
            while(temp != "A" && temp != "M"){
                temp = Console.ReadLine();
                if(temp == "E")
                {
                    return false;
                }
            }
            if(temp == "A")
            {
                HttpContext context = AutomatedFormLogin(Client);
                if(context == null)
                {
                    return false;
                }
                context.BuildRequest();
                var result = Client.Post(context).Result;
                
                Console.WriteLine($"Got Statuscode: {result.StatusCode} and headers:");
                foreach (var header in result.Headers)
                {
                    foreach (var headerspecifc in header.Value)
                    {
                        Console.WriteLine($"{header.Key}:{headerspecifc}");
                    }
                }
                return true;
            }
            else
            {
                AbstractHttpContext context = ManualFormLogin();
                if (context == null)
                {
                    return false;
                }
                context.BuildRequest();
                var result = Client.ByContextMethod(context).Result;
                Console.WriteLine($"Got Statuscode: {result.StatusCode} and headers:");
                foreach ( var header in result.Headers)
                {
                    foreach(var headerspecifc in header.Value)
                    {
                        Console.WriteLine($"{header.Key}:{headerspecifc}");
                    }               
                }
                return true;
            }
        }
        private HttpContext ManualFormLogin()
        {
            string targetUri;
            string method;
            HttpContext context = null;

            Console.WriteLine($"Enter target Uri:");
            targetUri = Console.ReadLine();
            Console.WriteLine($"Enter Form Method:");
            method = Console.ReadLine();

            if(method == "GET")
            {
                context = new HttpContext(HttpMethod.Get, targetUri);
            }else if(method == "POST")
            {
                context = new HttpContext(HttpMethod.Post, targetUri);
            }
            else
            {
                return null;
            }

            Console.WriteLine($"Enter {method} parameters matching form key:value type fin() to end");
            string temp;
            while((temp = Console.ReadLine())!= "fin()")
            {
                string[] split = temp.Split(":", 2);
                if(split.Length == 1)
                {
                    Console.WriteLine($"{temp} --- was not added doest have form input:value, to end type fin()");
                    continue;
                }
                Console.WriteLine($"Added {split[0].Trim()} as key and {split[1].Trim()} as value");
                context.AddField(split[0].Trim(), split[1].Trim());
            }
            return context;
        }
        private HttpContext AutomatedFormLogin(ICustomHttpClient Client)
        {
            string targetUri;
            Console.WriteLine($"Enter target Uri:");
            targetUri = Console.ReadLine();
            var openSite = Client.Get(targetUri);
            var document = CustomHttpClient.GetHtmlDocument(openSite);
            HttpContext context;
            var formsOfHtmlDocument = GetFormsOfHtmlDocument(document, targetUri);
            
            int FormID = -1;
            if (formsOfHtmlDocument.Count > 1)
            {
                Console.WriteLine($"Found Count: {formsOfHtmlDocument.Count} forms please chose a specific one,type 0 to end");              
                do
                {
                    int i = 1;
                    foreach (var specific in formsOfHtmlDocument)
                    {
                        Console.WriteLine($"Form: {i} has method: {specific.Method} and action: {specific.Action}");
                        i++;
                    }
                    Console.WriteLine("Chose an integer FormID");

                    try
                    {
                        FormID = Convert.ToInt32(Console.ReadLine());
                        if(FormID == 0)
                        {
                            return null;
                        }
                    }
                    catch
                    {
                        FormID = -1;
                    }
                } while (!(FormID > 0 && FormID <= formsOfHtmlDocument.Count));
                if(formsOfHtmlDocument[FormID-1].Method == "GET")
                {
                    Console.WriteLine("Form is type GET");
                    context = new HttpContext(HttpMethod.Get, targetUri.Trim());
                }
                else if (formsOfHtmlDocument[FormID - 1].Method == "POST")
                {
                    Console.WriteLine("Form is type POST");
                    context = new HttpContext(HttpMethod.Post, targetUri.Trim());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                FormID = 1; 
                if (formsOfHtmlDocument[0].Method == "GET")
                {
                    Console.WriteLine("Form is type GET");
                    context = new HttpContext(HttpMethod.Get, targetUri.Trim());
                }
                else if(formsOfHtmlDocument[0].Method == "POST")
                {
                    Console.WriteLine("Form is type POST");
                    context = new HttpContext(HttpMethod.Post, targetUri.Trim());
                }
                else
                {
                    return null;
                }
            }
            Console.WriteLine("INPUTFIELD");
            Console.WriteLine("InputField-Name can occur multipletimes incase of radiobuttons, Inputfields of same name overwrite");
            foreach (InputInformation inputField in formsOfHtmlDocument[FormID - 1].InputFields)
            {
                Console.WriteLine($"Found InputField:\"{inputField.Name}\" with id:\"{inputField.Id}\" and predefined value:\"{inputField.Value}\" and type:\"{inputField.Type}\"");
                Console.Write($"Enter Value that should be passed, enter spacebar for empty value: ");
                Console.Out.Flush();
                string temp = Console.ReadLine();
                if (temp == " ")
                {
                    context.AddField(inputField.Name, "");
                    continue;
                }
                context.AddField(inputField.Name, temp);
            }
            Console.WriteLine($"SelectFields, Count({formsOfHtmlDocument[FormID - 1].SelectFields.Count})");
            foreach (SelectInformation selectField in formsOfHtmlDocument[FormID - 1].SelectFields)
            {
                Console.WriteLine($"{selectField.Name} has id: {selectField.Id} and has an optioncount of ({selectField.OptionValues.Count})");
                Console.WriteLine("Found possible predefined values of:");
                foreach (string singleOptionFieldValue in selectField.OptionValues)
                {
                    Console.WriteLine(singleOptionFieldValue);
                }
                Console.Write($"Input a value for SelectField({selectField.Name}): ");
                Console.Out.Flush();
                string value = Console.ReadLine();
                context.AddField(selectField.Name, value);
            }
            return context;
        }
    }
}
