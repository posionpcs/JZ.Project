using System;

namespace FrameWork
{
    public static class DecimalExtensions
    {
        public static decimal Rounddown(this decimal number, int digits)
        {
            int num = 1;
            for (int i = 1; i <= digits; i++)
            {
                num *= 10;
            }
            return (Math.Truncate((decimal) (number * num)) / num);
        }
    }
}

