using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GooglePopularTimes
{
    public class Business
    {
        public string name { get; }
        public string URL { get; }
        public DataTable popularTimes { get; set; }
        public DataRow buffer { get; set; }
        public string currentDay { get; set; }

        public Business(string URL, string businessName)
        {
            this.name = businessName;
            this.URL = URL;
            this.popularTimes = new DataTable(businessName);
            this.popularTimes.Columns.Add("ExtractDate", typeof(DateTime));
            this.popularTimes.Columns.Add("Day", typeof(string));
            this.popularTimes.Columns.Add("12 AM", typeof(double));
            this.popularTimes.Columns.Add("1 AM", typeof(double));
            this.popularTimes.Columns.Add("2 AM", typeof(double));
            this.popularTimes.Columns.Add("3 AM", typeof(double));
            this.popularTimes.Columns.Add("4 AM", typeof(double));
            this.popularTimes.Columns.Add("5 AM", typeof(double));
            this.popularTimes.Columns.Add("6 AM", typeof(double));
            this.popularTimes.Columns.Add("7 AM", typeof(double));
            this.popularTimes.Columns.Add("8 AM", typeof(double));
            this.popularTimes.Columns.Add("9 AM", typeof(double));
            this.popularTimes.Columns.Add("10 AM", typeof(double));
            this.popularTimes.Columns.Add("11 AM", typeof(double));
            this.popularTimes.Columns.Add("12 PM", typeof(double));
            this.popularTimes.Columns.Add("1 PM", typeof(double));
            this.popularTimes.Columns.Add("2 PM", typeof(double));
            this.popularTimes.Columns.Add("3 PM", typeof(double));
            this.popularTimes.Columns.Add("4 PM", typeof(double));
            this.popularTimes.Columns.Add("5 PM", typeof(double));
            this.popularTimes.Columns.Add("6 PM", typeof(double));
            this.popularTimes.Columns.Add("7 PM", typeof(double));
            this.popularTimes.Columns.Add("8 PM", typeof(double));
            this.popularTimes.Columns.Add("9 PM", typeof(double));
            this.popularTimes.Columns.Add("10 PM", typeof(double));
            this.popularTimes.Columns.Add("11 PM", typeof(double));
            this.buffer = this.popularTimes.NewRow();
        }

        
        public void ExportToCSV()
        {
            string businessName = this.name.TrimEnd('.', ',', '[', ']', ' ');
            businessName = Regex.Replace(businessName, " ", "_");

            StringBuilder sb = new StringBuilder();

            string[] columnNames = this.popularTimes.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in this.popularTimes.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            string fileName = businessName + "_" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv";
            File.WriteAllText(fileName, sb.ToString());
        }

    }
}
