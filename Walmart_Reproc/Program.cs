using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Walmart_Reproc
{
    internal class Program
    {
        private static void Main()
        {
            GetUserInput();
        }

        private static void GetUserInput()
        {
            Console.WriteLine("Enter in XML directory:");
            var dir = Console.ReadLine();

            SearchXmlFiles(dir);
        }

        private static void SearchXmlFiles(string dir)
        {
            if (Directory.Exists(dir))
            {
                Console.Clear();
                Console.WriteLine("Processing Files in: " + dir + "\n");
                Console.WriteLine("=============================================================\n");
                //gathers all files in directory
                var xmlFiles = Directory.EnumerateFiles(dir, "*.xml");

                if (!xmlFiles.Any())
                {
                    Console.WriteLine("No XML file found. Press Enter to exit.");
                }
                else
                {
                    foreach (var file in xmlFiles)
                    {
                        XmlParse(file);
                    }

                    Console.WriteLine("Service Completed. Press Enter to exit.");
                }

                Console.ReadLine();

            }
            else
            {
                Console.WriteLine("\nInvalid directory path. Try Again \n");
                GetUserInput();
            }
        }

        private static void XmlParse(string file)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.PreserveWhitespace = true;
            xDoc.Load(file);
            XmlNamespaceManager oManager = new XmlNamespaceManager(xDoc.NameTable);
            oManager.AddNamespace("ns", "http://www.spscommerce.com/RSX");

            Console.WriteLine("Processing: " + file);
            ////TermsDeferredAmountDue
            try
            {
                var termsDeferredAmountDue = xDoc.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:Header/ns:PaymentTerms/ns:TermsDeferredAmountDue[1]", oManager).InnerText;
                xDoc.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:Header/ns:PaymentTerms/ns:TermsDeferredAmountDue[1]", oManager).InnerText = PercentConversion(termsDeferredAmountDue);
            }
            catch (Exception e) { Console.WriteLine("Warning: Element not found - <TermsDeferredAmountDue>"); }

            ////TotalAmount
            try
            {
                var totalAmount = xDoc.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:Summary/ns:TotalAmount[1]", oManager).InnerText;
                xDoc.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:Summary/ns:TotalAmount[1]", oManager).InnerText = PercentConversion(totalAmount);
            }
            catch (Exception e) { Console.WriteLine("Warning: Element not found - <TotalAmount>"); }

            ////TotalNetSalesAmount
            try
            {
                var totalNetSalesAmount = xDoc.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:Summary/ns:TotalNetSalesAmount[1]", oManager).InnerText;
                xDoc.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:Summary/ns:TotalNetSalesAmount[1]", oManager).InnerText = PercentConversion(totalNetSalesAmount);
            }
            catch (Exception e) { Console.WriteLine("Warning: Element not found - <TotalNetSalesAmount>"); }

            ////InvoiceAmtDueByTermsDate
            try
            {
                var invoiceAmtDueByTermsDate = xDoc.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:Summary/ns:InvoiceAmtDueByTermsDate[1]", oManager).InnerText;
                xDoc.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:Summary/ns:InvoiceAmtDueByTermsDate[1]", oManager).InnerText = PercentConversion(invoiceAmtDueByTermsDate);
            }
            catch (Exception e) { Console.WriteLine("Warning: Element not found - <InvoiceAmtDueByTermsDate>"); }


            var i = 0;
            foreach (XmlElement element in xDoc.SelectNodes("/ns:Invoices/ns:Invoice/ns:LineItems/ns:LineItem/ns:InvoiceLine", oManager))
            {
                i += 1;
                try
                {
                    var extendedItemTotal = element.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:LineItems/ns:LineItem[" + i + "]/ns:InvoiceLine/ns:ExtendedItemTotal[1]", oManager).InnerText;
                    element.SelectSingleNode("/ns:Invoices/ns:Invoice/ns:LineItems/ns:LineItem[" + i + "]/ns:InvoiceLine/ns:ExtendedItemTotal[1]", oManager).InnerText = PercentConversion(extendedItemTotal);
                }
                catch (Exception e) { Console.WriteLine("Error: Issue with <LineItem Row(" + i + ")>/<ExtendedItemTotal>"); }
            }

            xDoc.Save(file);
            Console.WriteLine("End File ----------------------------------------------------\n\n");
        }

        private static string PercentConversion(string val)
        {
            var doubleConv = Convert.ToDouble(val);
            var percentConv = doubleConv + (doubleConv * 2 / 100);

            var newStringVal = DoFormat(percentConv);

            return newStringVal;
        }

        public static string DoFormat(double myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        }
    }
}
