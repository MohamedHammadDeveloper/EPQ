using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPQ.Infrastructure
{
    public static class ConveringClass
    {
        public static int ToInt32(this string str)
        {
            int Result = 0;
            int.TryParse(str, out Result);
            return Result;
        }

        public static long ToLong(this string str)
        {
            long Result = 0;
            long.TryParse(str, out Result);
            return Result;
        }
        public static long ToLong(this object obj)
        {
            long Result = 0;
            try
            {
                if (obj != null) Result = Convert.ToInt64(obj);
            }
            catch (Exception)
            {
            }
            return Result;
        }
        public static int ToInt32(this object obj)
        {
            int Result = 0;
            try
            {
                if (obj != null) Result = Convert.ToInt32(obj);
            }
            catch (Exception)
            {
            }
            return Result;
        }
        public static bool ToBoolean(this object obj)
        {
            bool Result = false;
            try
            {
                if (obj != null) Result = Convert.ToBoolean(obj);

            }
            catch (Exception)
            {
            }
            return Result;
        }
        public static DateTime ToDateTime(this object obj)
        {
            DateTime Result = DateTime.Now;
            try
            {
                if (obj != null) Result = Convert.ToDateTime(obj);

            }
            catch (Exception)
            {
            }
            return Result;
        }

        public static short ToShortInt(this object obj)
        {
            short Result = 0;
            if (obj != null) Result = Convert.ToInt16(obj);
            return Result;
        }

        public static byte ToByte(this object obj)
        {
            byte Result = 0;
            try
            {
                if (obj != null) Result = Convert.ToByte(obj);

            }
            catch (Exception)
            {

            }
            return Result;
        }

        public static decimal ToDecimal(this object obj)
        {
            decimal Result = 0;
            try
            {
                if (obj != null) Result = Convert.ToDecimal(obj);

            }
            catch (Exception)
            {

            }
            return Result;
        }

        public static double ToDouble(this object obj)
        {
            double Result = 0;
            try
            {
                if (obj != null) Result = Convert.ToDouble(obj);

            }
            catch (Exception)
            {

            }
            return Result;
        }
        public static float ToFloat(this object obj)
        {
            float Result = 0;
            try
            {
                if (obj != null) Result = (float)obj;

            }
            catch (Exception)
            {

            }
            return Result;
        }

        public static string ToString(this object obj)
        {
            string Result = "";
            try
            {
                if (obj != null) Result = Convert.ToString(obj);

            }
            catch (Exception)
            {

            }
            return Result;
        }
    }
}
