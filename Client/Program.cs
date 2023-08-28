using System.Net.Sockets;
using System.Net;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using Windows.UI.Xaml.Media;

IPAddress ia = IPAddress.Parse("192.168.1.66");
IPEndPoint ep = new(ia, 1864);

TcpListener listener = new(ep);
listener.Start();

Console.WriteLine($"Listening at {ep}...");

while (true)
{
    var client = listener.AcceptTcpClient();

    Console.WriteLine($"{client.Client.RemoteEndPoint} connected!!!");

    Task.Run(() =>
    {
        while (true)
        {
            var stream = client.GetStream();
            var image = ScreenShot();

            ImageConverter imageConverter = new();
            var bytes = (byte[])imageConverter.ConvertTo(image, typeof(byte[]))!;
            stream.Write(bytes);
            Console.WriteLine("Screenshoted!");


            Task.Delay(2700).Wait();
            stream.Close();

        }
    });
}

Bitmap ScreenShot()
{
    Bitmap memoryImage;
    memoryImage = new Bitmap(1920, 1080);
    Size s = new Size(memoryImage.Width, memoryImage.Height);

    Graphics memoryGraphics = Graphics.FromImage(memoryImage);

    memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);

    return memoryImage;
}


//TcpClient Client = new();
//var ip = IPAddress.Parse("192.168.1.66");
//IPEndPoint ep = new(ip, 43050);



//Task.Run(() =>
//{
//    while (true)
//    {
//        try
//        {
//            Client = new();
//            Client.Connect(ep);
//            if (Client.Connected)
//            {
//                try
//                {
//                    using (NetworkStream networkStream = Client.GetStream())
//                    {
//                        byte[] imageData = new byte[4096];
//                        int bytesRead;
//                        using (MemoryStream memoryStream = new MemoryStream())
//                        {

//                            while ((bytesRead = networkStream.Read(imageData, 0, imageData.Length)) > 0)
//                            {
//                                memoryStream.Write(imageData, 0, bytesRead);
//                            }


//                            memoryStream.Seek(0, SeekOrigin.Begin);
//                            BitmapImage bitmapImage = new BitmapImage();
//                            bitmapImage.BeginInit();
//                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
//                            bitmapImage.StreamSource = memoryStream;
//                            bitmapImage.EndInit();
//                            bitmapImage.Freeze();

//                            Dispatcher.Invoke(() => PhotoSource = bitmapImage);
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    // Handle exceptions if any
//                    MessageBox.Show("Error loading the image: " + ex.Message);
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            MessageBox.Show(ex.Message);
//        }
//        Task.Delay(5000).Wait();
//    }
//}