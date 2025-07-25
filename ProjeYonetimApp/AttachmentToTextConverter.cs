using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjeYonetimApp
{
    public class AttachmentToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Boşsa “Yok”
            if (value == null || value == DBNull.Value)
                return "Yok";
            // Boş byte[] ise yine “Yok”
            if (value is byte[] b && b.Length == 0)
                return "Yok";
            // Diğer her durumda “Ekli”
            return "Ekli";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}