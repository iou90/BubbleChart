﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
            chart.HighlightOpacity = 1;
            chart.LoweredOpacity = 0.5;
        }

        public void UpdateNodeBrushes(Brush newBrush, IReadOnlyList<BubbleNode> nodes)
        {
            if(chart == null || nodes == null || nodes.Count == 0)
            {
                return;
            }

            chart.SetCurrentValue(BubbleChart.HighlightNodeProperty, null);

            foreach (var node in nodes)
            {
                node.Shape.Fill = newBrush.CloneCurrentValue();
                node.OriginalBrush = node.Shape.Fill.CloneCurrentValue();
            }
        }

        public void UpdateNodeBrushes(Dictionary<string, Brush> newBrushes, IReadOnlyList<BubbleNode> nodes)
        {
            if (chart == null || nodes == null || nodes.Count == 0)
            {
                return;
            }

            chart.SetCurrentValue(BubbleChart.HighlightNodeProperty, null);

            foreach (var node in nodes)
            {
                if(newBrushes.Keys.Contains(node.Name))
                {
                    node.Shape.Fill = newBrushes[node.Name].CloneCurrentValue();
                    node.OriginalBrush = node.Shape.Fill.CloneCurrentValue();
                }
            }
        }

        public void HighlightingNode(string highlightNode, IReadOnlyList<BubbleNode> nodes)
        {
            if ((string.IsNullOrEmpty(highlightNode) && string.IsNullOrEmpty(chart.HighlightNode) || nodes == null || nodes.Count == 0))
            {
                return;
            }

            // reset each element's brush first
            RecoverHighlight(nodes, false);

            // reset highlight if highlighting the same node twice
            if((from node in nodes where node.Name == chart.HighlightNode && node.IsHighlight select node).Count() == 1 && highlightNode == chart.HighlightNode)
            {
                RecoverHighlight(nodes);
                chart.SetCurrentValue(BubbleChart.HighlightNodeProperty, null);

                return;
            }

            if(string.IsNullOrEmpty(highlightNode) || !nodes.ToList().Exists(node => node.Name == highlightNode))
            {
                return;
            }

            foreach(var node in nodes)
            {
                if(node.Name == highlightNode)
                {
                    node.Shape.Fill.Opacity = chart.HighlightOpacity;
                    node.IsHighlight = true;

                    if(chart.HighlightBrush != null)
                    {
                        node.Shape.Fill = chart.HighlightBrush.CloneCurrentValue();
                    }
                }
                else
                {
                    var minimizeOpacity = node.Shape.Fill.Opacity - chart.LoweredOpacity < 0 ? 0 : node.Shape.Fill.Opacity - chart.LoweredOpacity;
                    node.Shape.Fill.Opacity = minimizeOpacity;
                    node.IsHighlight = false;
                }
            }
        }

        private void RecoverHighlight(IReadOnlyList<BubbleNode> nodes, bool resetHighlightStatus = true)
        {
            foreach(var node in nodes)
            {
                node.Shape.Fill = node.OriginalBrush.CloneCurrentValue();

                if (resetHighlightStatus)
                {
                    node.IsHighlight = false;
                }
            }
        }

        #endregion

        #region Fields & Properties

        private BubbleChart chart;

        #endregion
    }
}
