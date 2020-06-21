using ConsoleTables;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using WebDriverManager.DriverConfigs.Impl;

namespace BottlenoseGradeChecker
{
    /// <summary>
    /// Provides web-related functionality.
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// Attempts to login to the handin server endpoint via Selenium and print any results found.
        /// </summary>
        /// <param name="user">The username credential to use.</param>
        /// <param name="pass">The password credential to use.</param>
        public static void Login(string user, string pass)
        {
            string courseName = "";
            string gradeMin = "";
            string gradeSoFar = "";
            string gradeMax = "";
            List<Assignment> assignments = new List<Assignment>();

            new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());

            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            var options = new ChromeOptions();
            options.AddArgument("--headless");

            ChromeDriver driver = null;
            try
            {
                using (driver = new ChromeDriver(service, options))
                {
                    driver.Url = "https://handins.ccs.neu.edu/login";
                    driver.Navigate();

                    var waitU = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    waitU.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[3]/form/div/div[2]/div[2]/div[1]/input")));

                    var waitP = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    waitP.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[3]/form/div/div[2]/div[2]/div[2]/input")));

                    //user and pass
                    driver.FindElementByXPath("/html/body/div[3]/form/div/div[2]/div[2]/div[1]/input").SendKeys(user);
                    driver.FindElementByXPath("/html/body/div[3]/form/div/div[2]/div[2]/div[2]/input").SendKeys(pass);

                    //login
                    driver.FindElementByXPath("/html/body/div[3]/form/div/div[2]/div[2]/div[3]/input").Click();

                    courseName = driver.FindElementByXPath("/html/body/div[3]/div[1]/h3/a").Text;

                    driver.Url = $"{driver.FindElementByXPath("/html/body/div[3]/div[1]/h3/a").GetAttribute("href")}/assignments";
                    driver.Navigate();

                    assignments = ParseTable(driver.FindElementByClassName("table"));
                    gradeMin = driver.FindElementByXPath("/html/body/div[3]/div[3]/table/tbody/tr[1]/td[2]/span").Text;
                    gradeSoFar = driver.FindElementByXPath("/html/body/div[3]/div[3]/table/tbody/tr[2]/td[2]/span").Text;
                    gradeMax = driver.FindElementByXPath("/html/body/div[3]/div[3]/table/tbody/tr[3]/td[2]/span").Text;

                    driver.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Login failed. Please verify your username/password and try again. ({e.GetType().Name})\n");
                return;
            }
            finally
            {
                if (driver != null)
                {
                    try
                    {
                        driver.Close();
                    }
                    catch { }
                }
            }

            Console.WriteLine($"\nShowing grades for class: {courseName}");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine($"Minimum Grade: {gradeMin}");
            Console.WriteLine($"Grade So Far: {gradeSoFar}");
            Console.WriteLine($"Maximum Grade: {gradeMax}");

            Console.WriteLine("\n\nAssignments (Showing 10):\n");
            PrintAssignments(assignments);
        }

        /// <summary>
        /// Parses an HTMl table element into a list of Assignments.
        /// </summary>
        /// <param name="table">An HTML table element from Bottlenose.</param>
        /// <returns>The assignments contained in the table.</returns>
        private static List<Assignment> ParseTable(IWebElement table)
        {
            List<Assignment> result = new List<Assignment>();

            // Rows
            var rows = table.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
            foreach (var row in rows)
            {
                string name = "";
                string due = "";
                string weight = "";
                string score = "";

                // Columns
                var cols = row.FindElements(By.TagName("td"));
                for (int i = 0; i < 4; i++)
                {
                    var curItem = cols[i];
                    switch (i)
                    {
                        // Name
                        case 0:
                            name = curItem.FindElement(By.TagName("a")).Text;
                            break;
                        // Due date
                        case 1:
                            due = curItem.FindElement(By.TagName("span")).Text;
                            break;
                        // Weight
                        case 2:
                            weight = curItem.Text;
                            break;
                        // Score
                        case 3:
                            score = curItem.Text;
                            break;
                    }
                }
                result.Add(new Assignment(name, due, weight, score));
            }
            return result;
        }

        /// <summary>
        /// Prints a list of Assignments to the console as a formatted table.
        /// </summary>
        /// <param name="assignments">An input list of assignments.</param>
        private static void PrintAssignments(List<Assignment> assignments) =>
            Console.WriteLine(ConsoleTable.From<Assignment>(assignments.GetRange(0, 10)).ToMinimalString());

        /// <summary>
        /// Represents a single Assignment found within a Bottlenose course.
        /// </summary>
        private class Assignment
        {
            public string Name { get; private set; }
            public string Due { get; private set; }
            public string Weight { get; private set; }
            public string Score { get; private set; }

            public Assignment(string Name, string Due, string Weight, string Score)
            {
                this.Name = Name;
                this.Due = Due;
                this.Weight = Weight;
                this.Score = Score;
            }
        }
    }
}
