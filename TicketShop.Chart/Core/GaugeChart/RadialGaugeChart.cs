﻿using System.Windows;

namespace TicketShop.Chart.Core.GaugeChart
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

   
    public class RadialGaugeChart : ChartBase
    {
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="PieChart"/> class.
        /// </summary>
        static RadialGaugeChart()
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT

#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialGaugeChart), new FrameworkPropertyMetadata(typeof(RadialGaugeChart)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PieChart"/> class.
        /// </summary>
        public RadialGaugeChart()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(RadialGaugeChart);
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(RadialGaugeChart);
#endif
        }

        #endregion Constructors

        protected override double GridLinesMaxValue
        {
            get
            {
                return 0.0;
            }
        }
    }
}