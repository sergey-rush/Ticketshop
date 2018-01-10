using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TicketShop.Map;
using TicketShop.Shell.Models;
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Views
{
    /// <summary>
    /// Interaction logic for OrdersView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        private MainViewModel _mainViewModel;
        public MapView()
        {
            _mainViewModel = MainViewModel.Instance;
            DataContext = _mainViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the ContentListBox that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        /// <summary>
        /// Saves the previous zoom ellipse, pressing the backspace key jumps back to this zoom ellipse.
        /// </summary>
        private Rect prevZoomRect;

        /// <summary>
        /// Save the previous ContentListBox scale, pressing the backspace key jumps back to this scale.
        /// </summary>
        private double prevZoomScale;

        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        private bool prevZoomRectSet = false;

        /// <summary>
        /// Event raised when the Window has loaded.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ZoomAndPanControl.AnimatedScaleToFit();
        }

        #region Zoom Events

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ContentListBox.Focus();
            Keyboard.Focus(ContentListBox);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(ZoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(ContentListBox);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                ZoomAndPanControl.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the ContentListBox.
                        ZoomIn(origContentMouseDownPoint);
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the ContentListBox.
                        ZoomOut(origContentMouseDownPoint);
                    }
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    // When drag-zooming has finished we zoom in on the ellipse that was highlighted by the user.
                    ApplyDragZoomRect();
                }

                ZoomAndPanControl.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(ContentListBox);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                ZoomAndPanControl.ContentOffsetX -= dragOffset.X;
                ZoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                Point curZoomAndPanControlMousePoint = e.GetPosition(ZoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                     Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a ellipse to select the area
                    // to zoom in on.
                    //
                    mouseHandlingMode = MouseHandlingMode.DragZooming;
                    Point curContentMousePoint = e.GetPosition(ContentListBox);
                    InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                //
                // When in drag zooming mode continously update the position of the ellipse
                // that the user is dragging out.
                //
                Point curContentMousePoint = e.GetPosition(ContentListBox);
                SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(ContentListBox);
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(ContentListBox);
                ZoomOut(curContentMousePoint);
            }
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomIn(new Point(ZoomAndPanControl.ContentZoomFocusX, ZoomAndPanControl.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut(new Point(ZoomAndPanControl.ContentZoomFocusX, ZoomAndPanControl.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'JumpBackToPrevZoom' command was executed.
        /// </summary>
        private void JumpBackToPrevZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JumpBackToPrevZoom();
        }

        /// <summary>
        /// Determines whether the 'JumpBackToPrevZoom' command can be executed.
        /// </summary>
        private void JumpBackToPrevZoom_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = prevZoomRectSet;
        }

        /// <summary>
        /// The 'Fill' command was executed.
        /// </summary>
        private void Fill_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePrevZoomRect();

            ZoomAndPanControl.AnimatedScaleToFit();
        }

        /// <summary>
        /// The 'OneHundredPercent' command was executed.
        /// </summary>
        private void OneHundredPercent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePrevZoomRect();

            ZoomAndPanControl.AnimatedZoomTo(1.0);
        }

        /// <summary>
        /// Jump back to the previous zoom level.
        /// </summary>
        private void JumpBackToPrevZoom()
        {
            ZoomAndPanControl.AnimatedZoomTo(prevZoomScale, prevZoomRect);

            ClearPrevZoomRect();
        }

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in ContentListBox coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            ZoomAndPanControl.ZoomAboutPoint(ZoomAndPanControl.ContentScale - 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in ContentListBox coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            ZoomAndPanControl.ZoomAboutPoint(ZoomAndPanControl.ContentScale + 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Initialise the ellipse that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            SetDragZoomRect(pt1, pt2);

            dragZoomCanvas.Visibility = Visibility.Visible;
            dragZoomBorder.Opacity = 0.5;
        }

        /// <summary>
        /// Update the position and size of the ellipse that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the ellipse that is being dragged out by the user.
            // The we offset and rescale to convert from ContentListBox coordinates.
            //
            Canvas.SetLeft(dragZoomBorder, x);
            Canvas.SetTop(dragZoomBorder, y);
            dragZoomBorder.Width = width;
            dragZoomBorder.Height = height;
        }

        /// <summary>
        /// When the user has finished dragging out the ellipse the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect()
        {
            //
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            //
            SavePrevZoomRect();

            //
            // Retreive the ellipse that the user draggged out and zoom in on it.
            //
            double contentX = Canvas.GetLeft(dragZoomBorder);
            double contentY = Canvas.GetTop(dragZoomBorder);
            double contentWidth = dragZoomBorder.Width;
            double contentHeight = dragZoomBorder.Height;
            ZoomAndPanControl.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

            FadeOutDragZoomRect();
        }

        //
        // Fade out the drag zoom ellipse.
        //
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(dragZoomBorder, Border.OpacityProperty, 0.0, 0.1,
                delegate(object sender, EventArgs e)
                {
                    dragZoomCanvas.Visibility = Visibility.Collapsed;
                });
        }

        //
        // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
        //
        private void SavePrevZoomRect()
        {
            prevZoomRect = new Rect(ZoomAndPanControl.ContentOffsetX, ZoomAndPanControl.ContentOffsetY,
                ZoomAndPanControl.ContentViewportWidth, ZoomAndPanControl.ContentViewportHeight);
            prevZoomScale = ZoomAndPanControl.ContentScale;
            prevZoomRectSet = true;
        }

        /// <summary>
        /// Clear the memory of the previous zoom level.
        /// </summary>
        private void ClearPrevZoomRect()
        {
            prevZoomRectSet = false;
        }

        /// <summary>
        /// Event raised when the user has double clicked in the zoom and pan control.
        /// </summary>
        private void zoomAndPanControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            //{
            //    Point doubleClickPoint = e.GetPosition(ContentListBox);
            //    ZoomAndPanControl.AnimatedSnapTo(doubleClickPoint);
            //}
        }

        #endregion

        #region Ellipse Events


        /// <summary>
        /// Event raised when a mouse button is clicked down over a Ellipse.
        /// </summary>
        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseButtonDown = e.ChangedButton;
            if (mouseButtonDown == MouseButton.Left)
            {
                ContentListBox.Focus();
                Keyboard.Focus(ContentListBox);
                Grid grid = (Grid) sender;
                var ellipse = grid.Children.Cast<Ellipse>().FirstOrDefault();
                Spot spot = (Spot) ellipse.DataContext;

                if (spot.IsSelected)
                {
                    spot.Width = 20;
                    spot.Height = 20;
                    spot.IsSelected = false;
                    _mainViewModel.SelectedSpots.Remove(spot);
                }
                else
                {
                    spot.IsSelected = true;
                    spot.Width = 26;
                    spot.Height = 26;
                    _mainViewModel.SelectedSpots.Add(spot);
                }
            }
            e.Handled = true;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                // When the shift key is held down special zooming logic is executed in content_MouseDown,
                // so don't handle mouse input here.
                return;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // We are in some other mouse handling mode, don't do anything.
                return;
            }

            //mouseHandlingMode = MouseHandlingMode.DraggingEllipse;
            //origContentMouseDownPoint = e.GetPosition(ContentListBox);
            //ellipse.CaptureMouse();

        }

        /// <summary>
        /// Event raised when a mouse button is released over a Ellipse.
        /// </summary>
        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.DraggingEllipse)
            {
                //
                // We are not in ellipse dragging mode.
                //
                return;
            }

            mouseHandlingMode = MouseHandlingMode.None;

            Ellipse ellipse = (Ellipse)sender;
            ellipse.ReleaseMouseCapture();
            e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Ellipse.
        /// </summary>
        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.DraggingEllipse)
            {
                //
                // We are not in ellipse dragging mode, so don't do anything.
                //
                return;
            }

            Point curContentPoint = e.GetPosition(ContentListBox);
            Vector ellipseDragVector = curContentPoint - origContentMouseDownPoint;
            //Debug.WriteLine("X:{0} Y:{1}", curContentPoint.X, curContentPoint.Y);
            //
            // When in 'dragging ellipses' mode update the position of the ellipse as the user drags it.
            //

            origContentMouseDownPoint = curContentPoint;

            Ellipse ellipse = (Ellipse)sender;
            Spot myEllipse = (Spot)ellipse.DataContext;
            myEllipse.X += ellipseDragVector.X;
            myEllipse.Y += ellipseDragVector.Y;

            e.Handled = true;
        }

        private void Ellipse_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = (Grid)sender;
            Ellipse ellipse = grid.Children.Cast<Ellipse>().FirstOrDefault();
            Spot spot = (Spot)ellipse.DataContext;

            if (!spot.IsSelected)
            {
                spot.Width = 26;
                spot.Height = 26;
                ellipse.Stroke = new SolidColorBrush(Colors.White);
                
                int index =_mainViewModel.Spots.IndexOf(spot);
                _mainViewModel.Spots.Move(index, _mainViewModel.Spots.Count - 1);

                if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                {
                    spot.IsSelected = true;
                    _mainViewModel.SelectedSpots.Add(spot);
                    // Do not remove: hack to update property
                    var df = _mainViewModel.SelectedSpots;
                }
            }
        }

        private void Ellipse_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = (Grid)sender;
            var ellipse = grid.Children.Cast<Ellipse>().FirstOrDefault();
            Spot spot = ellipse.DataContext as Spot;

            if (spot != null)
            {
                if (spot.IsSelected)
                {
                    //Debug.WriteLine("Ellipse_OnMouseLeave");
                    //spot.Width = 26;
                    //spot.Height = 26;
                    //spot.IsSelected = false;
                    
                }
                else
                {
                    ellipse.Stroke = ellipse.Fill;
                    spot.Width = 20;
                    spot.Height = 20;
                }
            }
        }


        #endregion

        private void ContentListBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("MouseDoubleClick");
            //Point curContentPoint = e.GetPosition(ContentListBox);
            //Debug.WriteLine("X:{0} Y:{1}", curContentPoint.X, curContentPoint.Y);
        }
    }
}
