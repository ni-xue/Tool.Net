using System;

namespace Tool.Utils
{
    /// <summary>
    /// 一个完全独立于系统的时间流速器
    /// </summary>
    public partial class TimestampedClock
    {
        private readonly DateTime _startTimeUtc;
        private readonly long _startTimestamp;
        private readonly TimeZoneInfo _deviceTimeZone; // 设备所在时区
        private DateTime _nowUtc;
        private DateTime _lastUpdateUtc;
        private double _timeElapsedMilliseconds;

        /// <summary>
        /// 构造一个独立的时间区域
        /// </summary>
        /// <param name="timestampMilliseconds">初始时间戳</param>
        public TimestampedClock(long timestampMilliseconds)
        {
            _startTimestamp = timestampMilliseconds;
            _startTimeUtc = DateTimeOffset.FromUnixTimeMilliseconds(timestampMilliseconds).UtcDateTime;
            _nowUtc = _startTimeUtc;
            _lastUpdateUtc = DateTime.UtcNow;
            _timeElapsedMilliseconds = 0;
            _deviceTimeZone = TimeZoneInfo.Local; // 获取设备所在时区
        }

        /// <summary>
        /// 当前日期时间
        /// </summary>
        public DateTime Now
        {
            get
            {
                DateTime currentTimeUtc = DateTime.UtcNow;
                TimeSpan timeSinceLastUpdate = currentTimeUtc - _lastUpdateUtc;
                _timeElapsedMilliseconds += timeSinceLastUpdate.TotalMilliseconds;

                _nowUtc = _startTimeUtc.AddMilliseconds(_timeElapsedMilliseconds);
                _lastUpdateUtc = currentTimeUtc;

                // 将 UTC 时间转换为设备所在时区的时间
                return TimeZoneInfo.ConvertTimeFromUtc(_nowUtc, _deviceTimeZone);
            }
        }

        /// <summary>
        /// 开始的时间戳
        /// </summary>
        public long StartTimestamp => _startTimestamp;

        /// <summary>
        /// 获取设备所在时区的起始时间 Utc
        /// </summary>
        public DateTime StartTimeUtc => _startTimeUtc;

        /// <summary>
        /// 获取设备所在时区的起始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return TimeZoneInfo.ConvertTimeFromUtc(_startTimeUtc, _deviceTimeZone); }
        }

        /// <summary>
        /// 获取设备所在时区的今天的日期（不包括时间部分）
        /// </summary>
        public DateTime Today
        {
            get
            {
                DateTime nowInDeviceTimeZone = Now; // 获取设备时区的当前时间
                return nowInDeviceTimeZone.Date; // 获取日期部分
            }
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        public TimeSpan ElapsedTime => Now - StartTime;

        /// <summary>
        /// 例如:20250101 = 2025年1月1日
        /// </summary>
        public string DateText => Now.ToString("yyyyMMdd");

        /// <summary>
        /// 例如:01 = 第一周
        /// </summary>
        public string WeekNumberCode => GetWeeks().ToString().PadLeft(2, '0');

        /// <summary>
        /// 例如:2501 = 20年第一周
        /// </summary>
        public string YearWeekCode => string.Concat(DateText.AsSpan(2, 2), WeekNumberCode);

        #region 其他公共杂项

        /// <summary>
        /// 获取周数
        /// </summary>
        /// <returns></returns>
        public int GetWeeks()
        {
            //一.找到第一周的最后一天（先获取1月1日是星期几，从而得知第一周周末是几）
            int firstWeekend = 7 - (int)new DateTime(Today.Year, 1, 1).DayOfWeek;
            //二.获取今天是一年当中的第几天
            int currentDay = Today.DayOfYear;
            int rawWeek = (int)Math.Ceiling((currentDay - firstWeekend) / 7.0);
            return Math.Max(1, rawWeek + 1); // 避免返回 0 周
        }

        /// <summary>
        /// 获得星期几的数字
        /// </summary>
        /// <returns></returns>
        public int GetToday()
        {
            int A = 0;
            switch (GetDayName())
            {
                case "星期一":
                    A = 1;
                    break;
                case "星期二":
                    A = 2;
                    break;
                case "星期三":
                    A = 3;
                    break;
                case "星期四":
                    A = 4;
                    break;
                case "星期五":
                    A = 5;
                    break;
                case "星期六":
                    A = 6;
                    break;
                case "星期日":
                    A = 7;
                    break;
            }
            return A;
        }

        /// <summary>
        /// 获取星期几
        /// </summary>
        /// <returns></returns>
        public string GetDayName()
        {
            return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Now.DayOfWeek);
        }

        /// <summary>
        /// 获得本周的周一
        /// </summary>
        /// <returns></returns>
        public string GetThisWeekMonday()
        {
            DateTime date = Now;
            DateTime firstDate;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    goto default;
                case DayOfWeek.Tuesday:
                    firstDate = date.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    firstDate = date.AddDays(-2);
                    break;
                case DayOfWeek.Thursday:
                    firstDate = date.AddDays(-3);
                    break;
                case DayOfWeek.Friday:
                    firstDate = date.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    firstDate = date.AddDays(-5);
                    break;
                case DayOfWeek.Sunday:
                    firstDate = date.AddDays(-6);
                    break;
                default:
                    firstDate = date;
                    break;
            }
            return firstDate.ToString("yyyyMMdd");
        }  //获取周一日期

        /// <summary>
        /// 获取今年第一天
        /// </summary>
        /// <returns></returns>
        public string GetTheWeekNum()
        {
            //创建公历日历对象
            System.Globalization.GregorianCalendar gregorianCalendar = new();
            //获取指定日期是周数 CalendarWeekRule指定 第一周开始于该年的第一天，DayOfWeek指定每周第一天是星期几
            int weekOfYear = gregorianCalendar.GetWeekOfYear(Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear.ToString();
        }

        #endregion
    }
}
