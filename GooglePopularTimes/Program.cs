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
            PopularTimes("https://www.google.com/search?q=shoprite+edison+nj&rlz=1C1CHBF_enUS843US843&oq=shop&aqs=chrome.0.69i59j69i57j0l3j69i61l2j69i60.674j0j4&sourceid=chrome&ie=UTF-8");
        }


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
                    var hours = d.FindElement(By.ClassName("yPHXsc"));
                    var lubhBars = hours.FindElements(By.ClassName("lubh-bar"));
                    foreach (var b in lubhBars)
                    {
                        string time = Regex.Match(b.GetAttribute("aria-label"), "[0-9]{1,2} (AM|PM)").Value;
                        string height = Regex.Match(b.GetAttribute("style"), "(?<=height: )[0-9]+(?=px)").Value;
                        row[time] = height;
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
