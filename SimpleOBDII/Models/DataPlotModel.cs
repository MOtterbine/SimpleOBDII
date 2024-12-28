using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OS.OBDII.Models
{
    public class DataPlotModel : PlotModel, INotifyPropertyChanged, IDisposable
    {
        public uint Id { get; set; }

        #region INotifyPropertyChanged

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public void InvokeUpdate(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        private LineSeries _lineSeries = new LineSeries();
        public string PlotLabel
        {
            get => this.plotLabel;
            set => SetProperty(ref this.plotLabel, value);
        }
        private string plotLabel = string.Empty;

        public new string Title
        {
            get => this.title;
            set => SetProperty(ref this.title, value);
        }
        private string title = string.Empty;

        public double XAxisMaxValue
        {
            get { return this.Axes[0].Maximum; }
            set
            {
                this.Axes[0].Maximum = value;
            }
        }

        public double YAxisMaxValue
        {
            get { return this.Axes[1].Maximum; }
            set
            {
                this.Axes[1].Maximum = value;
            }
        }
        public double YAxisMinValue
        {
            get { return this.Axes[1].Minimum; }
            set
            {
                this.Axes[1].Minimum = value;
            }
        }
        public string YAxisTitle
        {
            get
            {
                return this.Axes[1].Title;
            }
            set
            {
                this.Axes[1].Title = value;
            }

        }

        private double tempMaxYValue = 0.0;
        private double tempMinYValue = 0.0;
        public void AddDataPoint(double xValue, double yValue)
        {
            this.lastXValue = xValue;
            this._datapoints.AddLast(new DataPoint(xValue, yValue));
            this.Points.Add(this._datapoints.Last());
           // this.Points.Add(new DataPoint(xValue, yValue));
            OnPropertyChanged("Points");
        }

        private double lastXValue = 0, xin, yin;
        public void AddDataPoint(double value)
        {
            
            if (double.IsNaN(value)) value = 0;

            if (double.IsNaN(value)) value = 0;
            if (value > YAxisMaxValue)
            {
                // adjusting for a margin above the plot
                YAxisMaxValue = 1.2 * value;
                this.Axes[1].Zoom(YAxisMinValue, YAxisMaxValue);
            }
            if (value < YAxisMinValue)
            {
                // adjusting for a margin below the plot
                if (value > -1)
                {
                    this.YAxisMinValue = .8 * value; // above zero - move to lower value
                }
                else
                {
                    this.YAxisMinValue = 1.2 * value; // below zero - also move to lower value
                }
                this.Axes[1].Zoom(YAxisMinValue, YAxisMaxValue);
            }

            // SHIFT THE CURVE LEFT...
            int i = 0;
            for (i = 0; i < this.Points.Count - 1; i++)
           // for (i = this.Points.Count - 2; i >= 0; i--)
            {
                this.Points[i] = new DataPoint(i, this.Points[i + 1].Y);
            }

            //this._datapoints.RemoveFirst();
            //this._datapoints.AddLast(new DataPoint(this.Points.Count - 1, value));

            //var p = _datapoints.ToList();
            //for (i = 0; i <= this.Points.Count - 1; i++)
            //{
            //    this.Points[i] = p[i];
            //    //this.Points[i] = new DataPoint(i, this.Points[i + 1].Y);
            //}
            //      this.Points[this.Points.Count - 1] =p[this.Points.Count-1];

            //var nArr  = Points.Take(this.Points.Count - 1).ToList();
            //nArr[this.Points.Count - 1] = new DataPoint(this.Points.Count - 1, value);
            //this._lineSeries.ItemsSource = nArr;

            //this.Points.Add(new DataPoint(this.Points.Count, value));

            this.Points[this.Points.Count - 1] = new DataPoint(this.Points.Count - 1, value);

            this.IsReset = false;

        }

        private LinkedList<DataPoint> _datapoints = new LinkedList<DataPoint>();

        private bool IsReset = false;
        public void ResetData()
        {
            if (this.IsReset) return;
            this.YAxisMinValue = -.000001;
            this.YAxisMaxValue = .000001;
            this.ZoomAllAxes(1);
            int i = 0;
            //this.Points.Clear();
            for (i = 0; i < Constants.PLOT_POINT_WIDTH; i++)
            {
                //var j = new DataPoint();

                //this.Points.Add(new DataPoint(i, 0));
                this.Points[i] = new DataPoint(i,0);
            }
            this.IsReset = true;
           // this.InvalidatePlot(true);
        }

        public void ResetVerticalRange(double minY = double.MinValue)
        {
            this.YAxisMaxValue = double.NaN;
            this.YAxisMinValue = minY;
            this.tempMaxYValue = 0.0;
            this.tempMinYValue = 0.0;
            this.ResetAllAxes();
            this.lastXValue = Constants.PLOT_POINT_WIDTH - 1;
        }
        public void ClearDataPoints()
        {
            
            this.Points.Clear();
           
        }

        public DataPlotModel(string title = null)
        {
            if (string.IsNullOrEmpty(title)) title = string.Empty; 
            this.Title = title;
            
            this.PlotType = PlotType.XY;
            this.TitleFontWeight = 200;
            this.TitleFontSize = 12;

            var foreColor = OxyPlot.OxyColor.FromArgb(0xCC, 0xFF, 0xFF, 0xFF);
            //this.TextColor = OxyPlot.OxyColor.FromArgb(0xDD, 0x00, 0x00, 0x00);
            this.TextColor = foreColor;
            this.PlotAreaBorderThickness = new OxyPlot.OxyThickness(1, 0, 1, 0);
            this.PlotMargins = new OxyPlot.OxyThickness(30, 14, 0, 0);
            this.Padding = new OxyPlot.OxyThickness(10);
            //this.Title = "Input Data";// $"Data: {plotYVal:F0}";
            this.TitleHorizontalAlignment = OxyPlot.TitleHorizontalAlignment.CenteredWithinPlotArea;
            

            this.TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinView;
            this.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, IsPanEnabled = false, IsZoomEnabled = false, AbsoluteMinimum = 0, Maximum = double.NaN, Key = "Horizontal", IsAxisVisible = false, Title = title });
            this.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, IsPanEnabled = false, IsZoomEnabled = true, TickStyle=TickStyle.Inside, /* Maximum = 5, */ Key = "Vertical",  });

            this.PlotAreaBorderColor = foreColor;

            this._lineSeries.Color = foreColor;
            this._lineSeries.StrokeThickness = 2;

            this._lineSeries.ItemsSource = new List<DataPoint>(Constants.PLOT_POINT_WIDTH+1);
            for (int i = 0; i < Constants.PLOT_POINT_WIDTH; i++)
            {
                this.AddDataPoint(i, 0);
            }
            this.Series.Add(this._lineSeries);
            this.lastXValue = Constants.PLOT_POINT_WIDTH - 1;
            //this.AddDataPoint(0, 0);

        }

        public IList<DataPoint> Points => this._lineSeries.ItemsSource as IList<DataPoint>;



        #region IDisposable

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.

        // Track whether Dispose has been called.
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        // Dispose(bool disposing) executes in two distinct scenarios.
        // 1) If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // 2) If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this.disposed == false)
            {
                // If disposing equals true managed resources can be disposed
                if (disposing == true)
                {
                    // Dispose managed resources.
                    this.ClearDataPoints();
                    this.Points.Clear();
                    this.Series.Remove(this._lineSeries);
                    this._lineSeries = null;
                    this.Axes.Clear();
                   

                    //	component.Dispose();
                }
                // Unmanaged resources are disposed in any case

                //	CloseHandle(handle);
                //	handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }
        ~DataPlotModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion


    }
}
