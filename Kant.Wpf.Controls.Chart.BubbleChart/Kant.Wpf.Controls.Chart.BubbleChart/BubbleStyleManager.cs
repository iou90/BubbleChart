using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kant.Wpf.Controls.Chart
{
    public class BubbleStyleManager
    {
        #region Constructor

        public BubbleStyleManager(BubbleChart chart)
        {
            this.chart = chart;
        }

        #endregion

        #region Methods

        public void SetDefaultStyles()
        {
            chart.BubbleAnticipateMinRadius = 15;
            chart.BubbleGap = 2; // will setted in dp
        }

        #endregion

        #region Fields & Properties

        private BubbleChart chart;

        #endregion
    }
}
