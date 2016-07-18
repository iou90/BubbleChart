using Kant.Wpf.MvvmFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Kant.Wpf.Controls.Chart.Example
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            random = new Random();
            bubbleColor = (Brush)Application.Current.FindResource("BubbleColor");
            bubbleLabelStyle1 = (Style)Application.Current.FindResource("BubbleLabelStyle1");
            bubbleLabelStyle2 = (Style)Application.Current.FindResource("BubbleLabelStyle2");
            BubbleLabelStyle = bubbleLabelStyle2;
            Label = "hey";
            Diameter = 55;

            // random datas
            var datas = new List<BubbleData>();
            var count = 7;

            for(var index = 0; index < count; index++)
            {
                datas.Add(new BubbleData()
                {
                    Name = "word" + index.ToString(),
                    Weight = random.Next(5, 55555),
                    Color = bubbleColor
                });
            }

            Datas = datas;
        }

        #endregion

        #region Commands

        private ICommand changeDatas;
        public ICommand ChangeDatas 
        {
            get
            {
                return GetCommand(changeDatas, new RelayCommand<object>(o =>
                {
                    var datas = new List<BubbleData>();
                    var count = random.Next(15, 55);

                    for (var index = 0; index < count; index++)
                    {
                        datas.Add(new BubbleData()
                        {
                            Name = "word" + index.ToString(),
                            Weight = random.Next(5, 55555),
                            Color = bubbleColor
                        });
                    }

                    Datas = datas;
                }));
            }
        }

        private ICommand clearDiagram;
        public ICommand ClearDiagram
        {
            get
            {
                return GetCommand(clearDiagram, new RelayCommand(() =>
                {
                }));
            }
        }

        private ICommand clearHighlight;
        public ICommand ClearHighlight
        {
            get
            {
                return GetCommand(clearHighlight, new RelayCommand(() =>
                {
                }));
            }
        }

        private ICommand highlightingNode;
        public ICommand HighlightingNode
        {
            get
            {
                return GetCommand(highlightingNode, new RelayCommand(() =>
                {
                }));
            }
        }

        private ICommand highlightingLink;
        public ICommand HighlightingLink
        {
            get
            {
                return GetCommand(highlightingLink, new RelayCommand(() =>
                {
                }));
            }
        }

        private ICommand changeStyles;
        public ICommand ChangeStyles
        {
            get
            {
                return GetCommand(changeStyles, new RelayCommand(() =>
                {
                }));
            }
        }

        #endregion

        #region Fields & Properties

        private List<BubbleData> datas;
        public List<BubbleData> Datas
        {
            get
            {
                return datas;
            }
            set
            {
                datas = value;
                RaisePropertyChanged(() => Datas);
            }
        }

        #region bubble test

        private double diameter;
        public double Diameter
        {
            get
            {
                return diameter;
            }
            set
            {
                diameter = value;
                RaisePropertyChanged(() => Diameter);
            }
        }

        private string label;
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
                RaisePropertyChanged(() => Label);
            }
        }

        #endregion

        private Style bubbleLabelStyle;
        public Style BubbleLabelStyle
        {
            get
            {
                return bubbleLabelStyle;
            }
            set
            {
                if (value != bubbleLabelStyle)
                {
                    bubbleLabelStyle = value;
                    RaisePropertyChanged(() => BubbleLabelStyle);
                }
            }
        }

        private Style bubbleLabelStyle1;

        private Style bubbleLabelStyle2;

        private Brush bubbleColor;

        private Random random;

        #endregion
    }
}
