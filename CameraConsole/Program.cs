using System;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CameraConsole
{

    class Program
        {
            static void Main(string[] args)
            {
                Console.WriteLine("--- Download data sample ---");
                WebClient webclient = new WebClient();
                webclient.Headers.Add("Referer", "http://datahus.se"); // Replace with your domain here
                                                                           
                webclient.UploadStringCompleted += (obj, arguments) =>
                {
                    if (arguments.Cancelled == true)
                    {
                        Console.Write("Request cancelled by user");
                    }
                    else if (arguments.Error != null)
                    {
                        Console.WriteLine(arguments.Error.Message);
                        Console.Write("Request Failed");
                    }
                    else
                    {
                        Console.WriteLine(formatXML(arguments.Result));
                        Console.Write("Data downloaded");
                    }
                    Console.WriteLine(", press 'X' to exit.");
                };

                try
                {
                    // API server url
                    Uri address = new Uri("http://api.trafikinfo.trafikverket.se/v1.3/data.xml");
                    string requestBody = "<REQUEST>" +
                                            // Use your valid authenticationkey
                                            "<LOGIN authenticationkey='5a9f02a344d24e3a957e77ac847d21ac'/>" +

                                            "<QUERY objecttype='Camera' schemaversion='1.3' limit='10' >" +
                                                "<FILTER>" + 
                                                     "<IN name='Active' value='true'/>" +
                                                "</FILTER>" +
                                         "<EXCLUDE>Deleted</EXCLUDE>" +
                                            "</QUERY>" +
                                        "</REQUEST>";

                    webclient.Headers["Content-Type"] = "text/xml";
                    Console.WriteLine("Fetching data ... (press 'C' to cancel)");
                    webclient.UploadStringAsync(address, "POST", requestBody);
                }
                catch (UriFormatException)
                {
                    Console.WriteLine("Malformed url, press 'X' to exit.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("An error occured, press 'X' to exit.");
                }

                char keychar = ' ';
                while (keychar != 'X')
                {
                    keychar = Char.ToUpper(Console.ReadKey().KeyChar);
                    if (keychar == 'C')
                    {
                        webclient.CancelAsync();
                    }
                }
            }

            // Format xml so it is readable by humans.
            private static string formatXML(string xml)
            {
                // Format xml.
                XDocument rxml = XDocument.Parse(xml);
                XmlWriterSettings xmlsettings = new XmlWriterSettings();
                xmlsettings.OmitXmlDeclaration = true;
                xmlsettings.Indent = true;
                xmlsettings.IndentChars = "      ";
                var sb = new StringBuilder();
                using (XmlWriter xmlWriter = XmlWriter.Create(sb, xmlsettings))
                {
                    rxml.WriteTo(xmlWriter);
                }
                return sb.ToString();
            }
        }
    
}
