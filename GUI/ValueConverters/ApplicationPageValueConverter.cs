using System;
using System.Diagnostics;
using System.Globalization;

namespace GUI
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            // Find the appropriate page
            switch ((ApplicationPage)value[0])
            {
                case ApplicationPage.ConstructMethodPage:
                    if (value[1] is not ApplicationViewModel applicationViewModel)
                        return new MethodConstructionPageView(new MethodConstructionPageViewModel());
                    else
                        return new MethodConstructionPageView((applicationViewModel).MethodConstructionPageViewModel);

                case ApplicationPage.AboutPage:
                    return new AboutPageView();

                case ApplicationPage.WelcomePage:
                    return new WelcomePageView();

                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
