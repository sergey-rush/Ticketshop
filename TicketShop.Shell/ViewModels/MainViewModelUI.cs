namespace TicketShop.Shell.ViewModels
{
    public partial class MainViewModel
    {
        #region UI

        ///
        /// The current scale at which the content is being viewed.
        /// 
        public double ContentScale
        {
            get
            {
                return contentScale;
            }
            set
            {
                contentScale = value;
                OnPropertyChanged("ContentScale");
            }
        }

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetX
        {
            get
            {
                return contentOffsetX;
            }
            set
            {
                contentOffsetX = value;
                OnPropertyChanged("ContentOffsetX");
            }
        }

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetY
        {
            get
            {
                return contentOffsetY;
            }
            set
            {
                contentOffsetY = value;
                OnPropertyChanged("ContentOffsetY");
            }
        }

        ///
        /// The width of the content (in content coordinates).
        /// 
        public double ContentWidth
        {
            get
            {
                return contentWidth;
            }
            set
            {
                contentWidth = value;
                OnPropertyChanged("ContentWidth");
            }
        }

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        public double ContentHeight
        {
            get
            {
                return contentHeight;
            }
            set
            {
                contentHeight = value;
                OnPropertyChanged("ContentHeight");
            }
        }

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportWidth
        {
            get
            {
                return contentViewportWidth;
            }
            set
            {
                contentViewportWidth = value;
                OnPropertyChanged("ContentViewportWidth");
            }
        }

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportHeight
        {
            get
            {
                return contentViewportHeight;
            }
            set
            {
                contentViewportHeight = value;
                OnPropertyChanged("ContentViewportHeight");
            }
        }

        #endregion

        #region ZoomAndPan

        ///
        /// The current scale at which the content is being viewed.
        /// 
        private double contentScale = 1;

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetX = 0;

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetY = 0;

        ///
        /// The width of the content (in content coordinates).
        /// 
        private double contentWidth = 2000;

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        private double contentHeight = 2000;

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        private double contentViewportWidth = 0;

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        private double contentViewportHeight = 0;

        #endregion ZoomAndPan
    }
}