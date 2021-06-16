using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SpillTrackerBDDTests.Steps
{
    public class TestUser
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    [Binding]
    public class GeneratingPDFReportOfSpillSteps
    {
        private readonly ScenarioContext _ctx;
        private string _hostBaseName = @"https://localhost:44343/";
        private readonly IWebDriver _webDriver;
        public GeneratingPDFReportOfSpillSteps(ScenarioContext scenarioContext, IWebDriver webDriver)
        {
            _ctx = scenarioContext;
            FirefoxOptions options = new FirefoxOptions();
            options.AcceptInsecureCertificates = true;
            _ctx["WebDriver"] = new FirefoxDriver(options);
        }

        [Given(@"the following users exist")]
        public void GivenTheFollowingUsersExist(Table table)
        {
            //Save the user data from the table on the .feature file for other steps by using the tables createset method
            IEnumerable<TestUser> users = table.CreateSet<TestUser>();
            _ctx["Users"] = users;
        }

        [Given(@"username is '(.*)'")]
        public void GivenUsernameIs(string p0)
        {
            _ctx["UserName"] = p0;
        }

        [Given(@"User is logged In")]
        public void GivenUserIsLoggedIn()
        {
            //Fetch the webdriver
            IWebDriver driver = (IWebDriver)_ctx["WebDriver"];
            //go to the login page (waits until document.readystate is complete)
            driver.Navigate().GoToUrl(_hostBaseName + @"Identity/Account/Login");
            //Get username and password for the user currently under test, look them up in the backgruond table
            string firstName = (string)_ctx["FirstName"];
            IEnumerable<TestUser> users = (IEnumerable<TestUser>)_ctx["Users"];
            TestUser u = users.Where(u => u.FirstName == firstName).FirstOrDefault();
            if (u == null)
            {
                //must have been selecting from non-user
            }

            //Enter email and password into the input fields
            driver.FindElement(By.Id("Input_Email")).SendKeys(u.Email);
            driver.FindElement(By.Id("Input_Password")).SendKeys(u.Password);
            driver.FindElement(By.Id("account")).FindElement(By.CssSelector("button[type=submit]")).Click();
        }


        [Given(@"users role is Admin")]
        public void GivenUsersRoleIsAdmin()
        {
            _ctx["Role"] = "Admin";
        }
        
        [Given(@"user navigates to Form/Details/(.*) '(.*)'")]
        public void GivenUserNavigatesToFormDetails(int p0, string p1)
        {
            //Fetch the webdriver
            IWebDriver driver = (IWebDriver)_ctx["WebDriver"];
            //go to the form details page of a form (waits until document.readystate is complete)
            driver.Navigate().GoToUrl(_hostBaseName + @"Form/Details/1");
        }
        
        [When(@"the user activates the generate pdf report button")]
        public void WhenTheUserActivatesTheGeneratePdfReportButton()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"they are directed to the generated pdf report page '(.*)'")]
        public void ThenTheyAreDirectedToTheGeneratedPdfReportPage(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"They can see the pdf report\.")]
        public void ThenTheyCanSeeThePdfReport_()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
