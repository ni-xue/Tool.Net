using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Tool.Utils
{
    /// <summary>
    /// 文件管理器
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class FileManager
	{
		/// <summary>
		/// 复制目录
		/// </summary>
		/// <param name="srcDir">原目录</param>
		/// <param name="desDir">到目录</param>
		public static void CopyDirectories(string srcDir, string desDir)
		{
			try
			{
				DirectoryInfo dInfo = new(srcDir);
				FileManager.CopyDirectoryInfo(dInfo, srcDir, desDir);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		/// <summary>
		/// 复制目录信息
		/// </summary>
		/// <param name="dInfo">DirectoryInfo 原目录对象</param>
		/// <param name="srcDir">原目录</param>
		/// <param name="desDir">到目录</param>
		private static void CopyDirectoryInfo(DirectoryInfo dInfo, string srcDir, string desDir)
		{
			if (!FileManager.Exists(desDir, FsoMethod.Folder))
			{
				FileManager.Create(desDir, FsoMethod.Folder);
			}
			DirectoryInfo[] directories = dInfo.GetDirectories();
			DirectoryInfo[] array = directories;
			for (int i = 0; i < array.Length; i++)
			{
				DirectoryInfo directoryInfo = array[i];
				FileManager.CopyDirectoryInfo(directoryInfo, directoryInfo.FullName, desDir + directoryInfo.FullName.Replace(srcDir, ""));
			}
			FileInfo[] files = dInfo.GetFiles();
			FileInfo[] array2 = files;
			for (int j = 0; j < array2.Length; j++)
			{
				FileInfo fileInfo = array2[j];
				FileManager.CopyFile(fileInfo.FullName, desDir + fileInfo.FullName.Replace(srcDir, ""));
			}
		}

		/// <summary>
		/// 复制文件
		/// </summary>
		/// <param name="srcFile">原文件</param>
		/// <param name="desFile">新文件的位置</param>
		public static void CopyFile(string srcFile, string desFile)
		{
			try
			{
				File.Copy(srcFile, desFile, true);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		/// <summary>
		/// 复制文件流
		/// </summary>
		/// <param name="srcFile">原文件</param>
		/// <param name="desFile">到文件</param>
		/// <returns></returns>
		public static bool CopyFileStream(string srcFile, string desFile)
		{
			bool result;
			try
			{
				FileStream fileStream = new(srcFile, FileMode.Open, FileAccess.Read);
				FileStream fileStream2 = new(desFile, FileMode.Create, FileAccess.Write);
				BinaryReader binaryReader = new(fileStream);
				BinaryWriter binaryWriter = new(fileStream2);
				binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				binaryReader.BaseStream.Seek(0L, SeekOrigin.End);
				while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
				{
					binaryWriter.Write(binaryReader.ReadByte());
				}
				binaryReader.Close();
				binaryWriter.Close();
				fileStream.Flush();
				fileStream.Close();
				fileStream2.Flush();
				fileStream2.Close();
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// 创建文件
		/// </summary>
		/// <param name="file">路径</param>
		/// <param name="method">文件类型</param>
		public static void Create(string file, FsoMethod method)
		{
			try
			{
				if (method == FsoMethod.File)
				{
					FileManager.WriteFile(file, "");
				}
				else if (method == FsoMethod.Folder)
				{
					Directory.CreateDirectory(file);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="file">路径</param>
		/// <param name="method">文件类型</param>
		public static void Delete(string file, FsoMethod method)
		{
			try
			{
				if (method == FsoMethod.File)
				{
					File.Delete(file);
				}
				if (method == FsoMethod.Folder)
				{
					Directory.Delete(file, true);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		private static long[] DirInfo(DirectoryInfo directory)
		{
			long[] array = new long[3];
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			FileInfo[] files = directory.GetFiles();
			num3 += (long)files.Length;
			FileInfo[] array2 = files;
			for (int i = 0; i < array2.Length; i++)
			{
				FileInfo fileInfo = array2[i];
				num += fileInfo.Length;
			}
			DirectoryInfo[] directories = directory.GetDirectories();
			num2 += (long)directories.Length;
			DirectoryInfo[] array3 = directories;
			for (int j = 0; j < array3.Length; j++)
			{
				DirectoryInfo directory2 = array3[j];
				num += FileManager.DirInfo(directory2)[0];
				num2 += FileManager.DirInfo(directory2)[1];
				num3 += FileManager.DirInfo(directory2)[2];
			}
			array[0] = num;
			array[1] = num2;
			array[2] = num3;
			return array;
		}

		/// <summary>
		/// 是否存在文件或文件夹
		/// </summary>
		/// <param name="file">路径</param>
		/// <param name="method">文件类型</param>
		/// <returns></returns>
		public static bool Exists(string file, FsoMethod method)
		{
			bool result2;
			try
			{
				if (method == FsoMethod.File)
				{
					bool result = File.Exists(file);
					return result;
				}
				if (method == FsoMethod.Folder)
				{
					bool result = Directory.Exists(file);
					return result;
				}
				result2 = false;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
			return result2;
		}

		/// <summary>
		/// 获取路径下的所有文件夹
		/// </summary>
		/// <param name="directory">路径</param>
		/// <returns></returns>
		private static string[] GetDirectories(string directory)
		{
			return Directory.GetDirectories(directory);
		}

		/// <summary>
		/// 获取目录文件列表
		/// </summary>
		/// <param name="directory">路径</param>
		/// <param name="method">类型</param>
		/// <returns></returns>
		public static DataTable GetDirectoryFilesList(string directory, FsoMethod method)
		{
			DataTable dataTable = new();
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("FullName");
			dataTable.Columns.Add("ContentType");
			dataTable.Columns.Add("Type");
			dataTable.Columns.Add("Path");
			dataTable.Columns.Add("LastWriteTime");
			dataTable.Columns.Add("Length");
			if (method != FsoMethod.File)
			{
				for (int i = 0; i < FileManager.GetDirectories(directory).Length; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					DirectoryInfo directoryInfo = new(FileManager.GetDirectories(directory)[i]);
					dataRow[0] = directoryInfo.Name;
					dataRow[1] = directoryInfo.FullName;
					dataRow[2] = "";
					dataRow[3] = 0;
					dataRow[4] = directoryInfo.FullName.Replace(directoryInfo.Name, "");
					dataRow[5] = directoryInfo.LastWriteTime;
					dataRow[6] = "";
					dataTable.Rows.Add(dataRow);
				}
			}
			if (method != FsoMethod.Folder)
			{
				for (int i = 0; i < FileManager.GetFiles(directory).Length; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					FileInfo fileInfo = new(FileManager.GetFiles(directory)[i]);
					dataRow[0] = fileInfo.Name;
					dataRow[1] = fileInfo.FullName;
					dataRow[2] = fileInfo.Extension.Replace(".", "");
					dataRow[3] = 1;
					dataRow[4] = fileInfo.DirectoryName + "\\";
					dataRow[5] = fileInfo.LastWriteTime;
					dataRow[6] = fileInfo.Length;
					dataTable.Rows.Add(dataRow);
				}
			}
			return dataTable;
		}

		/// <summary>
		/// 获取对象的目录文件列表
		/// </summary>
		/// <param name="directory">路径</param>
		/// <param name="method">类型</param>
		/// <returns></returns>
		public static IList<FolderInfo> GetDirectoryFilesListForObject(string directory, FsoMethod method)
		{
			return DataHelper.ConvertDataTableToObjects<FolderInfo>(FileManager.GetDirectoryFilesList(directory, method));
		}

		/// <summary>
		/// 获取对象的目录列表
		/// </summary>
		/// <param name="directory">路径</param>
		/// <param name="method">类型</param>
		/// <returns></returns>
		public static IList<FolderInfo> GetDirectoryListForObject(string directory, FsoMethod method)
		{
			return DataHelper.ConvertDataTableToObjects<FolderInfo>(FileManager.GetDirectoryList(directory, method));
		}

		/// <summary>
		/// 获取目录信息
		/// </summary>
		/// <param name="directory">路径</param>
		/// <returns></returns>
		public static long[] GetDirectoryInfo(string directory)
		{
			DirectoryInfo directory2 = new(directory);
			return FileManager.DirInfo(directory2);
		}

		private static DataTable GetDirectoryList(DirectoryInfo directoryInfo, FsoMethod method)
		{
			DataTable dataTable = new();
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("FullName");
			dataTable.Columns.Add("ContentType");
			dataTable.Columns.Add("Type");
			dataTable.Columns.Add("Path");
			dataTable.Columns.Add("LastWriteTime");
			dataTable.Columns.Add("Length");
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			DirectoryInfo[] array = directories;
			for (int i = 0; i < array.Length; i++)
			{
				DirectoryInfo directoryInfo2 = array[i];
				if (method == FsoMethod.File)
				{
					dataTable = FileManager.Merge(dataTable, FileManager.GetDirectoryList(directoryInfo2, method));
				}
				else
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow[0] = directoryInfo2.Name;
					dataRow[1] = directoryInfo2.FullName;
					dataRow[2] = "";
					dataRow[3] = 0;
					dataRow[4] = directoryInfo2.FullName.Replace(directoryInfo2.Name, "");
					dataRow[5] = directoryInfo2.LastWriteTime;
					dataRow[6] = "";
					dataTable.Rows.Add(dataRow);
					dataTable = FileManager.Merge(dataTable, FileManager.GetDirectoryList(directoryInfo2, method));
				}
			}
			if (method != FsoMethod.Folder)
			{
				FileInfo[] files = directoryInfo.GetFiles();
				FileInfo[] array2 = files;
				for (int j = 0; j < array2.Length; j++)
				{
					FileInfo fileInfo = array2[j];
					DataRow dataRow = dataTable.NewRow();
					dataRow[0] = fileInfo.Name;
					dataRow[1] = fileInfo.FullName;
					dataRow[2] = fileInfo.Extension.Replace(".", "");
					dataRow[3] = 1;
					dataRow[4] = fileInfo.DirectoryName + "\\";
					dataRow[5] = fileInfo.LastWriteTime;
					dataRow[6] = fileInfo.Length;
					dataTable.Rows.Add(dataRow);
				}
			}
			return dataTable;
		}

		/// <summary>
		/// 获取目录列表
		/// </summary>
		/// <param name="directory">路径</param>
		/// <param name="method">类型</param>
		/// <returns></returns>
		public static DataTable GetDirectoryList(string directory, FsoMethod method)
		{
			DataTable directoryList;
			try
			{
				DirectoryInfo directoryInfo = new(directory);
				directoryList = FileManager.GetDirectoryList(directoryInfo, method);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
			return directoryList;
		}

		/// <summary>
		/// 获取路径下的全部文件名称
		/// </summary>
		/// <param name="directory">路径</param>
		/// <returns></returns>
		private static string[] GetFiles(string directory)
		{
			return Directory.GetFiles(directory);
		}

		private static DataTable Merge(DataTable parent, DataTable child)
		{
			for (int i = 0; i < child.Rows.Count; i++)
			{
				DataRow dataRow = parent.NewRow();
				for (int j = 0; j < parent.Columns.Count; j++)
				{
					dataRow[j] = child.Rows[i][j];
				}
				parent.Rows.Add(dataRow);
			}
			return parent;
		}

		/// <summary>
		/// 移动
		/// </summary>
		/// <param name="srcFile">原路径</param>
		/// <param name="desFile">现路径</param>
		/// <param name="method">类型</param>
		public static void Move(string srcFile, string desFile, FsoMethod method)
		{
			try
			{
				if (method == FsoMethod.File)
				{
					File.Move(srcFile, desFile);
				}
				if (method == FsoMethod.Folder)
				{
					Directory.Move(srcFile, desFile);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		/// <summary>
		/// 读取文件
		/// </summary>
		/// <param name="file">路径</param>
		/// <returns></returns>
		public static string ReadFile(string file)
		{
			return FileManager.ReadFile(file, Encoding.UTF8);
		}

		/// <summary>
		/// 读取文件
		/// </summary>
		/// <param name="file">路径</param>
		/// <param name="encoding">编码格式</param>
		/// <returns></returns>
		public static string ReadFile(string file, Encoding encoding)
		{
			string result = "";
			FileStream fileStream = new(file, FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new(fileStream, encoding);
			try
			{
				result = streamReader.ReadToEnd();
			}
			catch
			{
			}
			finally
			{
				fileStream.Flush();
				fileStream.Close();
				streamReader.Close();
			}
			return result;
		}

		/// <summary>
		/// 读取文件返回字节
		/// </summary>
		/// <param name="filePath">路径</param>
		/// <returns></returns>
		public static byte[] ReadFileReturnBytes(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}
			FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			BinaryReader binaryReader = new(fileStream);
			byte[] result = binaryReader.ReadBytes((int)fileStream.Length);
			fileStream.Flush();
			fileStream.Close();
			binaryReader.Close();
			return result;
		}

		/// <summary>
		/// 写入Buff ToFile 文件
		/// </summary>
		/// <param name="buff">数据</param>
		/// <param name="filePath">路径</param>
		public static void WriteBuffToFile(byte[] buff, string filePath)
		{
			FileManager.WriteBuffToFile(buff, 0, buff.Length, filePath);
		}

		/// <summary>
		///  写入Buff ToFile 文件
		/// </summary>
		/// <param name="buff">数据</param>
		/// <param name="offset">开始</param>
		/// <param name="len">结束</param>
		/// <param name="filePath">路径</param>
		public static void WriteBuffToFile(byte[] buff, int offset, int len, string filePath)
		{
			string directoryName = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
			BinaryWriter binaryWriter = new(fileStream);
			binaryWriter.Write(buff, offset, len);
			binaryWriter.Flush();
			binaryWriter.Close();
			fileStream.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">路径</param>
		/// <param name="fileContent">内容</param>
		public static void WriteFile(string file, string fileContent)
		{
			FileManager.WriteFile(file, fileContent, Encoding.UTF8);
		}

		/// <summary>
		/// 写入文件
		/// </summary>
		/// <param name="file">路径</param>
		/// <param name="fileContent">内容</param>
		/// <param name="encoding">数据类型</param>
		public static void WriteFile(string file, string fileContent, Encoding encoding)
		{
			FileInfo fileInfo = new FileInfo(file);
			if (!Directory.Exists(fileInfo.DirectoryName))
			{
				Directory.CreateDirectory(fileInfo.DirectoryName);
			}
			FileStream fileStream = new(file, FileMode.Create, FileAccess.Write);
			StreamWriter streamWriter = new(fileStream, encoding);
			try
			{
				streamWriter.Write(fileContent);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
			finally
			{
				streamWriter.Flush();
				fileStream.Flush();
				streamWriter.Close();
				fileStream.Close();
			}
		}

		/// <summary>
		/// 写入文件
		/// </summary>
		/// <param name="file">路径</param>
		/// <param name="fileContent">内容</param>
		/// <param name="append">是否追加内容</param>
		public static void WriteFile(string file, string fileContent, bool append)
		{
			FileManager.WriteFile(file, fileContent, append, Encoding.UTF8);
		}

		/// <summary>
		/// 写入文件
		/// </summary>
		/// <param name="file">路径</param>
		/// <param name="fileContent">内容</param>
		/// <param name="append">是否追加内容</param>
		/// <param name="encoding">数据类型</param>
		public static void WriteFile(string file, string fileContent, bool append, Encoding encoding)
		{
			FileInfo fileInfo = new(file);
			if (!Directory.Exists(fileInfo.DirectoryName))
			{
				Directory.CreateDirectory(fileInfo.DirectoryName);
			}
			StreamWriter streamWriter = new(file, append, encoding);
			try
			{
				streamWriter.Write(fileContent);
			}
			finally
			{
				streamWriter.Flush();
				streamWriter.Close();
			}
		}

		/// <summary>
		/// 用来探测一个日志文件的id
		/// </summary>
		/// <param name="directory">路径</param>
		/// <param name="levelName">日志名称</param>
		/// <param name="i">存在数量</param>
		/// <param name="sparepath">备用文件名</param>
		/// <returns></returns>
		public static string GetCurrentLogName(string directory, string levelName, ref uint i, out string sparepath)
		{
            string path2;
            if (i > 0)
			{
				path2 = $"{directory}{levelName}Log{DateTime.Now:yyyy-MM}[{i}].log";
				//path2 = string.Concat(new object[]
				//{
				//	directory,
				//	$"{levelName}Log",
				//	DateTime.Now.ToString("yyyy-MM"),
				//	"[",
				//	i,
				//	"].log"
				//});
				sparepath = path2;
			}
			else
			{
				path2 = $"{directory}{levelName}Log{DateTime.Now:yyyy-MM}.log";// path;
				sparepath = string.Empty;
			}
			if (File.Exists(path2))
			{
				i++;
				string path3 = FileManager.GetCurrentLogName(directory, levelName, ref i, out sparepath);
				return path3.Equals(sparepath) ? path2 : path3;
			}
			return path2;
		}
	}
}
