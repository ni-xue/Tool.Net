using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 对DateTime进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DateTimeExtension
    {
        //public static bool operator > (int s, int x)
        //{
        //    return true;
        //}
        //public static bool operator  >= (int s, int x)
        //{
        //    return true;
        //}
        //public static bool operator == (int s, int x)
        //{
        //    return true;
        //}
        //public static bool operator != (int s, int x)
        //{
        //    return true;
        //}
        //public static bool operator < (int s, int x)
        //{
        //    return true;
        //}
        //public static bool operator <= (int s, int x)
        //{
        //    return true;
        //}

        /// <summary>
        /// 根据实力，计算与当前时间的毫秒差
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns>适用于获取代码执行时的耗时</returns>
        public static double GetMilliseconds(this DateTime dateTime)
        {
            return DateTime.Now.Subtract(dateTime).TotalMilliseconds;
        }

        /// <summary>
        /// 返回当前日期指定的星期几
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <param name="Week">指定星期几</param>
        /// <returns>返回当前日期指定的星期几</returns>
        public static DateTime DateMonday(this DateTime dateTime, DayOfWeek Week)
        {
            int date = 0;
            List<DayOfWeek> dayOfs = new List<DayOfWeek>() { dateTime.DayOfWeek, Week };
            int[] weeks = { };
            foreach (DayOfWeek week in dayOfs)
            {
                switch (week)
                {
                    case DayOfWeek.Monday: date = 1; break;
                    case DayOfWeek.Tuesday: date = 2; break;
                    case DayOfWeek.Wednesday: date = 3; break;
                    case DayOfWeek.Thursday: date = 4; break;
                    case DayOfWeek.Friday: date = 5; break;
                    case DayOfWeek.Saturday: date = 6; break;
                    case DayOfWeek.Sunday: date = 7; break;
                }
                weeks = weeks.Add(date);
            }

             //weeks[0].CompareTo(weeks[1]) > 0 ? weeks[0] - weeks[1] : weeks[1] - weeks[0];
            if (weeks[0].CompareTo(weeks[1]) == 0)
            {
                return dateTime.ToString("yyyy-MM-dd 00:00:00").ToDateTime();
            }
            else
            {
                date = weeks[1] - weeks[0];
                return dateTime.AddDays(date).ToString("yyyy-MM-dd 00:00:00").ToDateTime();
            }

            //var dayOfWeek = dateTime;//DateTime.Now;
            //var day = DateTime.Now;
            //TimeSpan ts = new TimeSpan();
            //if (Week == dayOfWeek.DayOfWeek)
            //{

            //}

            //Double douLen = 0; //ts.TotalSeconds;
            //switch (dayOfWeek.DayOfWeek)
            //{
            //    case DayOfWeek.Monday:
            //        date = 1;
            //        day = Convert.ToDateTime(dayOfWeek.ToString("yyyy-MM-dd 23:59:59"));
            //        ts = dayOfWeek.Subtract(day).Duration();
            //        douLen = ts.TotalSeconds;
            //        break;
            //    case DayOfWeek.Tuesday:
            //        date = 2;
            //        day = Convert.ToDateTime(dayOfWeek.AddDays(6).ToString("yyyy-MM-dd 23:59:59"));
            //        ts = dayOfWeek.Subtract(day).Duration();
            //        douLen = ts.TotalSeconds;
            //        break;
            //    case DayOfWeek.Wednesday:
            //        date = 3;
            //        day = Convert.ToDateTime(dayOfWeek.AddDays(5).ToString("yyyy-MM-dd 23:59:59"));
            //        ts = dayOfWeek.Subtract(day).Duration();
            //        douLen = ts.TotalSeconds;
            //        break;
            //    case DayOfWeek.Thursday:
            //        date = 4;
            //        day = Convert.ToDateTime(dayOfWeek.AddDays(4).ToString("yyyy-MM-dd 23:59:59"));
            //        ts = dayOfWeek.Subtract(day).Duration();
            //        douLen = ts.TotalSeconds;
            //        break;
            //    case DayOfWeek.Friday:
            //        date = 5;
            //        day = Convert.ToDateTime(dayOfWeek.AddDays(3).ToString("yyyy-MM-dd 23:59:59"));
            //        ts = dayOfWeek.Subtract(day).Duration();
            //        douLen = ts.TotalSeconds;
            //        break;
            //    case DayOfWeek.Saturday:
            //        date = 6;
            //        day = Convert.ToDateTime(dayOfWeek.AddDays(2).ToString("yyyy-MM-dd 23:59:59"));
            //        ts = dayOfWeek.Subtract(day).Duration();
            //        douLen = ts.TotalSeconds;
            //        break;
            //    case DayOfWeek.Sunday:
            //        date = 7;
            //        day = Convert.ToDateTime(dayOfWeek.AddDays(1).ToString("yyyy-MM-dd 23:59:59"));
            //        ts = dayOfWeek.Subtract(day).Duration();
            //        douLen = ts.TotalSeconds;
            //        break;
            //}
            //return dateTime; //douLen.ToString().Split('.')[0]; //date.ToString();
        }


        #region 返回本年有多少天
        /// <summary>本年有多少天</summary>  
        /// <param name="idt">日期</param>
        /// <returns>本天在当年的天数</returns>  
        public static int GetDaysOfYear(this DateTime idt)
        {
            int n;
            //取得传入参数的年份部分，用来判断是否是闰年  
            n = idt.Year;
            if (IsRuYear(n))
            {
                //闰年多 1 天 即：2 月为 29 天  
                return 366;
            }
            else
            {
                //--非闰年少1天 即：2 月为 28 天  
                return 365;
            }
        }
        #endregion

        #region 返回本月有多少天
        /// <summary>本月有多少天</summary>  
        /// <param name="dt">日期</param>  
        /// <returns>天数</returns>  
        public static int GetDaysOfMonth(this DateTime dt)
        {
            //--------------------------------//  
            //--从dt中取得当前的年，月信息  --//  
            //--------------------------------//  
            int year, month, days = 0;
            year = dt.Year;
            month = dt.Month;

            //--利用年月信息，得到当前月的天数信息。  
            switch (month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    if (IsRuYear(year))
                    {
                        //闰年多 1 天 即：2 月为 29 天  
                        days = 29;
                    }
                    else
                    {
                        //--非闰年少1天 即：2 月为 28 天  
                        days = 28;
                    }

                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }
            return days;
        }
        #endregion

        #region 返回当前日期的星期名称
        /// <summary>返回当前日期的星期名称</summary>  
        /// <param name="idt">日期</param>
        /// <returns>星期名称</returns>  
        public static string GetWeekNameOfDay(this DateTime idt)
        {
            string dt, week = "";

            dt = idt.DayOfWeek.ToString();
            switch (dt)
            {
                case "Mondy":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期六";
                    break;
                case "Sunday":
                    week = "星期日";
                    break;
            }
            return week;
        }
        #endregion

        #region 返回当前日期的星期编号
        /// <summary>返回当前日期的星期编号</summary>  
        /// <param name="idt">日期</param>  
        /// <returns>星期数字编号</returns>  
        public static string GetWeekNumberOfDay(this DateTime idt)
        {
            string dt, week = "";

            dt = idt.DayOfWeek.ToString();
            switch (dt)
            {
                case "Mondy":
                    week = "1";
                    break;
                case "Tuesday":
                    week = "2";
                    break;
                case "Wednesday":
                    week = "3";
                    break;
                case "Thursday":
                    week = "4";
                    break;
                case "Friday":
                    week = "5";
                    break;
                case "Saturday":
                    week = "6";
                    break;
                case "Sunday":
                    week = "7";
                    break;

            }
            return week;
        }
        #endregion

        /// <summary>
        /// 返回 <see cref="DateTime"/> 类型 ，根据指定时间数字转换
        /// </summary>
        /// <param name="Localtime">指定的时间数字</param>
        /// <param name="tal">true 毫秒, false 秒。</param>
        /// <returns>返回时间类型</returns>
        public static DateTime ToLocalTime(double Localtime , bool tal)//DateTime dateTime,
        {
            if (tal)
            {
                return new DateTime(1970, 1, 1).ToLocalTime().AddMilliseconds(Localtime);
                //return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Localtime);
            }
            else
            {
                return new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(Localtime);

                //DateTime  dateTime1 = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).AddSeconds(Localtime);
                //return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(Localtime);
            }
             
        }

        /// <summary>
        /// 返回 <see cref="DateTime"/> 类型 ，根据指定时间数字转换
        /// </summary>
        /// <param name="dateTime">时间类型</param>
        /// <param name="tal">返回 true 毫秒, false 秒。</param>
        /// <returns>返回64位时间数字</returns>
        public static long ToLocalTime(this DateTime dateTime,bool tal)
        {
            if (tal)
            {
                return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds.ToVar<long>();
                //return (long)(dateTime - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalMilliseconds;
            }
            else
            {
                return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds.ToVar<long>();
                //return (long)(dateTime - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }

        #region 判断当前年份是否是闰年，私有函数
        /// <summary>判断当前年份是否是闰年，私有函数</summary>  
        /// <param name="iYear">年份</param>  
        /// <returns>是闰年：True ，不是闰年：False</returns>  
        private static bool IsRuYear(int iYear)
        {
            //形式参数为年份  
            //例如：2003  
            int n;
            n = iYear;

            if ((n % 400 == 0) || (n % 4 == 0 && n % 100 != 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region DateTime[] 封装方法

        /// <summary>
        /// 给数组加新的值
        /// </summary>
        /// <param name="txt">DateTime[]</param>
        /// <param name="txt1">新增的值</param>
        public static DateTime[] Add(this DateTime[] txt, DateTime txt1)
        {
            var add = txt.ToList();
            add.Add(txt1);
            txt = add.ToArray();
            //txt.Initialize();
            return txt;
        }

        /// <summary>
        /// 查找该char数组中是否存在该值。
        /// </summary>
        /// <param name="txt">DateTime[]</param>
        /// <param name="txt1">查找的字符</param>
        /// <returns>方法存在或不存在</returns>
        public static bool Contains(this DateTime[] txt, DateTime txt1)
        {
            return txt.Contains<DateTime>(txt1);
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static DateTime[] GetArrayIndex(this DateTime[] obj, int index, int count)
        {
            if (obj == null)
            {
                throw new System.SystemException("该DateTime为空！");
            }
            if (index > count)
            {
                throw new System.SystemException("count不能小于index，数组越界！");
            }
            if (index < 0)
            {
                throw new System.SystemException("index不能小于0，数组越界！");
            }
            if (count < 0)
            {
                throw new System.SystemException("count不能小于0，数组越界！");
            }
            if (obj.Length < index)
            {
                throw new System.SystemException("index超出了数组，数组越界！");
            }
            if (obj.Length < count)
            {
                throw new System.SystemException("count超出了数组，数组越界！");
            }
            List<DateTime> obj1 = new List<DateTime>();

            for (int i = index; i < count; i++)
            {
                obj1.Add(obj[i]);
            }
            return obj1.ToArray();
        }
        #endregion

    }
}
