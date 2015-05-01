using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Windows.Media.Animation;
using System.Collections;
using System.Windows.Threading;

namespace demoSoftware
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        Image[] imgArray = new Image[10];
        Rectangle[] rectArray = new Rectangle[10];
        double xAxis = 0;
        double yAxis = 0;
        double orientation = 0;
        double flashDist = (18 * 2.22) + 39;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            imgArray[0] = img1;
            imgArray[1] = img2;
            imgArray[2] = img3;
            imgArray[3] = img4;
            imgArray[4] = img5;
            imgArray[5] = img6;
            imgArray[6] = img7;
            imgArray[7] = img8;
            imgArray[8] = img9;
            imgArray[9] = img10;
        }



        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">Event Arguments</param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        /// <summary>
        /// This function recognises tag, and gets relative x and y axis. It also gets orientation of the tag.
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">TagVisualizerEvent Arguments</param>
        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            LynxTagVisualization tag = (LynxTagVisualization)e.TagVisualization;

            Console.WriteLine(tag.VisualizedTag.Value);
            orientation = tag.Orientation;
            xAxis = (tag.Center.X - 960) * 2;
            yAxis = (tag.Center.Y - 540) * 2;
            xAxis = xAxis - flashDist;
            yAxis = yAxis - 17;
            Console.WriteLine(orientation);
            
        }

        #region TransferStuff

        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            string result = transferStringBuilder("Bet5034");
            string binary;
            char[] binArray;
            string transferString = result;
            binArray = transferString.ToCharArray();
            int location = 0;
            while (true)
            {
                while (location < transferString.Length)
                {
                    binary = ConvertToBinary(transferString[location]);
                    blink(binary);
                    location++;
                }
                location = 0;
            }

        }

        /// <summary>
        /// This method builds the transfer String with addition null characters between each transfer character.
        /// </summary>
        /// <param name="transferString">string to transfer</param>
        /// <returns>string including null characters in between</returns>
        private string transferStringBuilder(string transferString)
        {
            string BuildString = "";
            for (int i = 0; i < transferString.Length; i++)
            {
               string temp = transferString[i].ToString();

               BuildString = BuildString + (temp + '\0' + '\0' + '\0' ); //To keep whitle light stay, decrease no. of nulls.

            }
            return BuildString;
        }

        /// <summary>
        /// This method actually blinks the binary flashes on table
        /// </summary>
        /// <param name="binary">binary String with null characters</param>
        private void blink(string binary)
        {
            Dispatcher.Invoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                int binLength = binary.Length;
                //no. miliseconds delay, last point
                TimeSpan fadeInTime = new TimeSpan(0, 0, 0, 0, 50); //keep it similar to ThreadSleep--------Timing
                TimeSpan fadeOutTime = new TimeSpan(0, 0, 0, 0, 50); //keep it similar to ThreadSleep-------Timing
                var fadeInAnimation = new DoubleAnimation(0d, fadeInTime);
                var fadeOutAnimation = new DoubleAnimation(1d, fadeOutTime);

                fadeOutAnimation.Completed += (o, e) =>
                {
                    for (int i = 0; i < binLength; i++)
                    {
                   
                        if (binary[i] == '1')
                        {
                            imgArray[i].Margin = new Thickness(xAxis, yAxis + (i * flashDist), 0, 0);
                            imgArray[i].BeginAnimation(Image.OpacityProperty, fadeInAnimation);
                        }
                    }
                };
                Thread.Sleep(50); //keep it similar to FadeIn and FadeOut -----------------------------------Timing
                for (int i = 0; i < binLength; i++)
                {
                    if (binary[i] == '1')
                    {
                        imgArray[i].Margin = new Thickness(xAxis, yAxis + (i * flashDist), 0, 0);
                        imgArray[i].BeginAnimation(Image.OpacityProperty, fadeOutAnimation);
                    }
                }
            }));
        }

        /// <summary>
        /// This method converts character to binary string
        /// </summary>
        /// <param name="asciiString">ascii character</param>
        /// <returns>binary string</returns>
        public static string ConvertToBinary(char asciiString)
        {
            string result = string.Empty;

            result += Convert.ToString((int)asciiString, 2);

           return result;
            
        }
        #endregion

    }
}