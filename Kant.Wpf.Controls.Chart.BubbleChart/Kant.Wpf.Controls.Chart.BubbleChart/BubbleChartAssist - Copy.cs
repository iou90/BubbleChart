using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kant.Wpf.Controls.Chart
{
    public class BubbleChartAssist
    {
        #region Constructor

        public BubbleChartAssist(BubbleChart chart, BubbleStyleManager styleManager)
        {
            this.chart = chart;
            this.styleManager = styleManager;
            CurrentNodes = new List<BubbleNode>();
        }

        #endregion

        #region Methods

        public void UpdateChart(IEnumerable<BubbleData> datas)
        {
            // clear diagram first
            ClearChart();

            if (datas == null || datas.Count() == 0)
            {
                return;
            }

            currentDatas = datas.ToList();

            // drawing...
            if (chart.IsChartCreated)
            {
                CreateChart();
            }
        }

        public void CreateChart()
        {
            if (currentDatas == null || chart.ActualHeight <= 0 || chart.ActualWidth <= 0)
            {
                return;
            }

            #region set bubble initial information

            var bubbleLargerCoefficient = 5;
            var canvasCenter = new Point(chart.ActualWidth / 2, chart.ActualHeight / 2);
            var canvasRadius = Math.Min(canvasCenter.X, canvasCenter.Y);
            var canvasArea = Math.PI * Math.Pow(canvasRadius, 2);
            var singleBubbleAverageArea = canvasArea / currentDatas.Count;
            var bubbleMaxRadius = Math.Sqrt(singleBubbleAverageArea / Math.PI);
            bubbleMaxRadius = bubbleMaxRadius / chart.BubbleAnticipateMinRadius < bubbleLargerCoefficient ? chart.BubbleAnticipateMinRadius * bubbleLargerCoefficient : bubbleMaxRadius;
            var maxWeight = currentDatas.Max(d => d.Weight);
            var minWeight = currentDatas.Min(d => d.Weight);
            var bubbleMinRaidus = maxWeight == minWeight ? bubbleMaxRadius : chart.BubbleAnticipateMinRadius;
            var temp = chart.BubbleAnticipateMinRadius * 2;
            var margin = bubbleMinRaidus < temp ? temp - bubbleMinRaidus : 0;

            #endregion

            // create bubble nodes
            foreach (var data in currentDatas)
            {
                var bubbleRadius = maxWeight == minWeight ? bubbleMaxRadius : (data.Weight - minWeight) * ((bubbleMaxRadius - chart.BubbleAnticipateMinRadius) / (maxWeight - minWeight)) + chart.BubbleAnticipateMinRadius;
                bubbleRadius += margin;
                CreateBubbleNode(data, CurrentNodes, canvasCenter, bubbleRadius, chart.BubbleGap);
                //CreateBubbleNode1(data, CurrentNodes, canvasCenter, bubbleRadius, chart.BubbleGap);
            }

            DrawBubbles(canvasCenter, CurrentNodes);
        }

        private void ClearChart()
        {
            if (!(ChartCanvas == null || ChartCanvas.Children == null || ChartCanvas.Children.Count == 0))
            {
                ChartCanvas.Children.Clear();
                CurrentNodes.Clear();
            }
        }

        private void CreateBubbleNode1(BubbleData data, List<BubbleNode> currentNodes, Point canvasCenter, double newBubbleRadius, double bubbleGap)
        {
            var newNode = new BubbleNode();
            newNode.Radius = newBubbleRadius;
            bool oneFlag = false;
            bool twoFlag = false;

            newNode.Shape = new Bubble()
            {
                DataContext = data,
                Diameter = newNode.Radius * 2,
                Content = chart.BubbleContent
            };


            // 设定第一个气泡信息（第一个气泡在画布中心点位置）
            if (currentNodes.Count == 0)
            {
                // 设定气泡中心点坐标
                newNode.X = canvasCenter.X;
                newNode.Y = canvasCenter.Y;

                currentNodes.Add(newNode);
            }
            else if (currentNodes.Count == 1)
            {
                // 设定第三个气泡信息（位置随机设定）
                //int r = random.Next(0, 359);
                int r = 45;
                // 设定气泡中心点坐标
                newNode.X = canvasCenter.X + Math.Sin(Math.PI / 180 * r) * (currentNodes[0].Radius + newNode.Radius + bubbleGap + 100);
                newNode.Y = canvasCenter.Y + Math.Cos(Math.PI / 180 * r) * (currentNodes[0].Radius + newNode.Radius + bubbleGap - 100);
                // 设定相切气泡
                newNode.TangentBubbles.Add(currentNodes[0]);

                currentNodes.Add(newNode);
            }
            else
            {
                //int r = random.Next(1, 100);
                int r = 70;
                //var findBubbleList = new List<BubbleNode>();

                //for (var index = currentNodes.Count - 2; index >= 0; index--)
                //{
                //    findBubbleList.Insert(0, currentNodes[index]);
                //}
                var findBubbleList = currentNodes;

                for (int i = 0; i < findBubbleList.Count - 1; i++)
                    {
                        // 求两个圆的交点处理逻辑
                        // 以第一个圆的中心为坐标中心（坐标转换利于计算，圆心为（0， 0）圆方程：x^2 + y^2 = r1^2）
                        // 第一个圆的半径（第一个相切气泡的半径 + 新气泡的半径 + 气泡间隔）
                        double r1 = currentNodes.LastOrDefault().Radius + newNode.Radius + bubbleGap;

                        // 第二个圆的中心相对第一个圆的坐标(圆方程：(x - x2)^2 + (y - y2)^2 = r2^2)
                        double x2 = findBubbleList[i].X - currentNodes.LastOrDefault().X;
                        double y2 = findBubbleList[i].Y - currentNodes.LastOrDefault().Y;
                        // 第二个圆的半径（第二个相切气泡的半径 + 新气泡的半径 + 气泡间隔）
                        double r2 = findBubbleList[i].Radius + newNode.Radius + bubbleGap;

                        // 两式合并
                        double a = 4 * (x2 * x2 + y2 * y2);
                        double b = -4 * y2 * (x2 * x2 + y2 * y2 + r1 * r1 - r2 * r2);
                        double c = (x2 * x2 + y2 * y2 + r1 * r1 - r2 * r2) * (x2 * x2 + y2 * y2 + r1 * r1 - r2 * r2) - 4 * x2 * x2 * r1 * r1;

                        // 有解（b2-4ac≥0）
                        if (b * b - 4 * a * c >= 0)
                        {
                            if (x2 == 0)
                            {
                                x2 = 0.00000000000001;
                            }

                            double yOne = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                            double xOne = (x2 * x2 + y2 * y2 + r1 * r1 - r2 * r2 - 2 * y2 * yOne) / (2 * x2);
                            double yTwo = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                            double xTwo = (x2 * x2 + y2 * y2 + r1 * r1 - r2 * r2 - 2 * y2 * yTwo) / (2 * x2);

                            //相对坐标转绝对坐标
                            Point pointOne = new Point(xOne + currentNodes.LastOrDefault().X, yOne + currentNodes.LastOrDefault().Y);
                            Point pointTwo = new Point(xTwo + currentNodes.LastOrDefault().X, yTwo + currentNodes.LastOrDefault().Y);

                            oneFlag = false;
                            twoFlag = false;

                            // 判断是否有重叠(与以存在的所有气泡进行判断)
                            for (int j = currentNodes.Count - 1; j >= 0; j--)
                            {
                                if (!oneFlag)
                                {
                                    // 两圆心的距离
                                    double distanceOne = Math.Sqrt((pointOne.X - currentNodes[j].X) * (pointOne.X - currentNodes[j].X) +
                                        (pointOne.Y - currentNodes[j].Y) * (pointOne.Y - currentNodes[j].Y));
                                    // 重叠
                                    if (currentNodes[j].Radius + newNode.Radius + bubbleGap - distanceOne > 0.1)
                                    {
                                        oneFlag = true;
                                    }
                                }

                                if (!twoFlag)
                                {
                                    // 两圆心的距离
                                    double distanceTwo = Math.Sqrt((pointTwo.X - currentNodes[j].X) * (pointTwo.X - currentNodes[j].X) +
                                        (pointTwo.Y - currentNodes[j].Y) * (pointTwo.Y - currentNodes[j].Y));
                                    // 重叠
                                    if (currentNodes[j].Radius + newNode.Radius + bubbleGap - distanceTwo > 0.1)
                                    {
                                        twoFlag = true;
                                    }
                                }
                            }

                            // 全没有重叠
                            if (!oneFlag && !twoFlag)
                            {
                                // 新气泡相对相切气泡的角度（位置1）
                                double angleOne = CalculateAngle(pointOne, new Point(findBubbleList[i].X, findBubbleList[i].Y));

                                // 新气泡相对相切气泡的角度（位置2）
                                double angleTwo = CalculateAngle(pointTwo, new Point(findBubbleList[i].X, findBubbleList[i].Y));

                                // 两相切气泡指针方向角度
                                double angle = CalculateAngle(new Point(currentNodes.LastOrDefault().X, currentNodes.LastOrDefault().Y),
                                    new Point(findBubbleList[i].X, findBubbleList[i].Y));

                                // 取顺时针/逆时针方向180度的圆点坐标
                                if (CheckAngleInRightHand(angle, Math.PI, angleOne, r > 50))
                                {
                                    // 设定坐标
                                    newNode.X = pointOne.X;
                                    newNode.Y = pointOne.Y;
                                }
                                else
                                {
                                    // 设定坐标
                                    newNode.X = pointTwo.X;
                                    newNode.Y = pointTwo.Y;
                                }

                                // 设定相切气泡
                                newNode.TangentBubbles.Add(currentNodes.LastOrDefault());
                                newNode.TangentBubbles.Add(findBubbleList[i]);

                                currentNodes.Add(newNode);
                                break;
                            }
                            else if (!oneFlag)
                            {
                                // 设定坐标
                                newNode.X = pointOne.X;
                                newNode.Y = pointOne.Y;
                                // 设定相切气泡
                                newNode.TangentBubbles.Add(currentNodes.LastOrDefault());
                                newNode.TangentBubbles.Add(findBubbleList[i]);

                                currentNodes.Add(newNode);
                                break;
                            }
                            else if (!twoFlag)
                            {
                                // 设定坐标
                                newNode.X = pointTwo.X;
                                newNode.Y = pointTwo.Y;
                                // 设定相切气泡
                                newNode.TangentBubbles.Add(currentNodes.LastOrDefault());
                                newNode.TangentBubbles.Add(findBubbleList[i]);

                                currentNodes.Add(newNode);
                                break;
                            }
                        }
                    }
            }
        }

        private void CreateBubbleNode(BubbleData data, List<BubbleNode> currentNodes, Point canvasCenter, double newBubbleRadius, double bubbleGap)
        {
            var newNode = new BubbleNode();
            newNode.Radius = newBubbleRadius;

            newNode.Shape = new Bubble()
            {
                DataContext = data,
                Diameter = newNode.Radius * 2,
                Content = chart.BubbleContent
            };

            // create first bubble
            if (currentNodes.Count == 0)
            {
                newNode.X = canvasCenter.X;
                newNode.Y = canvasCenter.Y;
                currentNodes.Add(newNode);

                return;
            }

            // create second bubble
            if (currentNodes.Count == 1)
            {
                // set second bubble initial angle to 45
                var rad = Math.PI / 180 * 45;

                // clockwiseControl x is 100, y is - 100
                var distanceBetweenCenters = currentNodes[0].Radius + newNode.Radius + bubbleGap;
                newNode.X = canvasCenter.X + Math.Sin(rad) * (distanceBetweenCenters + 100);
                newNode.Y = canvasCenter.Y + Math.Cos(rad) * (distanceBetweenCenters - 100);

                newNode.TangentBubbles.Add(currentNodes[0]);
                currentNodes.Add(newNode);

                return;
            }

            var initialAngle = 70;

            // find second tangent bubble from currrent nodes except the last of it)
            for (var index = 0; index < currentNodes.Count - 1; index++)
            {
                #region prepare for checking collisions

                var interativeNode = currentNodes[index];
                var lastNode = currentNodes.Last();
                var distanceBetweenCentersFromLastNodeToNewBubble = lastNode.Radius + newNode.Radius + bubbleGap;
                var distanceBetweenCentersFromInterativeNodeToNewBubble = interativeNode.Radius + newNode.Radius + bubbleGap;
                var interativeNodeXRelativeToLastNode = interativeNode.X - lastNode.X;
                var interativeNodeYRelativeToLastNode = interativeNode.Y - lastNode.Y;
                var squareOfX = Math.Pow(interativeNodeXRelativeToLastNode, 2);
                var squareOfY = Math.Pow(interativeNodeYRelativeToLastNode, 2);
                var squareOfD1 = Math.Pow(distanceBetweenCentersFromLastNodeToNewBubble, 2);
                var sumFromSquareOfXAndSquareOfY = squareOfX + squareOfY;
                var differFromSquareOfD1AndSquareOfD2 = squareOfD1 - Math.Pow(distanceBetweenCentersFromInterativeNodeToNewBubble, 2);
                var sumFromSumFromSquareOfXAndSquareOfYAndDifferFromSquareOfD1AndSquareOfD2 = sumFromSquareOfXAndSquareOfY + differFromSquareOfD1AndSquareOfD2;
                var a = 4 * sumFromSquareOfXAndSquareOfY;
                var b = -4 * interativeNodeYRelativeToLastNode * sumFromSumFromSquareOfXAndSquareOfYAndDifferFromSquareOfD1AndSquareOfD2;
                var c = Math.Pow(sumFromSumFromSquareOfXAndSquareOfYAndDifferFromSquareOfD1AndSquareOfD2, 2) - 4 * squareOfX * squareOfD1;
                var squareOfB = Math.Pow(b, 2);
                var aMultiplycMultiply4 = 4 * a * c;
                var solutionOfQuadraticEquation = squareOfB - aMultiplycMultiply4;
                var aMultiply2 = 2 * a;
                var xMultiply2 = 2 * interativeNodeXRelativeToLastNode;
                var yMultiply2 = 2 * interativeNodeYRelativeToLastNode;

                #endregion

                // quadratic equation has solution
                if (solutionOfQuadraticEquation >= 0)
                {
                    var y1 = (-b + Math.Sqrt(solutionOfQuadraticEquation)) / aMultiply2;
                    var x1 = (sumFromSumFromSquareOfXAndSquareOfYAndDifferFromSquareOfD1AndSquareOfD2 - yMultiply2 * y1) / xMultiply2;
                    var y2 = (-b - Math.Sqrt(solutionOfQuadraticEquation)) / aMultiply2;
                    var x2 = (sumFromSumFromSquareOfXAndSquareOfYAndDifferFromSquareOfD1AndSquareOfD2 - yMultiply2 * y2) / xMultiply2;
                    var point1 = new Point(x1 + lastNode.X, y1 + lastNode.Y);
                    var point2 = new Point(x2 + lastNode.X, y2 + lastNode.Y);
                    var collisionWithPoint1 = false;
                    var collisionWithPoint2 = false;

                    // check collisions
                    for (var checkCollitionIndex = currentNodes.Count - 1; checkCollitionIndex >= 0; checkCollitionIndex--)
                    {
                        var node = currentNodes[checkCollitionIndex];

                        if (!collisionWithPoint1)
                        {
                            collisionWithPoint1 = CheckCollision(point1, node, newNode.Radius, bubbleGap);
                        }

                        if (!collisionWithPoint2)
                        {
                            collisionWithPoint2 = CheckCollision(point2, node, newNode.Radius, bubbleGap);
                        }
                    }

                    // no collisions
                    if (!collisionWithPoint1 && !collisionWithPoint2)
                    {
                        var point = new Point(interativeNode.X, interativeNode.Y);
                        var angleFromIntertiveNodeToPoint1 = CalculateAngle(point1, point);
                        var newBubbleAngle = CalculateAngle(new Point(lastNode.X, lastNode.Y), point);

                        // > 50 means it's clockwise
                        if (CheckAngleInRightHand(newBubbleAngle, Math.PI, angleFromIntertiveNodeToPoint1, initialAngle > 50))
                        {
                            AddNode(currentNodes, interativeNode, lastNode, newNode, point1);

                            break;
                        }
                        else
                        {
                            AddNode(currentNodes, interativeNode, lastNode, newNode, point2);

                            break;
                        }
                    }
                    else if (!collisionWithPoint1)
                    {
                        AddNode(currentNodes, interativeNode, lastNode, newNode, point1);

                        break;
                    }
                    else if (!collisionWithPoint2)
                    {
                        AddNode(currentNodes, interativeNode, lastNode, newNode, point2);

                        break;
                    }
                }
            }
        }

        private void DrawBubbles(Point canvasCenter, List<BubbleNode> currentNodes)
        {
            var top = chart.ActualHeight;
            var bottom = chart.ActualHeight;
            var left = chart.ActualWidth;
            var right = chart.ActualWidth;

            // calculate bubble zone bundary
            foreach (var node in currentNodes)
            {
                var newLeft = node.X - node.Radius;

                if (newLeft < left)
                {
                    left = newLeft;
                }

                var newRight = canvasCenter.X * 2 - (node.X + node.Radius);

                if (newRight < right)
                {
                    right = newRight;
                }

                var newTop = node.Y - node.Radius;

                if (newTop < top)
                {
                    top = newTop;
                }

                var newBottom = canvasCenter.Y * 2 - (node.Y + node.Radius);

                if (newBottom < bottom)
                {
                    bottom = newBottom;
                }
            }

            #region re-layout canvas & nodes

            var newX = canvasCenter.X * 2 + left - right;
            var newY = canvasCenter.Y * 2 + top - bottom;
            var newCanvasCenter = new Point(newX / 2, newY / 2);
            var centerOffset = new Point(newCanvasCenter.X - canvasCenter.X, newCanvasCenter.Y - canvasCenter.Y);
            ChartCanvas.Width = canvasCenter.X * 2 - left - right;
            ChartCanvas.Height = canvasCenter.Y * 2 - top - bottom;
            var horizontalMargin = canvasCenter.X - ChartCanvas.Width / 2;
            var verticalMargin = canvasCenter.Y - ChartCanvas.Height / 2;
            ChartCanvas.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);

            foreach (var node in currentNodes)
            {
                node.X -= centerOffset.X + horizontalMargin;
                node.Y -= centerOffset.Y + verticalMargin;
            }

            #endregion

            // scale canvas
            var scaleX = chart.ActualWidth / ChartCanvas.Width;
            var scaleY = chart.ActualHeight / ChartCanvas.Height;
            var scale = Math.Min(scaleX, scaleY);
            ChartCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
            ChartCanvas.RenderTransform = new ScaleTransform(scale, scale);

            #region draw bubbles

            foreach(var node in currentNodes)
            {
                Canvas.SetTop(node.Shape, node.Y - node.Radius);
                Canvas.SetLeft(node.Shape, node.X - node.Radius);
                ChartCanvas.Children.Add(node.Shape);
            }

            #endregion
        }

        private void AddNode(List<BubbleNode> currentNodes, BubbleNode interativeNode, BubbleNode lastNode, BubbleNode newNode, Point point)
        {
            newNode.X = point.X;
            newNode.Y = point.Y;
            newNode.TangentBubbles.Add(lastNode);
            newNode.TangentBubbles.Add(interativeNode);
            currentNodes.Add(newNode);
        }

        private bool CheckCollision(Point point, BubbleNode node, double newBubbleRadius, double bubbleGap)
        {
            var collisionAlpha = 0.1;
            var differFromPointXAndNodeX = point.X - node.X;
            var differFromPointYAndNodeY = point.Y - node.Y;
            var distanceBetweenCentersFromNodeToPoint = Math.Sqrt(differFromPointXAndNodeX) * differFromPointXAndNodeX + Math.Pow(differFromPointYAndNodeY, 2);
            var distanceBetweenCentersFromNodeToNewBubble = node.Radius + newBubbleRadius + bubbleGap;

            if (distanceBetweenCentersFromNodeToNewBubble - distanceBetweenCentersFromNodeToPoint > collisionAlpha)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private double CalculateAngle(Point targetPoint, Point referencePoint)
        {
            var differFromTargetXAndReferenceX = targetPoint.X - referencePoint.X;
            var differFromTargetYAndReferenceY = targetPoint.Y - referencePoint.Y;
            var hyperbolicTangent = Math.Tanh(Math.Abs((differFromTargetYAndReferenceY / differFromTargetXAndReferenceX)));

            // second quartile
            if(differFromTargetXAndReferenceX > 0 && differFromTargetYAndReferenceY <= 0)
            {
                return hyperbolicTangent;
            }
            else if (differFromTargetXAndReferenceX <= 0 && differFromTargetYAndReferenceY < 0)
            {
                // third quartile
                if(differFromTargetXAndReferenceX != 0)
                {
                    return Math.PI - hyperbolicTangent;
                }
                else
                {
                    return Math.PI / 2;
                }
            }
            // fourth quartile
            else if (differFromTargetXAndReferenceX < 0 && differFromTargetYAndReferenceY >= 0)
            {
                return Math.PI + hyperbolicTangent;
            }
            else
            {
                // first quartile
                if(differFromTargetXAndReferenceX != 0)
                {
                    return Math.PI * 2 - hyperbolicTangent;
                }
                else
                {
                    return Math.PI * 3 / 2;
                }
            }
        }

        private bool CheckAngleInRightHand(double targetAngle, double rangeAngle, double checkAngle, bool isClockwise)
        {
            var piMultiply2 = Math.PI * 2;

            if (isClockwise)
            {
                if (targetAngle - rangeAngle > 0)
                {
                    if (checkAngle >= targetAngle - rangeAngle && checkAngle <= targetAngle)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if ((checkAngle >= 0 && checkAngle <= targetAngle) || (checkAngle < piMultiply2 && checkAngle >= piMultiply2 - (rangeAngle - targetAngle)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (targetAngle < rangeAngle)
                {
                    if(checkAngle >= targetAngle && checkAngle <= targetAngle + rangeAngle)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if((checkAngle < piMultiply2 && checkAngle >= targetAngle) || (checkAngle >= 0 && checkAngle <= (targetAngle - Math.PI)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        #endregion

        #region Fields & Properties

        public Canvas ChartCanvas { get; set; }

        public List<BubbleNode> CurrentNodes { get; private set; }

        private List<BubbleData> currentDatas;

        private BubbleChart chart;

        private BubbleStyleManager styleManager;

        #endregion
    }
}
