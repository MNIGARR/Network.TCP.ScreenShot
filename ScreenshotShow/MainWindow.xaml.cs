using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Printing;
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

namespace ScreenshotShow
{
    
    public partial class MainWindow : Window
    {
        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public TcpClient Client { get; set; }
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapImage), typeof(MainWindow));


        public MainWindow()
        {
            Port = 1864;
            IpAddress = IPAddress.Parse("192.168.1.66");
            InitializeComponent();
            DataContext = this;
        }

        public RelayCommand StartCommand 
        { get => new RelayCommand(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        
                        while (true)
                        {
                            Client = new();

                            Client.Connect(IpAddress, Port);

                            if (Client.Connected)
                            {
                                try
                                {
                                    using NetworkStream nwStream = Client.GetStream();
                                    byte[] imageData = new byte[4096];
                                    int length;
                                    using MemoryStream memoryStream = new();


                                    while ((length = nwStream.Read(imageData, 0, imageData.Length)) > 0)
                                    {
                                        memoryStream.Write(imageData, 0, length);
                                    }

                                    memoryStream.Seek(0, SeekOrigin.Begin);
                                    BitmapImage bitmapImage = new();
                                    bitmapImage.BeginInit();
                                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmapImage.StreamSource = memoryStream;
                                    bitmapImage.EndInit();
                                    bitmapImage.Freeze();

                                    Dispatcher.Invoke(() => ImageSource = bitmapImage);


                                }
                                catch (Exception ex)
                                {
                                    // Handle exceptions if any
                                    MessageBox.Show("Error loading the image: " + ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception except)
                    {
                        MessageBox.Show("Something went wrong. Couldn't connect!! ");

                    }

                }, TaskCreationOptions.LongRunning);
                
            });
        }

       
    }
}
