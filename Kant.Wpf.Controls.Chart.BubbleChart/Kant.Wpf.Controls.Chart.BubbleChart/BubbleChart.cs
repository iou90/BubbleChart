﻿using Kant.Wpf.Toolkit;
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

namespace Kant.Wpf.Controls.Chart
{
    [TemplatePart(Name = "PartChartCanvas", Type = typeof(Canvas))]
    public class BubbleChart : Control, IDisposable
    {
        #region Constructor

        static BubbleChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BubbleChart), new FrameworkPropertyMetadata(typeof(BubbleChart)));
        }

        public BubbleChart()
        {
            disposedValue = false;
            styleManager = new BubbleStyleManager(this);
            styleManager.SetDefaultStyles();
            assist = new BubbleChartAssist(this, styleManager);
            SizeChanged += assist.ChartSizeChanged;

            Loaded += BubbleChartLoaded;
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var canvas = GetTemplateChild("PartChartCanvas") as Canvas;

            if (canvas == null)
            {
                throw new MissingMemberException("can not find template child PartDiagramCanvas.");
            }
            else
            {
                chartCanvas = canvas;
                assist.ChartCanvas = chartCanvas;
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (assist != null)
                    {
                        assist.ClearChart();
                    }
                }

                disposedValue = true;
            }
        }

        ~BubbleChart()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region dependency property methods

        private static void OnDatasSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleChart)o).assist.UpdateChart((IEnumerable<BubbleData>)e.NewValue);
        }

        private static void OnBubbleGapSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleChart)o).assist.CreateChart();
        }

        private static void OnBubbleAnticipateMinRadiusSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleChart)o).assist.CreateChart();
        }

        private static void OnBubbleBrushSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var chart = (BubbleChart)o;
            chart.styleManager.UpdateNodeBrushes((Brush)e.NewValue, chart.assist.CurrentNodes);
        }

        private static void OnBubbleBrushesSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var chart = (BubbleChart)o;
            chart.styleManager.UpdateNodeBrushes((Dictionary<string, Brush>)e.NewValue, chart.assist.CurrentNodes);
        }

        private static object HighlightNodeValueCallback(DependencyObject o, object value)
        {
            var chart = (BubbleChart)o;
            chart.styleManager.HighlightingNode((string)value, chart.assist.CurrentNodes);

            return value;
        }

        private static void OnHighlightModeSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleChart)o).SetCurrentValue(HighlightNodeProperty, null);
        }

        #endregion

        private void BubbleChartLoaded(object sender, RoutedEventArgs e)
        {
            assist.CreateChart();
            Loaded -= BubbleChartLoaded;
        }

        #endregion

        #region Fields & Properties

        #region dependency properties

        public IEnumerable<BubbleData> Datas
        {
            get { return (IEnumerable<BubbleData>)GetValue(DatasProperty); }
            set { SetValue(DatasProperty, value); }
        }

        public static readonly DependencyProperty DatasProperty = DependencyProperty.Register("Datas", typeof(IEnumerable<BubbleData>), typeof(BubbleChart), new PropertyMetadata(new List<BubbleData>(), OnDatasSourceChanged));

        //public double BubbleAnticipateMinRadius
        //{
        //    get { return (double)GetValue(BubbleAnticipateMinRadiusProperty); }
        //    set { SetValue(BubbleAnticipateMinRadiusProperty, value); }
        //}

        //public static readonly DependencyProperty BubbleAnticipateMinRadiusProperty = DependencyProperty.Register("BubbleAnticipateMinRadius", typeof(double), typeof(BubbleChart), new PropertyMetadata(15.0, OnBubbleAnticipateMinRadiusSourceChanged));

        public double BubbleGap
        {
            get { return (double)GetValue(BubbleGapProperty); }
            set { SetValue(BubbleGapProperty, value); }
        }

        public static readonly DependencyProperty BubbleGapProperty = DependencyProperty.Register("BubbleGap", typeof(double), typeof(BubbleChart), new PropertyMetadata(5.0, OnBubbleGapSourceChanged));

        /// <summary>
        /// universal node brush,
        /// will not work if BubbleBrushes is setted
        /// </summary>
        public Brush BubbleBrush
        {
            get { return (Brush)GetValue(BubbleBrushProperty); }
            set { SetValue(BubbleBrushProperty, value); }
        }

        public static readonly DependencyProperty BubbleBrushProperty = DependencyProperty.Register("BubbleBrush", typeof(Brush), typeof(BubbleChart), new PropertyMetadata(OnBubbleBrushSourceChanged));

        public Dictionary<string, Brush> BubbleBrushes
        {
            get { return (Dictionary<string, Brush>)GetValue(BubbleBrushesProperty); }
            set { SetValue(BubbleBrushesProperty, value); }
        }

        public static readonly DependencyProperty BubbleBrushesProperty = DependencyProperty.Register("BubbleBrushes", typeof(Dictionary<string, Brush>), typeof(BubbleChart), new PropertyMetadata(OnBubbleBrushesSourceChanged));

        public string HighlightNode
        {
            get { return (string)GetValue(HighlightNodeProperty); }
            set { SetValue(HighlightNodeProperty, value); }
        }

        public static readonly DependencyProperty HighlightNodeProperty = DependencyProperty.Register("HighlightNode", typeof(string), typeof(BubbleChart), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, HighlightNodeValueCallback));

        /// <summary>
        /// MouseLeftButtonUp by default
        /// </summary>
        public HighlightMode HighlightMode
        {
            get { return (HighlightMode)GetValue(HighlightModeProperty); }
            set { SetValue(HighlightModeProperty, value); }
        }

        public static readonly DependencyProperty HighlightModeProperty = DependencyProperty.Register("HighlightMode", typeof(HighlightMode), typeof(BubbleChart), new PropertyMetadata(HighlightMode.MouseLeftButtonUp, OnHighlightModeSourceChanged));

        #endregion

        #region initial settings

        public DataTemplate BubbleLabelTemplate { get; set; }

        public Brush HighlightBrush { get; set; }

        /// <summary>
        /// apply to nodes, links
        /// it does not work if you have already setted HighlightBrush property
        /// 1.0 by default
        /// </summary>
        public double HighlightOpacity { get; set; }

        /// <summary>
        /// apply to nodes, links
        /// 0.25 by default
        /// </summary>
        public double LoweredOpacity { get; set; }

        #endregion

        private BubbleStyleManager styleManager;

        private BubbleChartAssist assist;

        private Canvas chartCanvas;

        private bool disposedValue;

        #endregion
    }
}
