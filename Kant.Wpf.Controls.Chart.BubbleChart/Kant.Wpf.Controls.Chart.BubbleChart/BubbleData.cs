using Kant.Wpf.MvvmFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
