using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Imsat.Bashmaky.Model
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T enumValue)
        {
            if (!typeof(T).IsEnum)
                return null;


            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }
    }
}
