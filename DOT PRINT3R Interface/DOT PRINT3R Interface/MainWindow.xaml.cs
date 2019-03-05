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
using System.Diagnostics;

using Core;
using Core.ImageProcessing;

namespace DOT_PRINT3R_Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Rectangle[,] rectangles;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            FileLoader.LoadImage();

            // Draw image preview
            DrawPreview();

        }

        private void SendFileBtn_Click(object sender, RoutedEventArgs e)
        {
            FileConverter.ConvertFile("dummy");
            FileSender.ConnectAndSend();
        }

        void DrawPreview()
        {
            ImageCanvas.Children.Clear();
            rectangles = new Rectangle[FileLoader.p.ResizeSize.Width, FileLoader.p.ResizeSize.Height];

            for (int i = 0; i < FileLoader.p.ResizeSize.Height; i++)
            {
                for (int j = 0; j < FileLoader.p.ResizeSize.Width; j++)
                {
                    byte brightness = (byte)((float)(FileLoader.p.QuantizeLevel - ImageReader.stream[j + i * FileLoader.p.ResizeSize.Width]) / FileLoader.p.QuantizeLevel * 255f);
                    rectangles[j, i] = new Rectangle()
                    {
                        Width = ImageCanvas.ActualWidth / FileLoader.p.ResizeSize.Width,
                        Height = ImageCanvas.ActualHeight / FileLoader.p.ResizeSize.Height,
                        Fill = new SolidColorBrush(new Color()
                        {
                            R = brightness,
                            G = brightness,
                            B = brightness,
                            A = 255
                        })
                    };
                }
            }

            foreach (Rectangle rect in rectangles)
            {
                ImageCanvas.Children.Add(rect);
            }

            for (int i = 0; i < FileLoader.p.ResizeSize.Height; i++)
            {
                for (int j = 0; j < FileLoader.p.ResizeSize.Width; j++)
                {
                    Canvas.SetLeft(rectangles[j, i], rectangles[j, i].Width * j);
                    Canvas.SetTop(rectangles[j, i], rectangles[j, i].Height * i);
                }
            }
        }
    }
}
