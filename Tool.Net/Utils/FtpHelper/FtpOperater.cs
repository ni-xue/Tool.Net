using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tool.Utils.FtpHelper
{
    /// <summary>
    /// FTP操作类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class FtpOperater
    {
        #region 属性
        private FtpClient ftp;
        /// <summary>
        /// 全局FTP访问变量
        /// </summary>
        public FtpClient Ftp
        {
            get { return ftp; }
            set { ftp = value; }
        }

        private string _server;
        /// <summary>
        /// Ftp服务器
        /// </summary>
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        private string _User;
        /// <summary>
        /// Ftp用户
        /// </summary>
        public string User
        {
            get { return _User; }
            set { _User = value; }
        }

        private string _Pass;
        /// <summary>
        /// Ftp密码
        /// </summary>
        public string Pass
        {
            get { return _Pass; }
            set { _Pass = value; }
        }

        private string _FolderZJ;
        /// <summary>
        /// Ftp密码
        /// </summary>
        public string FolderZJ
        {
            get { return _FolderZJ; }
            set { _FolderZJ = value; }
        }

        private string _FolderWX;
        /// <summary>
        /// Ftp密码
        /// </summary>
        public string FolderWX
        {
            get { return _FolderWX; }
            set { _FolderWX = value; }
        }
        #endregion

        /// <summary>
        /// 得到文件列表
        /// </summary>
        /// <returns></returns>
        public string[] GetList(string strPath)
        {
            if (ftp == null) ftp = this.GetFtpClient();
            ftp.Connect();
            ftp.ChDir(strPath);
            return ftp.Dir("*");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="ftpFolder">ftp目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        /// <param name="localFolder">本地目录</param>
        /// <param name="localFileName">本地文件名</param>
        public bool GetFile(string ftpFolder, string ftpFileName, string localFolder, string localFileName)
        {
            try
            {
                if (ftp == null) ftp = this.GetFtpClient();
                if (!ftp.Connected)
                {
                    ftp.Connect();
                    ftp.ChDir(ftpFolder);
                }
                ftp.Get(ftpFileName, localFolder, localFileName);

                return true;
            }
            catch
            {
                try
                {
                    ftp.DisConnect();
                    ftp = null;
                }
                catch { ftp = null; }
                return false;
            }
        }

        /// <summary>
        /// 修改文件
        /// </summary>
        /// <param name="ftpFolder">本地目录</param>
        /// <param name="ftpFileName">本地文件名temp</param>
        /// <param name="localFolder">本地目录</param>
        /// <param name="localFileName">本地文件名</param>
        /// <param name="BscInfo"></param>
        /// <param name="path">获得应用程序所在的完整的路径</param>
        public bool AddMSCFile(string ftpFolder, string ftpFileName, string localFolder, string localFileName, string BscInfo, string path)
        {
            string sLine = "";
            string sResult = "";
            path = path.Substring(0, path.LastIndexOf("\\"));
            try
            {
                FileStream fsFile = new FileStream(ftpFolder + "\\" + ftpFileName, FileMode.Open);
                FileStream fsFileWrite = new FileStream(localFolder + "\\" + localFileName, FileMode.Create);
                StreamReader sr = new StreamReader(fsFile);
                StreamWriter sw = new StreamWriter(fsFileWrite);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                while (sr.Peek() > -1)
                {
                    sLine = sr.ReadToEnd();
                }
                string[] arStr = sLine.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < arStr.Length - 1; i++)
                {
                    sResult += BscInfo + "," + arStr[i].Trim() + "\n";
                }
                sr.Close();
                byte[] connect = new UTF8Encoding(true).GetBytes(sResult);
                fsFileWrite.Write(connect, 0, connect.Length);
                fsFileWrite.Flush();
                sw.Close();
                fsFile.Close();
                fsFileWrite.Close();
                return true;
            }
            catch //(Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="ftpFolder">ftp目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        public bool DelFile(string ftpFolder, string ftpFileName)
        {
            try
            {
                if (ftp == null) ftp = this.GetFtpClient();
                if (!ftp.Connected)
                {
                    ftp.Connect();
                    ftp.ChDir(ftpFolder);
                }
                ftp.Delete(ftpFileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="ftpFolder">ftp目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        public bool PutFile(string ftpFolder, string ftpFileName)
        {
            try
            {
                if (ftp == null) ftp = this.GetFtpClient();
                if (!ftp.Connected)
                {
                    ftp.Connect();
                    ftp.ChDir(ftpFolder);
                }
                ftp.Put(ftpFileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="ftpFolder">ftp目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        /// <param name="localFolder">本地目录</param>
        /// <param name="localFileName">本地文件名</param>
        public bool GetFileNoBinary(string ftpFolder, string ftpFileName, string localFolder, string localFileName)
        {
            try
            {
                if (ftp == null) ftp = this.GetFtpClient();
                if (!ftp.Connected)
                {
                    ftp.Connect();
                    ftp.ChDir(ftpFolder);
                }
                ftp.GetNoBinary(ftpFileName, localFolder, localFileName);
                return true;
            }
            catch
            {
                try
                {
                    ftp.DisConnect();
                    ftp = null;
                }
                catch
                {
                    ftp = null;
                }
                return false;
            }
        }

        /// <summary>
        /// 得到FTP上文件信息
        /// </summary>
        /// <param name="ftpFolder">FTP目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        public string GetFileInfo(string ftpFolder, string ftpFileName)
        {
            string strResult = "";
            try
            {
                if (ftp == null) ftp = this.GetFtpClient();
                if (ftp.Connected) ftp.DisConnect();
                ftp.Connect();
                ftp.ChDir(ftpFolder);
                strResult = ftp.GetFileInfo(ftpFileName);
                return strResult;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 测试FTP服务器是否可登陆
        /// </summary>
        public bool CanConnect()
        {
            if (ftp == null) ftp = this.GetFtpClient();
            try
            {
                ftp.Connect();
                ftp.DisConnect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 得到FTP上文件信息
        /// </summary>
        /// <param name="ftpFolder">FTP目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        public string GetFileInfoConnected(string ftpFolder, string ftpFileName)
        {
            string strResult = "";
            try
            {
                if (ftp == null) ftp = this.GetFtpClient();
                if (!ftp.Connected)
                {
                    ftp.Connect();
                    ftp.ChDir(ftpFolder);
                }
                strResult = ftp.GetFileInfo(ftpFileName);
                return strResult;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 得到文件列表
        /// </summary>
        /// <param name="ftpFolder">FTP目录</param>
        /// <param name="strMask"></param>
        /// <returns>FTP通配符号</returns>
        public string[] GetFileList(string ftpFolder, string strMask)
        {
            string[] strResult;
            try
            {
                if (ftp == null) ftp = this.GetFtpClient();
                if (!ftp.Connected)
                {
                    ftp.Connect();
                    ftp.ChDir(ftpFolder);
                }
                strResult = ftp.Dir(strMask);
                return strResult;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///得到FTP传输对象
        /// </summary>
        public FtpClient GetFtpClient()
        {
            FtpClient ft = new FtpClient
            {
                RemoteHost = this.Server,
                RemoteUser = this.User,
                RemotePass = this.Pass
            };
            return ft;
        }
    }
}
