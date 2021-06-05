using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.ThreadQueue
{
	/// <summary>
	/// 一个公共锁
	/// </summary>
	/// <remarks>代码由逆血提供支持</remarks>
	public class ActionLock
    {
		/// <summary>
		/// 锁定时长最大限制 （毫秒）
		/// </summary>
		public static int WaitTimeout { get; set; } = 120000;


		private static readonly object _lock = new();

		/// <summary>
		/// 上锁
		/// </summary>
		/// <returns>返回是否成功</returns>
		public static bool Start()
		{
			return Monitor.TryEnter(ActionLock._lock, ActionLock.WaitTimeout);
		}

		/// <summary>
		/// 解锁
		/// </summary>
		/// <returns>返回是否成功</returns>
		public static bool End()
		{
			if (Monitor.IsEntered(ActionLock._lock))
			{
				Monitor.Exit(ActionLock._lock);
				return true;
			}
			return false;
		}

	}
}
