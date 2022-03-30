using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BanderasAYB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SqlServer sql = new SqlServer();
            List<Sanborn> list = sql.getSanborns();
            String date = DateTime.Now.ToString("yyyyMMdd");
            int error = 0;

            MailAddress from = new MailAddress("juarezo@sanborns.com.mx", "Oscar Juarez");
            MailAddress to = new MailAddress("SappsiSoporte@sanborns.net");

            MailMessage message = new MailMessage(from, to);

            message.BodyEncoding = Encoding.UTF8;
            message.Body = "";
            message.IsBodyHtml = true;

            if (list != null)
            {
                foreach (Sanborn sanborn in list)
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{sanborn.ipSanborn}/xps/applications/ayb/resaybbkp/AYB.FLG");
                    request.Credentials = new NetworkCredential("primario", "5vr54p");
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    try
                    {
                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                        Console.Write($"Server {(sanborn.idSanborn + 1000)}: {response.StatusDescription}");
                        Log.writeLog($"Server {(sanborn.idSanborn + 1000)}: {response.StatusDescription}");

                        Stream responseStream = response.GetResponseStream();                        

                        StreamReader reader = new StreamReader(responseStream);
                        string sReader = reader.ReadLine().Trim();

                        Console.WriteLine($"Download complete, fecha {sReader}");
                        Log.writeLineLog($"Download complete, fecha {sReader}");

                        if (!sReader.Equals(date))
                        {
                            message.Body += "Revisar unidad " + (sanborn.idSanborn + 1000) + " con fecha " + sReader + "<br>";
                            error++;
                        }                        

                        reader.Close();
                        reader.Dispose();
                        response.Close();
                    }
                    catch (Exception ex)
                    {
                        message.Body += "Revisar unidad " + (sanborn.idSanborn + 1000) + ": " + ex.Message + "<br>";
                        error++;
                        Console.WriteLine(ex.Message);
                        Log.writeLineLog($"Server {(sanborn.idSanborn + 1000)}: {ex.Message}");
                    }
                }
                if (error == 0)
                    message.Body += "<p>Sin unidades pendientes.</p>";
                message.Subject = "Unidades que no respaldaron AYB.bak";

                SmtpClient client = new SmtpClient("10.128.10.32", 25);
                client.Credentials = new NetworkCredential("juarezo@sanborns.com.mx", "Cogo7454");

                try
                {
                    Console.WriteLine("Sending mail...");
                    Log.writeLineLog("Sending mail...");
                    client.Send(message);
                    Console.WriteLine("Email sent successfully!");
                    Log.writeLineLog("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Log.writeLineLog(ex.Message);
                }
            }
        }       
    }
}
