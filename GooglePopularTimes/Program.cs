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

namespace GooglePopularTimes
{
    class Program
    {
        static void Main(string[] args)
        {
            //PopularTimes("https://www.google.com/search?q=shoprite+edison+nj&rlz=1C1CHBF_enUS843US843&oq=shop&aqs=chrome.0.69i59j69i57j0l3j69i61l2j69i60.674j0j4&sourceid=chrome&ie=UTF-8");
            PopularTimesLive("https://www.google.com/search?q=shoprite+edison+nj&rlz=1C1CHBF_enUS843US843&oq=shoprite+edison+nj&aqs=chrome..69i57.3164j0j9&sourceid=chrome&ie=UTF-8");
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


        public static void PopularTimesLive(string URL)
        {
            try
            {
                IWebDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl(URL);

                string storeName = driver.FindElement(By.XPath("//*[@id=\"rhs\"]/div/div[1]/div/div[1]/div/div[1]/div[2]/div[2]/div[1]/div/div/div[1]/span")).Text;
                storeName = storeName.TrimEnd('.', ',', '[', ']', ' ');
                storeName = Regex.Replace(storeName, " ", "_");

                DataTable dt = new DataTable(storeName);
                dt.Columns.Add("ExtractDate", typeof(DateTime));
                dt.Columns.Add("Day", typeof(string));            
                dt.Columns.Add("0", typeof(double));
                dt.Columns.Add("1", typeof(double));
                dt.Columns.Add("2", typeof(double));
                dt.Columns.Add("3", typeof(double));
                dt.Columns.Add("4", typeof(double));
                dt.Columns.Add("5", typeof(double));
                dt.Columns.Add("6", typeof(double));
                dt.Columns.Add("7", typeof(double));
                dt.Columns.Add("8", typeof(double));
                dt.Columns.Add("9", typeof(double));
                dt.Columns.Add("10", typeof(double));
                dt.Columns.Add("11", typeof(double));
                dt.Columns.Add("12", typeof(double));
                dt.Columns.Add("13", typeof(double));
                dt.Columns.Add("14", typeof(double));
                dt.Columns.Add("15", typeof(double));
                dt.Columns.Add("16", typeof(double));
                dt.Columns.Add("17", typeof(double));
                dt.Columns.Add("18", typeof(double));
                dt.Columns.Add("19", typeof(double));
                dt.Columns.Add("20", typeof(double));
                dt.Columns.Add("21", typeof(double));
                dt.Columns.Add("22", typeof(double));
                dt.Columns.Add("23", typeof(double));
                dt.Columns.Add("24", typeof(double));

                DataRow row = dt.NewRow();

                string liveHour = "";
                string liveDay = "";
                string currentDay = DateTime.Today.ToString("yyyy-MM-dd");

                while(true)
                {
                    var timeOfDay = DateTime.Now.TimeOfDay;
                    var nextFullHour = TimeSpan.FromHours(Math.Ceiling(timeOfDay.TotalHours));
                    int delta = Convert.ToInt32((nextFullHour - timeOfDay).TotalMilliseconds);

                    liveHour = Convert.ToString(DateTime.Now.Hour);
                    liveDay = DateTime.Now.DayOfWeek.ToString();

                    // Check if it is the next day
                    if (currentDay != DateTime.Today.ToString("yyyy-MM-dd"))
                    {
                        currentDay = DateTime.Today.ToString("yyyy-MM-dd");
                        // It is a new day. Flush the data row
                        row["ExtractDate"] = DateTime.Now;
                        dt.Rows.Add(row);
                        ExportToCSV(dt, storeName);
                        row = dt.NewRow();
                    }


                    double height = 0;                    
                    try
                    {
                        var liveBar = driver.FindElement(By.ClassName("ZQ55mf"));
                        height = Convert.ToDouble(Regex.Match(liveBar.GetAttribute("style"), "(?<=height: )[0-9]+(\\.[0-9]+)?(?=px)").Value);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(DateTime.Now + "Could not find the live bar. Will check next hour");
                        row[liveHour] = 0;
                        row["Day"] = liveDay;
                        System.Threading.Thread.Sleep(delta + 5000); // Wait until the next hour plus five minutes
                        continue;
                    }
                                                        
                    
                    row[liveHour] = height;
                    row["Day"] = liveDay;
                    // Wait for the next hour to collect data
                    System.Threading.Thread.Sleep(delta + 5000); // Wait until the next hour plus five minutes
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                Console.WriteLine("Maybe this store does not have a Popular Times graph or live data?");
                return;
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
