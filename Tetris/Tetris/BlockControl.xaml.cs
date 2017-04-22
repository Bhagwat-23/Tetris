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

namespace Tetris
{


    /// <summary>
    /// Interaction logic for BlockControl.xaml
    /// </summary>
    public partial class BlockControl : UserControl
    {
        const int MAX_HEIGHT = 24;
        const int MAX_WIDTH = 16;
        
        DrawingVisualElement drawingVisualElement;
        Matrix[,] temp = new Matrix[5, 5];

        public BlockControl()
        {
            InitializeComponent();
            drawingVisualElement = new DrawingVisualElement();
            block.Children.Add(drawingVisualElement);
            this.Loaded += BoardView_Loaded;
        }

       
        void BoardView_Loaded(object sender, RoutedEventArgs e)
        {
            DrawingContext drawingContext = drawingVisualElement.drawingVisual.RenderOpen();
            drawingContext = drawingVisualElement.drawingVisual.RenderOpen();
            for(int i=0;i<5;i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    drawingContext.DrawRectangle(Brushes.Black, (Pen)null, new Rect(25 * j, 25 * i, 25, 25));
                }
            }
           
            drawingContext.Close();
        }



        public object BlockBoard
        {
            get { return (object)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("BlockBoard", typeof(object), typeof(BlockControl), new PropertyMetadata(null, new PropertyChangedCallback(handler)));

        private static void handler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as BlockControl;
            obj.temp = e.NewValue as Matrix[,];

            DrawingContext drawingContext = obj.drawingVisualElement.drawingVisual.RenderOpen();
            drawingContext = obj.drawingVisualElement.drawingVisual.RenderOpen();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (obj.temp[i, j].data == 1)
                        drawingContext.DrawRectangle(Brushes.Green, (Pen)null, new Rect(25 * j, 25 * i, MAX_HEIGHT-1, MAX_HEIGHT-1));
                    else
                        drawingContext.DrawRectangle(Brushes.Black, (Pen)null, new Rect(25 * j, 25 * i, MAX_HEIGHT-1, MAX_HEIGHT-1));
                }
            }
            drawingContext.Close();   
        }

    }
}
