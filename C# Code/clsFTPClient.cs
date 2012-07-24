using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SpecialProjectFTP
{
    /// <summary>
    /// FTP Client Class
    /// </summary>
    class clsFTPClient
    {
        /// <summary>
        /// Get List Of Files From FTP Server
        /// </summary>
        /// <returns></returns>
        public string[] GetFileList()
        {
            String ftpUserID = @"[userid]";
            String ftpPassword = @"[password]";
            String ftpServerIP = @"[url]";

            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;

            try
            {
                // Open FTP Server For Listing
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/[remotepath]/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());

                // Retrieve Listing Response
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                
                // to remove the trailing '\n'
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                // Clean Up If Error
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                downloadFiles = null;
                return downloadFiles;
            }
        }

        /// <summary>
        /// Download Specified File
        /// </summary>
        /// <param name="file">File TO Download</param>
        /// <returns>Success</returns>
         public Boolean Download(string file)
        {
            Boolean success = true;
            try
            {  
                String ftpUserID = @"[userid]";
                String ftpPassword = @"[password]";
                String ftpServerIP = @"url";
                String remoteDir = @"[remotepath]";
                String localDestnDir = @"[localpath]";

                // Open connection to FTP Server for Download of Specified File
                string uri = "ftp://" + ftpServerIP + "/" + remoteDir + "/" + file;
                Uri serverUri = new Uri(uri);
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return false;
                }       
                FtpWebRequest reqFTP;                
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + remoteDir + "/" + file));                                
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);                
                reqFTP.KeepAlive = false;                
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;                                
                reqFTP.UseBinary = true;
                reqFTP.Proxy = null;                 
                reqFTP.UsePassive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                //Get Response Stream
                Stream responseStream = response.GetResponseStream();
                // Create File Stream to Write Contents
                FileStream writeStream = new FileStream(localDestnDir + @"\" + file, FileMode.Create);                
                int Length = 2048;
                Byte[] buffer = new Byte[Length];
                int bytesRead = responseStream.Read(buffer, 0, Length);               

                // Keep Getting and Writing contents
                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, Length);
                }                
                writeStream.Close();
                response.Close(); 
            }
            catch (WebException wEx)
            {
                // Ooops!  Error
                success = false;
            }
            catch (Exception ex)
            {
                // Ooops!  Error
                success = false;
            }

             // Return Success
             return success;
         }

        /// <summary>
        /// Move File On FTP Server
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
         public String Move(string file)
         {
             try
             {
                 String ftpUserID = @"[userid]";
                 String ftpPassword = @"[password]";
                 String ftpServerIP = @"url";
                 String remoteDir = @"[remotepath]";
                 String localDestnDir = @"[localpath]";

                 string uri = "ftp://" + ftpServerIP + "/" + remoteDir + "/" + file;
                 Uri serverUri = new Uri(uri);
                 if (serverUri.Scheme != Uri.UriSchemeFtp)
                 {
                     return "Schema Error";
                 }

                 // Connect and Move Specied File To Alternate Directory
                 FtpWebRequest reqFTP;
                 reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + remoteDir + "/" + file));
                 reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                 reqFTP.KeepAlive = false;
                 reqFTP.Method = WebRequestMethods.Ftp.Rename;
                 reqFTP.UseBinary = true;
                 reqFTP.Proxy = null;
                 reqFTP.UsePassive = false;
                 reqFTP.RenameTo = @"archive/" + file;

                 // Get Response
                 FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                 Stream responseStream = response.GetResponseStream();
                 int Length = 2048;
                 Byte[] buffer = new Byte[Length];
                 int bytesRead = responseStream.Read(buffer, 0, Length);

                 // Keep Getting response
                 while (bytesRead > 0)
                 {
                     bytesRead = responseStream.Read(buffer, 0, Length);
                 }
                 response.Close();

                 return "";
             }
             catch (WebException wEx)
             {
                 return wEx.Message;//, "Archive Error");
             }
             catch (Exception ex)
             {
                 return ex.Message;//, "Archive Error");
             }
         }
    }
}
