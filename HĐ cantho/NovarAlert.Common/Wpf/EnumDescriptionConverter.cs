using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using NovaAlert.Common.Utils;

namespace NovaAlert.Common.Wpf
{
    public class EnumDescriptionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            if (value is Enum)
            {
                return EnumEx.GetEnumDescription((Enum)value);
            }

            return null;            
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var list = EnumEx.EnumToListOfKeyValuePairs<int>(targetType);
                if (list.Any(it => it.Value == value.ToString()))
                {
                    return list.Where(it => it.Value == value.ToString()).First().Key;
                }
            }           

            return null;            
        }
    }
}
