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

            Loaded += (s, e) =>
            {
                if (IsChartCreated)
                {
                    return;
                }

                assist.CreateChart();
                IsChartCreated = true;
            };
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

        #endregion

        #endregion

        #region Fields & Properties

        #region dependency properties

        public IEnumerable<BubbleData> Datas
        {
            get { return (IEnumerable<BubbleData>)GetValue(DatasProperty); }
            set { SetValue(DatasProperty, value); }
        }

        public static readonly DependencyProperty DatasProperty = DependencyProperty.Register("Datas", typeof(IEnumerable<BubbleData>), typeof(BubbleChart), new PropertyMetadata(new List<BubbleData>(), OnDatasSourceChanged));

        #endregion

        #region initial settings

        /// <summary>
        /// will be dp
        /// </summary>
        public double BubbleGap { get; set; }

        public DataTemplate BubbleLabelTemplate { get; set; }

        public double BubbleAnticipateMinRadius { get; set; }

        #endregion

        public bool IsChartCreated { get; private set; }

        private BubbleStyleManager styleManager;

        private BubbleChartAssist assist;

        private Canvas chartCanvas;

        private bool disposedValue;

        #endregion
    }
}
