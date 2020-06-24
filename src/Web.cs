using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.XPath;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BottlenoseGradeChecker
{
    /// <summary>
    /// Provides web-related functionality.
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// Attempts to login to Bottlenose and prints any results found.
        /// </summary>
        /// <param name="user">The username credential to use.</param>
        /// <param name="pass">The password credential to use.</param>
        public static async Task Login(string user, string pass)
        {
            string courseName = "";
            string gradeMin = "";
            string gradeSoFar = "";
            string gradeMax = "";
            List<Assignment> assignments = new List<Assignment>();

            using (var client = new HttpClient())
            using (var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies()))
            {
                var endpoint = "https://handins.ccs.neu.edu";

                // Obtain a CSRF token
                var initReq = await client.GetAsync(endpoint + "/login");
                var initContent = await initReq.Content.ReadAsStringAsync();
                var initDoc = await context.OpenAsync(req => req.Content(initContent));

                var authToken = ((IHtmlElement)initDoc.Body.SelectSingleNode("/html/head/meta[4]")).GetAttribute("content");

                // Use it to send a login request
                var loginPostContent = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        { "user[username]", user },
                        { "user[password]", pass },
                        { "authenticity_token", authToken }
                    });
                var loginReq = await client.PostAsync("https://handins.ccs.neu.edu/login", loginPostContent);
                var loginContent = await loginReq.Content.ReadAsStringAsync();
                var loginDoc = await context.OpenAsync(req => req.Content(loginContent));

                // Get the course the student is currently enrolled in
                var courseUrlElem = (IHtmlElement)loginDoc.Body.SelectSingleNode("//*[@id=\"term_0\"]/h3/a");
                if (courseUrlElem == null)
                {
                    throw new ArgumentException("Login failed. Please verify your username/password and try again.");
                }
                var courseUrl = courseUrlElem.GetAttribute("href");
                var courseReq = await client.GetAsync(endpoint + courseUrl + "/assignments");
                var courseContent = await courseReq.Content.ReadAsStringAsync();
                var courseDoc = await context.OpenAsync(req => req.Content(courseContent));

                // Find the student's assignments/grades
                var table = courseDoc.Body.QuerySelector<IHtmlTableElement>("table");

                courseName = courseDoc.Body.SelectSingleNode("/html/body/div[3]/div[1]/h1/a").TextContent.Trim();
                gradeMin = courseDoc.Body.SelectSingleNode("/html/body/div[3]/div[3]/table/tbody/tr[1]/td[2]/span").TextContent.Trim();
                gradeSoFar = courseDoc.Body.SelectSingleNode("/html/body/div[3]/div[3]/table/tbody/tr[2]/td[2]/span").TextContent.Trim();
                gradeMax = courseDoc.Body.SelectSingleNode("/html/body/div[3]/div[3]/table/tbody/tr[3]/td[2]/span").TextContent.Trim();
                assignments = ParseTable(table);
            }

            // Display results
            Console.WriteLine($"\nShowing grades for class: {courseName}");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine($"Minimum Grade: {gradeMin}");
            Console.WriteLine($"Grade So Far: {gradeSoFar}");
            Console.WriteLine($"Maximum Grade: {gradeMax}");
            Console.WriteLine("\n\nAssignments (Showing 10):\n");
            PrintAssignments(assignments);
        }

        /// <summary>
        /// Parses an HTML table into a list of assignments.
        /// </summary>
        /// <param name="table">The table of assignments from Bottlenose.</param>
        /// <returns>A list of assignments representing the contents of the table.</returns>
        private static List<Assignment> ParseTable(IHtmlTableElement table)
        {
            List<Assignment> result = new List<Assignment>();

            var body = table.QuerySelector<IHtmlTableSectionElement>("tbody");
            var rows = body.QuerySelectorAll<IHtmlTableRowElement>("tr");

            foreach (var row in rows)
            {
                string name = "";
                string due = "";
                string weight = "";
                string score = "";

                // Columns
                var cols = row.QuerySelectorAll<IHtmlTableDataCellElement>("td");
                for (int i = 0; i < 4; i++)
                {
                    var curItem = cols.ElementAt(i);
                    switch (i)
                    {
                        // Name
                        case 0:
                            name = curItem.QuerySelector("a").TextContent;
                            break;
                        // Due date
                        case 1:
                            due = curItem.QuerySelector("span").TextContent;
                            break;
                        // Weight
                        case 2:
                            weight = curItem.TextContent;
                            break;
                        // Score
                        case 3:
                            score = curItem.TextContent;
                            break;
                    }
                }
                result.Add(new Assignment(name.Trim(), due.Trim(), weight.Trim(), score.Trim()));
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
