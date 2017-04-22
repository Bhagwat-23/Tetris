using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Tetris
{
    public class DrawingVisualElement : FrameworkElement
    {
        private VisualCollection _children;

        public DrawingVisual drawingVisual;

        public DrawingVisualElement()
        {
            _children = new VisualCollection(this);

            drawingVisual = new DrawingVisual();
            _children.Add(drawingVisual);
        }

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
                throw new ArgumentOutOfRangeException();

            return _children[index];
        }
    }


    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        const int MAX_HEIGHT = 24;
        const int MAX_WIDTH = 16;
         
        DrawingVisualElement drawingVisualElement;
        Matrix[,] temp = new Matrix[MAX_HEIGHT, MAX_WIDTH];
        
        public BoardView()
        {
            InitializeComponent();
            drawingVisualElement = new DrawingVisualElement();
            board.Children.Add(drawingVisualElement);
            
           // this.DataContext = new BoardViewModel();
            this.Loaded += BoardView_Loaded;
        }

       
        void BoardView_Loaded(object sender, RoutedEventArgs e)
        {
            DrawingContext drawingContext = drawingVisualElement.drawingVisual.RenderOpen();
            drawingContext = drawingVisualElement.drawingVisual.RenderOpen();
            for(int i=0;i<MAX_HEIGHT;i++)
            {
                for (int j = 0; j < MAX_WIDTH; j++)
                {
                    drawingContext.DrawRectangle(Brushes.Black, (Pen)null, new Rect(25 * j, 25 * i, 25, 25));
                }
            }

            FormattedText formattedText1 = new FormattedText(
                "PLAY",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                50,
                Brushes.Green);
            drawingContext.DrawText(formattedText1, new Point() { X = 140, Y = 170 });
            
            FormattedText formattedText2 = new FormattedText(
                "TETRIS",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                50,
                Brushes.Green);
            drawingContext.DrawText(formattedText2, new Point() { X=110,Y=230});

            drawingContext.Close();
        }



        public object TetrisBoard
        {
            get { return (object)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("TetrisBoard", typeof(object), typeof(BoardView), new PropertyMetadata(null, new PropertyChangedCallback(handler)));

        private static void handler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as BoardView;
            obj.temp = e.NewValue as Matrix[,];

            DrawingContext drawingContext = obj.drawingVisualElement.drawingVisual.RenderOpen();
            drawingContext = obj.drawingVisualElement.drawingVisual.RenderOpen();
            for (int i = 0; i < MAX_HEIGHT; i++)
            {
                for (int j = 0; j < MAX_WIDTH; j++)
                {
                    if (obj.temp[i, j].data == 0)
                        drawingContext.DrawRectangle(Brushes.Black, (Pen)null, new Rect(25 * j, 25 * i, MAX_HEIGHT-1, MAX_HEIGHT-1));
                    else if (obj.temp[i, j].data == -1)
                        drawingContext.DrawRectangle(Brushes.Yellow, (Pen)null, new Rect(25 * j, 25 * i, MAX_HEIGHT-1, MAX_HEIGHT-1));
                    else
                        drawingContext.DrawRectangle(Brushes.Green, (Pen)null, new Rect(25 * j, 25 * i, MAX_HEIGHT-1, MAX_HEIGHT-1));                    
                }
            }
            drawingContext.Close();
        }



        public bool IsGameOver
        {
            get { return (bool)GetValue(IsGameOverProperty); }
            set { SetValue(IsGameOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsGameOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsGameOverProperty =
            DependencyProperty.Register("IsGameOver", typeof(bool), typeof(BoardView), new PropertyMetadata(default(bool),OnGameOver));

        private static void OnGameOver(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as BoardView;
            obj.IsGameOver = (bool)e.NewValue;
            if (obj.IsGameOver == true)
            {
                DrawingContext drawingContext = obj.drawingVisualElement.drawingVisual.RenderOpen();
                drawingContext = obj.drawingVisualElement.drawingVisual.RenderOpen();

                FormattedText formattedText1 = new FormattedText(
                "GAME",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                50,
                Brushes.Red);
                drawingContext.DrawText(formattedText1, new Point() { X = 130, Y = 170 });

                FormattedText formattedText2 = new FormattedText(
                    "OVER",
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    50,
                    Brushes.Red);
                drawingContext.DrawText(formattedText2, new Point() { X = 130, Y = 230 });

                drawingContext.Close();
            }
        }

        private static void GameOverHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as BoardView;
            obj.IsGameOver = (bool)e.NewValue;
            //if (obj.IsGameOver == true)
            {
                DrawingContext drawingContext = obj.drawingVisualElement.drawingVisual.RenderOpen();
                drawingContext = obj.drawingVisualElement.drawingVisual.RenderOpen();

                FormattedText formattedText1 = new FormattedText(
                "GAME",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                50,
                Brushes.Red);
                drawingContext.DrawText(formattedText1, new Point() { X = 130, Y = 170 });

                FormattedText formattedText2 = new FormattedText(
                    "OVER",
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    50,
                    Brushes.Red);
                drawingContext.DrawText(formattedText2, new Point() { X = 130, Y = 230 });

                drawingContext.Close();
            }
        }

    }
}
