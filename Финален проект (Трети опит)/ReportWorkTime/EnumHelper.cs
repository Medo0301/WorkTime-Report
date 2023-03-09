using ReportWorkTime.Models;

namespace ReportWorkTime
{
    public static class EnumHelper
    {
        public static readonly Dictionary<TypeOfDay, string> TypeOfDayDisplayNames = new Dictionary<TypeOfDay, string>
    {
        { TypeOfDay.WorkingDay, "Работен ден" },
        { TypeOfDay.BusinessTrip, "Командировка" },
        { TypeOfDay.PaidLeave, "Платен отпуск" },
        { TypeOfDay.UnpaidLeave, "Неплатен отпуск" },
        { TypeOfDay.SickLeave, "Болничен" }
    };
    }
}
