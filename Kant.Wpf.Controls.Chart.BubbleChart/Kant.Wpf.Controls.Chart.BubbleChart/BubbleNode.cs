using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kant.Wpf.Controls.Chart
{
    public class BubbleNode : Element
    {
        public BubbleNode()
        {
            TangentBubbles = new List<BubbleNode>();
        }

        public int Index { get; set; }

        public List<BubbleNode> TangentBubbles { get; set; }

        /// <summary>
        /// x coordinate to canvas center
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// y coordinate to canvas center
        /// </summary>
        public double Y { get; set; }

        public double Radius { get; set; }

        public Bubble Shape { get; set; }

        public string Name { get; set; }
    }
}
