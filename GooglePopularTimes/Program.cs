using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System;
using System.Data;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace GooglePopularTimes
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Tuple<string, string>> places = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("https://www.google.com/search?q=shoprite+edison+nj&rlz=1C1CHBF_enUS843US843&oq=shoprite+edison+nj&aqs=chrome..69i57.3164j0j9&sourceid=chrome&ie=UTF-8","ShopRite Edison NJ"),
                new Tuple<string, string>("https://www.google.com/search?rlz=1C1CHBF_enUS843US843&ei=Scy9XrnCK9eDytMPkLu90Aw&q=wendys+edison+nj&oq=wendys+edison+nj&gs_lcp=CgZwc3ktYWIQAzIECAAQCjIECAAQCjIICAAQFhAKEB4yBggAEBYQHjIGCAAQFhAeMgYIABAWEB4yCAgAEBYQChAeMgYIABAWEB4yBggAEBYQHjIGCAAQFhAeOggIABCDARCRAjoFCAAQkQI6AggAOgUIABCDAToECAAQQzoHCAAQgwEQQzoHCAAQChCRAjoGCAAQChBDUP0WWMciYKAkaABwAHgAgAGlAYgBhxKSAQQwLjE2mAEAoAEBqgEHZ3dzLXdpeg&sclient=psy-ab&ved=0ahUKEwj5ir-zubTpAhXXgXIEHZBdD8oQ4dUDCAw&uact=5", "Wendy's Edison NJ")
            };

            PopularTimesLive(places);
            //PopularTimes("https://www.google.com/search?q=shoprite+edison+nj&rlz=1C1CHBF_enUS843US843&oq=shop&aqs=chrome.0.69i59j69i57j0l3j69i61l2j69i60.674j0j4&sourceid=chrome&ie=UTF-8");
        }


        /// <summary>
        /// Collects the popular times for a specific business.
        /// It is logged for each day and hour.
        /// </summary>
        /// <param name="URL">The Google page for the business</param>
        public static void PopularTimes(string URL)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(URL);            

            string storeName = driver.FindElement(By.XPath("//*[@id=\"rhs\"]/div/div[1]/div/div[1]/div/div[1]/div[2]/div[2]/div[1]/div/div/div[1]/span")).Text;
            storeName = storeName.TrimEnd('.', ',', '[', ']', ' ');
            storeName = Regex.Replace(storeName, " ", "_");

            DataTable dt = new DataTable(storeName);
            dt.Columns.Add("ExtractDate", typeof(DateTime));
            dt.Columns.Add("Day", typeof(string));
            dt.Columns.Add("12 AM", typeof(double));
            dt.Columns.Add("1 AM", typeof(double));
            dt.Columns.Add("2 AM", typeof(double));
            dt.Columns.Add("3 AM", typeof(double));
            dt.Columns.Add("4 AM", typeof(double));
            dt.Columns.Add("5 AM", typeof(double));
            dt.Columns.Add("6 AM", typeof(double));
            dt.Columns.Add("7 AM", typeof(double));
            dt.Columns.Add("8 AM", typeof(double));
            dt.Columns.Add("9 AM", typeof(double));
            dt.Columns.Add("10 AM", typeof(double));
            dt.Columns.Add("11 AM", typeof(double));
            dt.Columns.Add("12 PM", typeof(double));
            dt.Columns.Add("1 PM", typeof(double));
            dt.Columns.Add("2 PM", typeof(double));
            dt.Columns.Add("3 PM", typeof(double));
            dt.Columns.Add("4 PM", typeof(double));
            dt.Columns.Add("5 PM", typeof(double));
            dt.Columns.Add("6 PM", typeof(double));
            dt.Columns.Add("7 PM", typeof(double));
            dt.Columns.Add("8 PM", typeof(double));
            dt.Columns.Add("9 PM", typeof(double));
            dt.Columns.Add("10 PM", typeof(double));
            dt.Columns.Add("11 PM", typeof(double));

            try
            {

                var days = driver.FindElements(By.CssSelector("div[class=\"ecodF vL1E9b\"]"));
                foreach (var d in days)
                {
                    DataRow row = dt.NewRow();
                    string dayName = Regex.Match(d.GetAttribute("aria-label"), "(?<=on ).*(?=s)").Value;

                    if(String.IsNullOrEmpty(dayName))
                    {
                        // Business is closed on this day.
                        //var closedDays = d.FindElements(By.CssSelector("div[class=\"BvDC2e ePSZhc\"]")).Text;
                        dayName = "CLOSED";
                    }
                    else
                    {
                        var hours = d.FindElement(By.ClassName("yPHXsc"));
                        var lubhBars = hours.FindElements(By.ClassName("lubh-bar"));
                        foreach (var b in lubhBars)
                        {
                            string time = Regex.Match(b.GetAttribute("aria-label"), "[0-9]{1,2} (AM|PM)").Value;
                            string height = Regex.Match(b.GetAttribute("style"), "(?<=height: )[0-9]+(?=px)").Value;
                            row[time] = height;
                        }
                    }                    
                    row["ExtractDate"] = DateTime.Now;
                    row["Day"] = dayName;
                    dt.Rows.Add(row);
                }

                ExportToCSV(dt, storeName);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                Console.WriteLine("Maybe this store does not have a Popular Times graph?");
                return;
            }
        }

        /// <summary>
        /// Collect the Popular Times live data for a list of businesses.
        /// </summary>
        /// <param name="locations">A tuple that contains the URL of the google page and the name of the business.</param>
        public static void PopularTimesLive(List<Tuple<string,string>> locations)
        {
            List<Business> businesses = new List<Business>();
            foreach (var l in locations)
            {
                businesses.Add(new Business(l.Item1,l.Item2));
            }

            IWebDriver driver = new ChromeDriver();
            string liveHour = "";
            string liveDay = "";
            double height = 0;            

            while (true)
            {                               
                foreach(Business b in businesses)
                {
                    try
                    {
                        // Try to get the current popularity of a business.
                        driver.Navigate().GoToUrl(b.URL);

                        liveDay = driver.FindElements(By.CssSelector("div[class=\"ecodF vL1E9b\"]")).Where(x => x.GetAttribute("aria-hidden").ToString() == "true").First().GetAttribute("aria-label");
                        liveDay = Regex.Match(liveDay, "(?<=Histogram showing popular times on ).*(?=s)").Value;

                        if(b.currentDay != liveDay && b.currentDay != null)
                        {
                            // It is a new day. Flush the buffer.
                            b.buffer["ExtractDate"] = DateTime.Now;
                            b.popularTimes.Rows.Add(b.buffer);                            
                            b.buffer = b.popularTimes.NewRow();
                            b.currentDay = liveDay;
                            b.ExportToCSV();
                        }

                        var hour = driver.FindElement(By.CssSelector("div[class=\"lubh-bar lubh-sel\"]"));
                        liveHour = Regex.Match(hour.GetAttribute("aria-label"), "[0-9]{1,2} (PM|AM)(?=:)").Value;
                        
                        var liveBar = driver.FindElement(By.ClassName("ZQ55mf"));
                        height = Convert.ToDouble(Regex.Match(liveBar.GetAttribute("style"), "(?<=height: )[0-9]+(\\.[0-9]+)?(?=px)").Value);
                        
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(DateTime.Now + "Could not find the live bar. Will check next hour");
                        b.buffer[liveHour] = 0;
                        b.buffer["Day"] = liveDay;
                        b.currentDay = liveDay;
                    }

                    b.buffer[liveHour] = height;
                    b.buffer["Day"] = liveDay;
                    b.currentDay = liveDay;
                    Console.WriteLine($"[{DateTime.Now}] [{b.name}] Logged {height} for hour {liveHour} on {liveDay}");
                }

                // All of the live data for this hour has been logged. Wait until the next hour.
                var timeOfDay = DateTime.Now.TimeOfDay;
                var nextFullHour = TimeSpan.FromHours(Math.Ceiling(timeOfDay.TotalHours));
                int delta = Convert.ToInt32((nextFullHour - timeOfDay).TotalMilliseconds);
                System.Threading.Thread.Sleep(delta + 5000); // Wait until the next hour plus five minutes
            }




        }

        public static void ExportToCSV(DataTable dt, string name)
        {
            StringBuilder sb = new StringBuilder();

            string[] columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            string fileName = name + "_" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv";
            File.WriteAllText(fileName, sb.ToString());
        }

    }
}
