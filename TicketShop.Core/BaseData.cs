using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TicketShop.Core
{
    public abstract class BaseData
    {
        //public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected static ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }

        /// <summary>
        /// Remove from cache all items whose key starts with the input prefix
        /// </summary>
        public static void RemoveFromCache(string key)
        {
            key = key.ToLower();
            List<string> items = new List<string>();

            items = Cache.Select(kvp => kvp.Key).ToList();

            foreach (string item in items)
            {
                if (item.ToLower().Contains(key))
                {
                    Cache.Remove(item);
                }
            }
        }

        public static void ClearCache()
        {
            var allKeys = Cache.Select(o => o.Key);
            Parallel.ForEach(allKeys, key => Cache.Remove(key));
        }

        /// <summary>
        /// Cache the input data
        /// </summary>
        protected static void CacheData(string key, object data)
        {
            if (data != null)
            {
                Cache.Add(key, data, DateTime.Now.AddDays(1), null);
            }
        }

        protected static int GetPageIndex(int pageIndex, int pageSize)
        {
            if (pageSize <= 0)
                return 0;
            else
                return (int)Math.Floor((double)pageIndex / (double)pageSize);
        }

        protected static bool BarCodeIsValid(string barCode)
        {
            bool result = false;
            if (barCode.Length == 12)
            {
                char[] codes = barCode.ToCharArray();
                foreach (char code in codes)
                {
                    int number;
                    if (!Int32.TryParse(code.ToString(), out number))
                    {
                        barCode = String.Empty;
                        break;
                    }
                }

                result = true;
            }
            else
            {
                barCode = String.Empty;
            }
            return result;
        }

        /// <summary>
        /// Generates BarCode in Int64 value
        /// The legacy system requires BarCode as Int64
        /// Remove upon get upgraded  
        /// </summary>
        /// <returns>BarCode in Int64 value</returns>
        protected static long GetBarCode()
        {
            long result = 0;
            Random random = new Random();
            while (result <= 0)
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                result = BitConverter.ToInt64(buf, 0);
            }
            return result;
        }

        /// <summary>
        /// Result of server operation
        /// </summary>
        public static string GetResult(int result)
        {
            switch (result)
            {
                case 0:
                    return String.Empty;
                case 1:
                    return "Неизвестная ошибка.";
                case 2:
                    return "Успешно";
                case 3:
                    return "Невозможно изменить статус билета";
                case 4:
                    return "Пользователь не может изменить статус билета";
                case 5:
                    return "Остаток времени до выступления не позволяет отменить заказ или билет";
                case 6:
                    return "Остаток времени до выступления не позволяет оплатить заказ или билет";
            }
            return string.Empty;
        }

        /// <summary>
        /// Status of seat, order, orderItem, ticket and so on
        /// </summary>
        public static string GetStatus(ItemStatus status)
        {
            switch (status)
            {
                case ItemStatus.None:
                    return String.Empty;
                case ItemStatus.OnSale:
                    return "Свободен";
                case ItemStatus.Locked:
                    return "В обработке";
                case ItemStatus.Reserved:
                    return "Зарезервирован";
                case ItemStatus.Sold:
                    return "Оплачен";
                case ItemStatus.Removed:
                    return "Списан";
                case ItemStatus.Returned:
                    return "Возвращен";
                case ItemStatus.Cancelled:
                    return "Аннулирован";
                case ItemStatus.OnRetail:
                    return "На реализации";
                case ItemStatus.OnStock:
                    return "На складе";
                case ItemStatus.Deleted:
                    return "Удален";
                case ItemStatus.Printed:
                    return "Распечатан";
            }
            return string.Empty;
        }

        public static string GetMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "Январь";
                case 2:
                    return "Февраль";
                case 3:
                    return "Март";
                case 4:
                    return "Апрель";
                case 5:
                    return "Май";
                case 6:
                    return "Июнь";
                case 7:
                    return "Июль";
                case 8:
                    return "Август";
                case 9:
                    return "Сентябрь";
                case 10:
                    return "Октябрь";
                case 11:
                    return "Ноябрь";
                case 12:
                    return "Декабрь";

            }
            return string.Empty;
        }

        public static string GetGenitiveMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "января";
                case 2:
                    return "февраля";
                case 3:
                    return "марта";
                case 4:
                    return "апреля";
                case 5:
                    return "мая";
                case 6:
                    return "июня";
                case 7:
                    return "июля";
                case 8:
                    return "августа";
                case 9:
                    return "сентября";
                case 10:
                    return "октября";
                case 11:
                    return "ноября";
                case 12:
                    return "декабря";

            }
            return string.Empty;
        }

        public static string GetShortMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "янв";
                case 2:
                    return "фев";
                case 3:
                    return "мар";
                case 4:
                    return "апр";
                case 5:
                    return "май";
                case 6:
                    return "июн";
                case 7:
                    return "июл";
                case 8:
                    return "авг";
                case 9:
                    return "сен";
                case 10:
                    return "окт";
                case 11:
                    return "ноя";
                case 12:
                    return "дек";

            }
            return string.Empty;
        }

        public static string GetWeekDay(DateTime dt)
        {
            DayOfWeek dow = dt.DayOfWeek;
            switch (dow)
            {
                case DayOfWeek.Monday:
                    return "Понедельник";
                case DayOfWeek.Tuesday:
                    return "Вторник";
                case DayOfWeek.Wednesday:
                    return "Среда";
                case DayOfWeek.Thursday:
                    return "Четверг";
                case DayOfWeek.Friday:
                    return "Пятница";
                case DayOfWeek.Saturday:
                    return "Суббота";
                case DayOfWeek.Sunday:
                    return "Воскресенье";

            }
            return string.Empty;
        }

        public static Color SetColor(decimal price, ItemStatus seatStatus)
        {
            Color color = Colors.Gainsboro;
            if (seatStatus == ItemStatus.Sold)
            {
                return color;
            }

            //color = System.Drawing.Color.Blue;

            //if (Colors == null)
            //{
            //    Colors = new Dictionary<decimal, Color>();
            //    Colors.Add(500, System.Drawing.Color.CornflowerBlue);
            //    Colors.Add(600, System.Drawing.Color.DodgerBlue);
            //    Colors.Add(700, System.Drawing.Color.Navy);
            //    Colors.Add(800, System.Drawing.Color.YellowGreen);
            //    Colors.Add(900, System.Drawing.Color.ForestGreen);
            //    Colors.Add(1000, System.Drawing.Color.Goldenrod);
            //    Colors.Add(1300, System.Drawing.Color.DarkOrange);
            //    Colors.Add(1200, System.Drawing.Color.Orchid);
            //    Colors.Add(1500, System.Drawing.Color.DodgerBlue);
            //    Colors.Add(1600, System.Drawing.Color.Crimson);
            //    Colors.Add(1700, System.Drawing.Color.Crimson);
            //    Colors.Add(1800, System.Drawing.Color.Crimson);
            //    Colors.Add(1900, System.Drawing.Color.Crimson);
            //    Colors.Add(2000, System.Drawing.Color.Crimson);
            //    Colors.Add(2100, System.Drawing.Color.Crimson);
            //    Colors.Add(2200, System.Drawing.Color.Crimson);
            //    Colors.Add(2300, System.Drawing.Color.Crimson);
            //    Colors.Add(2400, System.Drawing.Color.Crimson);
            //    Colors.Add(2500, System.Drawing.Color.Crimson);
            //    Colors.Add(2600, System.Drawing.Color.Crimson);
            //    Colors.Add(2700, System.Drawing.Color.Crimson);
            //    Colors.Add(2800, System.Drawing.Color.Crimson);
            //    Colors.Add(2900, System.Drawing.Color.Crimson);
            //    Colors.Add(3000, System.Drawing.Color.Crimson);
            //    Colors.Add(3100, System.Drawing.Color.Crimson);
            //    Colors.Add(3200, System.Drawing.Color.Crimson);
            //    Colors.Add(3300, System.Drawing.Color.Crimson);
            //    Colors.Add(3400, System.Drawing.Color.Crimson);
            //    Colors.Add(3500, System.Drawing.Color.Crimson);
            //}


            //try
            //{
            //    color = Colors[price];
            //}
            //catch { }

            //KnownColor color = System.Drawing.Color.DodgerBlue.ToKnownColor();
            if (price > 0 && price <= 500)
            {
                color = Colors.Blue;
            }

            if (price > 500 && price <= 1000)
            {
                color = Colors.ForestGreen;
            }

            if (price > 1000 && price <= 1500)
            {
                color = Colors.DarkOrange;
            }

            if (price > 1500 && price <= 2000)
            {
                color = Colors.Red;
            }

            if (price > 2000 && price <= 10000)
            {
                color = Colors.Brown;
            }

            return color;
        }
    }
}