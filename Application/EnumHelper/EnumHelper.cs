using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EnumHepler
{
    public static class EnumHelper
    {
        public static List<(int Value, string Name, string Description)> GetEnumList<T>() where T : Enum
        {
            var type = typeof(T);
            return Enum.GetValues(type)
                       .Cast<Enum>()
                       .Select(e => (
                           Convert.ToInt32(e),
                           e.ToString(),
                           GetDescription(e)
                       )).ToList();
        }

        private static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .Cast<DescriptionAttribute>()
                            .FirstOrDefault();
            return attr?.Description ?? value.ToString();
        }
    }
}
public class EnumDto
{
    public int Value { get; set; }
    public string Key { get; set; }
    public string Description { get; set; }
}
