namespace TicketShop.Chart.Core.Doughnut
{
    #if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Core;
#else

#endif

    public class DoughnutChart : PieChart.PieChart
    {
        protected override double GridLinesMaxValue
        {
            get
            {
                return 0.0;
            }
        }
    }
}