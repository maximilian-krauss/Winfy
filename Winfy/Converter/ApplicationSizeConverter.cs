using System;
using System.Globalization;
using System.Windows.Data;
using Winfy.Core;

namespace Winfy.Converter {
	public sealed class ApplicationSizeConverter : IValueConverter {
		private const int LargeApplicationSizeInPixel = 300;
		private const int MediumApplicationSizeInPixel = 174;
		private const int SmallApplicationSizeInPixel = 100;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is ApplicationSize)) return MediumApplicationSizeInPixel;
			
			var appSize = (ApplicationSize) value;
			switch (appSize) {
				case ApplicationSize.Large:
					return LargeApplicationSizeInPixel;
				case ApplicationSize.Medium:
					return MediumApplicationSizeInPixel;
				case ApplicationSize.Small:
					return SmallApplicationSizeInPixel;
			}
			return MediumApplicationSizeInPixel;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
}
