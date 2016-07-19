using Kant.Wpf.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Kant.Wpf.Controls.Chart
{
    public class BubbleData : ObservableObject
    {
        public BubbleData()
        {
        }

        public double Weight { get; set; }

        public Brush Color { get; set; }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private Dictionary<string, Size> labelSizes;
        public Dictionary<string, Size> LabelSizes
        {
            get
            {
                return labelSizes;
            }
            set
            {
                labelSizes = value;
                RaisePropertyChanged(() => LabelSizes);
            }
        }

        private ReadOnlyDictionary<string, double> labelScales;
        public ReadOnlyDictionary<string, double> LabelScales
        {
            get
            {
                return labelScales;
            }
            set
            {
                labelScales = value;
                RaisePropertyChanged(() => LabelScales);
            }
        }
    }
}
