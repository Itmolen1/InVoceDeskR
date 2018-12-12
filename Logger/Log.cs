using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Logger
{
   public class Log : Ilog
    {
        private static readonly Lazy<Log> instance = new Lazy<Log>(() => new Log());

        public static Log GetInstance
        {
            get
            {
                return instance.Value;
            }
        }

        public void LogException(string message)
        {

            string path = HttpContext.Current.Server.MapPath("/ErrorLog/");
            string fileNames = string.Format("{0}_{1}.log", "Exception", DateTime.Now.ToShortDateString());
            string logFilePath = string.Format(@"{0}\{1}", path, fileNames);


            string folderName = @"F:\RepoProject\InvoiceDiskLast\ErrorLog\";

            string pathString = System.IO.Path.Combine(folderName, "SubFolder");

            //string pathString2 = @"c:\Top-Level Folder\SubFolder2";

            //System.IO.Directory.CreateDirectory(pathString);

            // Create a file name for the file you want to create. 
            //string fileName = System.IO.Path.GetRandomFileName().Replace(".tmp", ".csv"); 
            //string fileName = System.IO.Path.GetTempFileName().Replace(".tmp", ".csv");
            string fileName = string.Format("{0}_{1}.log", "Exception", DateTime.Now.Second.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Day.ToString());

            pathString = System.IO.Path.Combine(folderName, fileName);

            // Verify the path that you have constructed.
           // Console.WriteLine("Path to my file: {0}\n", pathString);

            if (!System.IO.File.Exists(pathString))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(pathString))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("----------------------------------------");
                    sb.AppendLine(DateTime.Now.ToString());
                    sb.AppendLine(message);
                    fs.Close();
                    using (StreamWriter writer = new StreamWriter(pathString, true))
                    {
                        writer.Write(sb.ToString());
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", fileName);
                return;
            }
            
        }
    }
}
