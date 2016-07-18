using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kant.Wpf.Controls
{
    public class Bubble : ContentControl
    {
        static Bubble()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Bubble), new FrameworkPropertyMetadata(typeof(Bubble)));
        }

        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register("Diameter", typeof(double), typeof(Bubble), new PropertyMetadata(25.0));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(Bubble), new PropertyMetadata(new RadialGradientBrush(new GradientStopCollection()
        {
            new GradientStop((Color)ColorConverter.ConvertFromString("#88bdc0ba"), 0),
            new GradientStop((Color)ColorConverter.ConvertFromString("#FFbdc0ba"), 0.5),
            new GradientStop((Color)ColorConverter.ConvertFromString("#AAbdc0ba"), 1),
        })
        { GradientOrigin = new Point(0.25, 0.25) }));
    }
}
