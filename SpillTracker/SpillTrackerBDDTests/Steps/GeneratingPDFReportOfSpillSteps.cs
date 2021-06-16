using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SpillTrackerBDDTests.Steps
{
    [Binding]
    public sealed class GeneratingPDFReportOfSpillSteps
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

        private readonly ScenarioContext _ctx;
        private string _hostBaseName = @"https://localhost:5001/";
        public GeneratingPDFReportOfSpillSteps(ScenarioContext scenarioContext)
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
        
        [Given(@"username is TaliaK")]
        public void GivenUsernameIsTaliaK()
        {
            _ctx["UserName"] = "TaliaK";
        }
        
        [Given(@"user is logged In")]
        public void GivenUserIsLoggedIn()
        {
            // Fetch the webdriver
            IWebDriver driver = (IWebDriver)_ctx["WebDriver"];
            //go to the login page (waits until document.readystate is complete)
            driver.Navigate().GoToUrl(_hostBaseName + @"Identity/Account/Login");
            //Get username and password for the user currently under test, look them up in the backgruond table
            string firstName = (string)_ctx["UserName"];
            IEnumerable<TestUser> users = (IEnumerable<TestUser>)_ctx["Users"];
            TestUser u = users.Where(u => u.UserName == firstName).FirstOrDefault();
            

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
        
        [Given(@"user navigates to Form/Details/(.*) page")]
        public void GivenUserNavigatesToFormDetailsPage(int p0)
        {
            //Fetch the webdriver
            IWebDriver driver = (IWebDriver)_ctx["WebDriver"];
            //go to the form details page of a form (waits until document.readystate is complete)
            driver.Navigate().GoToUrl(_hostBaseName + @"Forms/Details/1");
        }
        
        [When(@"the user activates the generate pdf report button")]
        public void WhenTheUserActivatesTheGeneratePdfReportButton()
        {
            //Fetch the webdriver
            IWebDriver driver = (IWebDriver)_ctx["WebDriver"];
            driver.FindElement(By.LinkText("Generate PDF")).Click();
        }


        //**********************THEN**********************

        [Then(@"the pdf is displayed")]
        public void ThenThePDFIsDisplayed()
        {
            IWebDriver driver = (IWebDriver)_ctx["WebDriver"];
            Assert.That(driver.Url, Is.EqualTo(@"https://localhost:5001/pdf/PDFReport?id=1").IgnoreCase);
        }
    }
}
