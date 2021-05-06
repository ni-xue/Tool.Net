using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Utils
{
	/// <summary>
	/// 文件详情类
	/// </summary>
	public class FolderInfo
	{
		/// <summary>
		/// 无参构造
		/// </summary>
		public FolderInfo()
		{
			this.m_name = "";
			this.m_fullName = "";
			this.m_contentType = "";
			this.m_type = 0;
			this.m_fsoType = FsoMethod.Folder;
			this.m_path = "";
			this.m_lastWriteTime = DateTime.Now;
			this.m_length = 0L;
		}

		/// <summary>
		/// 有参构造
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fullName"></param>
		/// <param name="contentType"></param>
		/// <param name="type"></param>
		/// <param name="path"></param>
		/// <param name="lastWriteTime"></param>
		/// <param name="length"></param>
		public FolderInfo(string name, string fullName, string contentType, byte type, string path, DateTime lastWriteTime, long length)
		{
			this.m_name = name;
			this.m_fullName = fullName;
			this.m_contentType = contentType;
			this.m_type = type;
			this.m_path = path;
			this.m_lastWriteTime = lastWriteTime;
			this.m_length = length;
		}

		/// <summary>
		/// 内容类型
		/// </summary>
		public string ContentType
		{
			get
			{
				return this.m_contentType;
			}
			set
			{
				this.m_contentType = value;
			}
		}

		/// <summary>
		/// 文件类型
		/// </summary>
		public FsoMethod FsoType
		{
			get
			{
				return this.m_fsoType;
			}
			set
			{
				this.m_fsoType = value;
				this.m_type = (byte)value;
			}
		}

		/// <summary>
		/// 文件名称
		/// </summary>
		public string FullName
		{
			get
			{
				return this.m_fullName;
			}
			set
			{
				this.m_fullName = value;
			}
		}

		/// <summary>
		/// 修改日期
		/// </summary>
		public DateTime LastWriteTime
		{
			get
			{
				return this.m_lastWriteTime;
			}
			set
			{
				this.m_lastWriteTime = value;
			}
		}

		/// <summary>
		/// 大小
		/// </summary>
		public long Length
		{
			get
			{
				return this.m_length;
			}
			set
			{
				this.m_length = value;
			}
		}

		/// <summary>
		/// 名称
		/// </summary>
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		/// <summary>
		/// 路径
		/// </summary>
		public string Path
		{
			get
			{
				return this.m_path;
			}
			set
			{
				this.m_path = value;
			}
		}

		/// <summary>
		/// 文件类型
		/// </summary>
		public byte Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
				this.m_fsoType = (FsoMethod)value;
			}
		}

		private string m_contentType;

		private FsoMethod m_fsoType;

		private string m_fullName;

		private DateTime m_lastWriteTime;

		private long m_length;

		private string m_name;

		private string m_path;

		private byte m_type;
	}

	/// <summary>
	/// 文件类型
	/// </summary>
	public enum FsoMethod : byte
	{
		/// <summary>
		/// 文件夹
		/// </summary>
		Folder,
		/// <summary>
		/// 文件
		/// </summary>
		File,
		/// <summary>
		/// 全部
		/// </summary>
		All
	}
}
