using System;
using System.Threading.Tasks;

namespace HouseService.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsBefore(this DateTime date, string target)
        {
            var targetTime = DateTime.Parse(target).TimeOfDay;
            return date.TimeOfDay < targetTime;
        }

        public static bool IsAfter(this DateTime date, string target)
        {
            var targetTime = DateTime.Parse(target).TimeOfDay;
            return date.TimeOfDay >= targetTime;
        }

        public static void Forget(this Task task)
        {
        }
    }
}
