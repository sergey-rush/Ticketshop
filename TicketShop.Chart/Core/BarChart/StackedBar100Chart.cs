using System.Windows;
using TicketShop.Chart.Core.ColumnChart;

namespace TicketShop.Chart.Core.BarChart
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

    /// <summary>
    /// Represents an Instance of the bar-chart
    /// </summary>
    public class StackedBar100Chart : ChartBase
    {
        /// <summary>
        /// Initializes the <see cref="ClusteredColumnChart"/> class.
        /// </summary>
        static StackedBar100Chart()        
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT
    
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedBar100Chart), new FrameworkPropertyMetadata(typeof(StackedBar100Chart)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusteredColumnChart"/> class.
        /// </summary>
        public StackedBar100Chart()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(StackedBar100Chart);
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(StackedBar100Chart);
#endif
        }

        protected override double GridLinesMaxValue
        {
            get
            {
                return 100.0;
            }
        }
    }
}