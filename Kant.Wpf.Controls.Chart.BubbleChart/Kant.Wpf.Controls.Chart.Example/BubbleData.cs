using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kant.Wpf.Controls.Chart.Example
{
    public class BubbleData : Kant.Wpf.Controls.Chart.BubbleData
    {
        private string catalog;
        public string Catalog
        {
            get
            {
                return catalog;
            }
            set
            {
                catalog = value;
                RaisePropertyChanged(() => Catalog);
            }
        }
    }
}
