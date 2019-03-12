﻿using System;
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
        public bool redrawOnValueChanged = true;
        ImageTools.ImageConversionParams p;
        public bool imagePicked = false;
        Process companion;

        Point cursorCanvasPoint;

        public MainWindow()
        {
            InitializeComponent();

            p = new ImageTools.ImageConversionParams()
            {
                Bias = 0,
                QuantizeLevel = 4,
                ResizeSize = new System.Drawing.Size(28, 28),
                NormalizeImage = false,
                InvertImage = false,
                InvertPost = false
            };
            FileLoader.p = p;

            SendFileBtn.IsEnabled = false;

            XSizeBox.Text = p.ResizeSize.Width.ToString();
            YSizeBox.Text = p.ResizeSize.Height.ToString();

            BiasSlider.Value = p.Bias;
            QuantizeSlider.Value = p.QuantizeLevel;

            BiasSlider.ValueChanged += UpdateImageEvent;
            XSizeBox.TextChanged += UpdateImageEvent;
            YSizeBox.TextChanged += UpdateImageEvent;
            QuantizeSlider.ValueChanged += UpdateImageEvent;
            InvertCheckBox.Checked += UpdateImageEvent;
            NormalizeCheckBox.Checked += UpdateImageEvent;
            InvertPostCheckBox.Checked += UpdateImageEvent;
            InvertCheckBox.Unchecked += UpdateImageEvent;
            NormalizeCheckBox.Unchecked += UpdateImageEvent;
            InvertPostCheckBox.Unchecked += UpdateImageEvent;

            ImageCanvas.MouseMove += ImageCanvas_MouseMove;
            ImageCanvas.MouseLeftButtonDown += ImageCanvas_MouseLeftButtonDown;
            ImageCanvas.MouseRightButtonDown += ImageCanvas_MouseRightButtonDown;
        }

        private void ImageCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (imagePicked) DrawOnImage(0);
        }

        private void ImageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (imagePicked) DrawOnImage((byte)p.QuantizeLevel);
        }

        Point GetCursorPos()
        {
            cursorCanvasPoint = Mouse.GetPosition(ImageCanvas);

            Point imagePos = new Point(Math.Floor(cursorCanvasPoint.X * (p.ResizeSize.Width / ImageCanvas.ActualWidth)), Math.Floor(cursorCanvasPoint.Y * (p.ResizeSize.Width / ImageCanvas.ActualHeight)));
            return imagePos;
        }

        void UpdateStream(Point point, byte value)
        {
            ImageReader.stream[(int)point.X + (int)point.Y * p.ResizeSize.Width] = value;
        }

        void DrawOnImage(byte value)
        {
            Point imagePos = GetCursorPos();
            UpdateStream(imagePos, value);
            DrawPreview();
        }

        private void ImageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (imagePicked)
                if (e.LeftButton == MouseButtonState.Pressed)
                    DrawOnImage((byte)p.QuantizeLevel);
                else if (e.RightButton == MouseButtonState.Pressed)
                    DrawOnImage(0);
        }

        private void UpdateImageEvent<T>(object sender, T e)
        {
            UpdateConversionParams();
            ConvertImage(p);
        }

        private void LoadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            FileLoader.ImageLoadResult result;
            if ((result = FileLoader.LoadImage()) == FileLoader.ImageLoadResult.Success)
            {
                imagePicked = true;
                ConvertImage(p);
                SendFileBtn.IsEnabled = true;
            }
            else if (result == FileLoader.ImageLoadResult.Invalid)
            {
                UIMessage.ShowError("The file you selected does not appear to be an image. Try another file.");
            }
            else if (result == FileLoader.ImageLoadResult.UnsupportedImage)
            {
                UIMessage.ShowError("SVGs are not supported. Use raster images only.");
            }
        }

        private void SendFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (imagePicked)
            {
                EV3.ConnectToBrick();
                if (EV3.Connected)
                {
                    EV3.Disconnect();
                    if (imagePicked)
                    {
                        SendFileBtn.IsEnabled = false;
                        FileConverter.ConvertFile(System.IO.Path.ChangeExtension(FileLoader.filePath, ".rtf"));
                        //FileSender.ConnectAndSend();

                        companion = new Process();
                        companion.StartInfo.UseShellExecute = true;
                        companion.StartInfo.Arguments = FileConverter.outputPath;
                        companion.StartInfo.FileName = "EV3Comm.exe";
                        companion.StartInfo.CreateNoWindow = true;
                        companion.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //companion.EnableRaisingEvents = true;

                        //companion.Exited += Companion_Exited;

                        try
                        {
                            companion.Start();
                        }
                        catch (Exception)
                        {
                            UIMessage.ShowError("Companion app not found. Is there EV3Comm.exe in the same directory as this executable?");
                            companion.Dispose();
                        }
                        SendFileBtn.IsEnabled = true;
                    }
                }
                else
                {
                    UIMessage.ShowError("Cannot connect to EV3. Make sure it's plugged in via USB and turned on.");
                }
                EV3.Disconnect();
            }
        }

        private void Companion_Exited(object sender, EventArgs e)
        {
            SendFileBtn.IsEnabled = true;
            if (companion.ExitCode == -1)
            {
                Debug.WriteLine("Something went wrong");
            }
        }

        void UpdateConversionParams()
        {
            int sizeX = p.ResizeSize.Width, sizeY = p.ResizeSize.Height;
            System.Drawing.Size oldSize = p.ResizeSize;
            bool canChangeSize = int.TryParse(XSizeBox.Text, out sizeX) && int.TryParse(YSizeBox.Text, out sizeY) && imagePicked && XSizeBox.Text != "" && YSizeBox.Text != "" && sizeX > 0 && sizeY > 0;

            p = new ImageTools.ImageConversionParams()
            {
                Bias = (float)BiasSlider.Value,
                QuantizeLevel = (int)QuantizeSlider.Value,
                ResizeSize = canChangeSize ? new System.Drawing.Size(sizeX, sizeY) : oldSize,
                NormalizeImage = NormalizeCheckBox.IsChecked == true,
                InvertImage = InvertCheckBox.IsChecked == true,
                InvertPost = InvertPostCheckBox.IsChecked == true
            };

            FileLoader.p = p;
        }

        void ConvertImage(ImageTools.ImageConversionParams p)
        {
            if (imagePicked)
            {
                ImageReader.ConvertImageToByteStream(p);

                // Draw image preview
                DrawPreview();

            }
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
