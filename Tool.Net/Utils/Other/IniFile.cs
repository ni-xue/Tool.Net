using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tool.Utils.Other
{
    /// <summary>
    /// INI文件读写类。
    /// Copyright (C) Maticsoft
    /// </summary>
    public class IniFile
    {
        /// <summary>
        /// INI文件的路径
        /// </summary>
        public string path;

        /// <summary>
        /// 有参构造，ini文件的绝对路径
        /// </summary>
        /// <param name="INIPath"></param>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);
        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="Section">要在其中写入新字串的小节名称。这个字串不区分大小写</param>
        /// <param name="Key">要设置的项名或条目名。这个字串不区分大小写。用vbNullString可删除这个小节的所有设置项</param>
        /// <param name="Value">指定为这个项写入的字串值。用vbNullString表示删除这个项现有的字串</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="Section">欲在其中查找条目的小节名称。这个字串不区分大小写。如设为vbNullString，就在lpReturnedString缓冲区内装载这个ini文件所有小节的列表。</param>
        /// <param name="Key">欲获取的项名或条目名。这个字串不区分大小写。如设为vbNullString，就在lpReturnedString缓冲区内装载指定小节所有项的列表</param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="Section">欲在其中查找条目的小节名称。这个字串不区分大小写。如设为vbNullString，就在lpReturnedString缓冲区内装载这个ini文件所有小节的列表。</param>
        /// <param name="Key">欲获取的项名或条目名。这个字串不区分大小写。如设为vbNullString，就在lpReturnedString缓冲区内装载指定小节所有项的列表</param>
        /// <returns></returns>
        public byte[] IniReadValues(string Section, string Key)
        {
            byte[] temp = new byte[255];
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp;

        }

        /// <summary>
        /// 删除ini文件下所有段落
        /// </summary>
        public void ClearAllSection()
        {
            IniWriteValue(null, null, null);
        }

        /// <summary>
        /// 删除ini文件下personal段落下的所有键
        /// </summary>
        /// <param name="Section"></param>
        public void ClearSection(string Section)
        {
            IniWriteValue(Section, null, null);
        }
    }
}
