namespace FrameWork
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class CommonExtensions
    {
        public static bool? TryBoolean(this object o, string trueVal = "1", string falseVal = "0", bool? nullVal = new bool?())
        {
            if (o != null)
            {
                bool flag;
                string str = o.ToString();
                if (bool.TryParse(str, out flag))
                {
                    return new bool?(flag);
                }
                if (trueVal == str)
                {
                    return true;
                }
                if (falseVal == str)
                {
                    return false;
                }
            }
            return nullVal;
        }

        public static DateTime? TryDateTime(this object o, DateTime? nullValue = new DateTime?())
        {
            DateTime time;
            if ((o != null) && DateTime.TryParse(o.ToString(), out time))
            {
                return new DateTime?(time);
            }
            return nullValue;
        }

        public static decimal? TryDecimal(this object o, decimal? nullVal = new decimal?())
        {
            decimal num;
            if ((o != null) && decimal.TryParse(o.ToString(), out num))
            {
                return new decimal?(num);
            }
            return nullVal;
        }

        public static Guid? TryGuid(this object o, Guid? nullVal = new Guid?())
        {
            Guid guid;
            if ((o != null) && Guid.TryParse(o.ToString(), out guid))
            {
                return new Guid?(guid);
            }
            return nullVal;
        }

        public static int? TryInt(this object o, int? nullVal = new int?())
        {
            int num;
            if ((o != null) && int.TryParse(o.ToString(), out num))
            {
                return new int?(num);
            }
            return nullVal;
        }

        public static long? TryLong(this object o, long? nullVal = new long?())
        {
            long num;
            if ((o != null) && long.TryParse(o.ToString(), out num))
            {
                return new long?(num);
            }
            return nullVal;
        }

        public static string TryString(this object o, string nullValue = "")
        {
            if (o == null)
            {
                return nullValue;
            }
            return o.ToString();
        }

        public static string TryString(this object o, Func<string> fn, string nullValue = "")
        {
            if (o == null)
            {
                return nullValue;
            }
            return fn();
        }

        public static string TryTrimString(this object o, string nullValue = "")
        {
            if (o == null)
            {
                return nullValue;
            }
            return o.ToString().Trim();
        }
    }
}

