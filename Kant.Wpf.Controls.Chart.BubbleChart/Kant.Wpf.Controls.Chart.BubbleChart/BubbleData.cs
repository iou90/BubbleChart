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
		
		private double weight;
        public double Weight 
		{ 
			get
			{
				return weight;
			}
            set
			{
				weight = value;
				RaisePropertyChanged(() => Weight);
			} 
		}
		
		private Brush color;
        public Brush Color 
		{ 
			get
			{
				return color;
			}
			set
			{
				color = value;
				RaisePropertyChanged(() => Color);
			}
		}

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
