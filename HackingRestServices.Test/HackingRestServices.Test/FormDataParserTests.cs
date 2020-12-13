using Hacking_REST_Services.Helpers;
using HtmlAgilityPack;
using NUnit.Framework;

namespace HackingRestServices.Test
{
    public class Tests
    {
        private const string HtmlBeeWappLogin = @"
<div id=""main"">

    <h1>Login</h1>

    <p>Enter your credentials <i>(bee/bug)</i>.</p>

    <form action=""/login.php"" method=""POST"" data-bitwarden-watching=""1"">

        <p><label for=""login"">Login:</label><br>
        <input type=""text"" id=""login"" name=""login"" size=""20"" autocomplete=""off""></p> 

        <p><label for=""password"">Password:</label><br>
        <input type=""password"" id=""password"" name=""password"" size=""20"" autocomplete=""off""></p>

        <p><label for=""security_level"">Set the security level:</label><br>

        <select name=""security_level"">

            <option value=""0"">low</option>
            <option value=""1"">medium</option>
            <option value=""2"">high</option>

        </select>

        </p>

        <button type=""submit"" name=""form"" value=""submit"">Login</button>

    </form>

    <br>
    
</div>";

        private const string StackOverflowLoginHtml = @"
<form id=""login-form"" class=""grid fd-column gs12 gsy"" action=""/users/login"" method=""POST"" data-bitwarden-watching=""1"">
        <input type=""hidden"" name=""fkey"" value=""ff393d5bacc349f28279eb3f7a805240c8a0c16779b30d398ac56999ccf1474d"">
            <input type=""hidden"" name=""ssrc"" value=""head"">
                <div class=""grid fd-column gs4 gsy js-auth-item "">
                    <label class=""grid--cell s-label"" for=""email"">Email</label>
                    <div class=""grid ps-relative"">
                        <input class=""s-input"" id=""email"" type=""email"" size=""30"" maxlength=""100"" name=""email"">
                        <svg aria-hidden=""true"" class=""s-input-icon js-alert-icon d-none svg-icon iconAlertCircle"" width=""18"" height=""18"" viewBox=""0 0 18 18""><path d=""M9 17A8 8 0 119 1a8 8 0 010 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z""></path></svg>
                    </div>
                            <p class=""grid--cell s-input-message js-error-message d-none"">
            
        </p>

                </div>
                <div class=""grid fd-column-reverse gs4 gsy js-auth-item "">
                            <p class=""grid--cell s-input-message js-error-message d-none"">
            
        </p>

                    <div class=""grid ps-relative js-password"">
                        <input class=""grid--cell s-input"" type=""password"" autocomplete=""off"" name=""password"" id=""password"">
                        <svg aria-hidden=""true"" class=""s-input-icon js-alert-icon d-none svg-icon iconAlertCircle"" width=""18"" height=""18"" viewBox=""0 0 18 18""><path d=""M9 17A8 8 0 119 1a8 8 0 010 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z""></path></svg>
                    </div>
                    <div class=""grid ai-center ps-relative jc-space-between"">
                        <label class=""grid--cell s-label"" for=""password"">Password</label>

                            <a class=""grid--cell s-link fs-caption"" href=""/users/account-recovery"">Forgot password?</a>
                    </div>
                </div>
            <div class=""grid gs4 gsy fd-column js-auth-item "">
                <button class=""grid--cell s-btn s-btn__primary"" id=""submit-button"" name=""submit-button"">Log in</button>
                        <p class=""grid--cell s-input-message js-error-message d-none"">
            
        </p>

            </div>

        <input type=""hidden"" id=""oauth_version"" name=""oauth_version"">
        <input type=""hidden"" id=""oauth_server"" name=""oauth_server"">


    </form>
";

        [Test]
        public void GetFormsOfHtmlDocument_ShouldDetectBwappLoginCorrectly()
        {
            // arrange
            var doc = new HtmlDocument();
            doc.LoadHtml(HtmlBeeWappLogin);
            
            // act
            var formInfo = FormDataParser.GetFormsOfHtmlDocument(doc, "beewapp.com");
            var form = formInfo[0];
            
            // assert
            Assert.AreEqual("/login.php", form.Action);
            Assert.AreEqual("POST", form.Method);
            Assert.AreEqual(3, form.InputFields.Count);
            Assert.AreEqual(1, form.SelectFields.Count);
        }
        
        [Test]
        public void GetFormsOfHtmlDocument_ShouldDetectStackOverFlowLoginCorrectly()
        {
            // arrange
            var doc = new HtmlDocument();
            doc.LoadHtml(StackOverflowLoginHtml);
            
            // act
            var formInfo = FormDataParser.GetFormsOfHtmlDocument(doc, "beewapp.com");
            var form = formInfo[0];
            
            // assert
            Assert.AreEqual("/users/login", form.Action);
            Assert.AreEqual("POST", form.Method);
            Assert.AreEqual(7, form.InputFields.Count);
            Assert.AreEqual(0, form.SelectFields.Count);
        }
    }
}