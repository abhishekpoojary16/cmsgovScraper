using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using OpenQA.Selenium.PhantomJS;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;

namespace cmsgovScraper
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Initialize StringBuilder to write data 
            var csv = new StringBuilder();

            //read csv file
            using (var rd = new StreamReader(ConfigurationManager.AppSettings["sourcePath"]))
            {
                while (!rd.EndOfStream)
                {
                    var splits = rd.ReadLine().Split(',');
                    var first = Regex.Replace(splits[0].ToString(), @"[\""]", "", RegexOptions.None);//Id
                    var second = getnodeValue(first); //Scraped data corresponding to ID
                    var newLine = string.Format("{0},{1}", first, second); //concat 
                    csv.AppendLine(newLine);
                }
            }

            //Write StringBuilder onto file
            File.WriteAllText(ConfigurationManager.AppSettings["destinationPath"], csv.ToString());
        }

        public static string getnodeValue(string physicianId)
        {
            try
            {
                using (var driver = new PhantomJSDriver())
                {
                    driver.Url = "https://openpaymentsdata.cms.gov/physician/" + physicianId + "/summary";
                    driver.Navigate();
                    //the driver can now provide you with what you need (it will execute the script)
                    //get the source of the page
                    //var source = driver.PageSource;
                    var node = driver.FindElementByClassName("PaymentSummary__amount___VNcyt");
                    return node.Text.Replace("$", "");
                }
            }

            catch
            {
                return "#Error";
            }
        }
    }
}
