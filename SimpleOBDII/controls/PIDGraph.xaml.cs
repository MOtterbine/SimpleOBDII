using OS.OBDII.Models;
using OS.OBDII.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OS.OBDII.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PIDGraph : ContentView
    {

        public PIDGraph()
        {
            InitializeComponent();
        }


        public String ValueText
        {
            get => (String)GetValue(ValueTextProperty);
            set => SetValue(ValueTextProperty, value);
        }
        public static readonly BindableProperty ValueTextProperty =
            BindableProperty.Create("ValueText", typeof(String), typeof(PIDGraph), string.Empty);

        public String Title
        {
            get => (String)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create("Title", typeof(String), typeof(PIDGraph), string.Empty);

        public DataPlotModel Model
        {
            get => (DataPlotModel)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }
        public static readonly BindableProperty ModelProperty =
            BindableProperty.Create("Model", typeof(DataPlotModel), typeof(PIDGraph), null);

    }
}