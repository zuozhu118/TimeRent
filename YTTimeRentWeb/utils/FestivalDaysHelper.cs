using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace YTTimeRentWeb.utils
{
    public class FestivalDaysHelper
    {
        #region 公历转农历

        #region 农历年

        static ChineseLunisolarCalendar cCalendar = new ChineseLunisolarCalendar();
        //cCalendar.MaxSupportedDateTime 返回支持的最大日期，即2101-1-28
        //cCalendar.MinSupportedDateTime  返回支持的最小日期，即1901－2-19

        /// <summary>
        /// 根据公历获取农历日期
        /// </summary>
        /// <param name="datetime">公历日期</param>
        /// <returns>格式：庚寅[虎]年正月十九</returns>
        public static string GetChineseDateTime(DateTime datetime)
        {
            int lyear = cCalendar.GetYear(datetime);
            int lmonth = cCalendar.GetMonth(datetime);
            int lday = cCalendar.GetDayOfMonth(datetime);

            //获取闰月， 0 则表示没有闰月
            int leapMonth = cCalendar.GetLeapMonth(lyear);

            bool isleap = false;

            if (leapMonth > 0)
            {
                if (leapMonth == lmonth)
                {
                    //闰月
                    isleap = true;
                    lmonth--;
                }
                else if (lmonth > leapMonth)
                {
                    lmonth--;
                }
            }

            return string.Concat(GetLunisolarYear(datetime.Year), "年", isleap ? "闰" : string.Empty, GetLunisolarMonth(lmonth), "月", GetLunisolarDay(lday));
        }


        /// <summary>
        /// 十天干
        /// </summary>
        private static string[] tiangan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };

        /// <summary>
        /// 十二地支
        /// </summary>
        private static string[] dizhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

        /// <summary>
        /// 十二生肖
        /// </summary>
        private static string[] shengxiao = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };


        /// <summary>
        /// 返回农历天干地支年 
        /// </summary>
        /// <param name="year">公历年</param>
        /// <returns></returns>
        public static string GetLunisolarYear(int year)
        {
            if (year > 3)
            {
                int tgIndex = (year - 4) % 10;
                int dzIndex = (year - 4) % 12;

                return string.Concat(tiangan[tgIndex], dizhi[dzIndex], "[", shengxiao[dzIndex], "]");

            }

            throw new ArgumentOutOfRangeException("无效的年份!");
        }
        #endregion

        #region 农历月
        /// <summary>
        /// 农历月
        /// </summary>
        private static string[] months = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二(腊)" };


        /// <summary>
        /// 返回农历月
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public static string GetLunisolarMonth(int month)
        {
            if (month < 13 && month > 0)
            {
                return months[month - 1];
            }

            throw new ArgumentOutOfRangeException("无效的月份!");
        }
        #endregion

        #region 农历日
        /// <summary>
        /// 
        /// </summary>
        private static string[] days1 = { "初", "十", "廿", "三" };

        /// <summary>
        /// 日
        /// </summary>
        private static string[] days = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };


        /// <summary>
        /// 返回农历日
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string GetLunisolarDay(int day)
        {
            if (day > 0 && day < 32)
            {
                if (day != 20 && day != 30)
                {
                    return string.Concat(days1[(day - 1) / 10], days[(day - 1) % 10]);
                }
                else
                {
                    return string.Concat(days[day / 10], days1[1]);
                }
            }

            throw new ArgumentOutOfRangeException("无效的日!");
        }

        #endregion

        #endregion

        #region 农历转公历

        /// <summary>
        /// 把农历转为公历
        /// </summary>
        /// <param name="date">农历日期，格式：20160621</param>
        /// <returns>DateTime</returns>
        public DateTime GetGregorian(string date)
        {
            date = date.Replace("-","");
            int year = Convert.ToInt32(date.Substring(0, 4));
            int month = Convert.ToInt32(date.Substring(4, 2));
            int second = Convert.ToInt32(date.Substring(6, 2));
            return cCalendar.ToDateTime(year, month, second, 0, 0, 0, 0);
        }

        #endregion
    }
}