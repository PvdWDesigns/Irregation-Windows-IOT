
using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using System.Threading;
using System.Threading.Tasks;

namespace IrrigationTimerPi
{
    public sealed partial class MainPage : Page
    {
        //private const int LED_PIN = 5;
        private GpioPin pin;
        private GpioPin PumpPin;
        private GpioPinValue pinValue;
        private DispatcherTimer timer1;
       // private DispatcherTimer timer2;
        int KickOffHour = 18;
        int KickOffMinute = 15;
        int KickOffSecond = 00;
        int[] ValveTimes = new int[] { 10, 20,10, 20, 10}; // Seconds Now
        int[] ValvePins = new int[] { 26, 19, 13, 6, 5}; //Gpio's on Pi for each Valve
        int PumpPinNumber = 21; // GPIO on Pi to control Pump / 220V
        int t = 0;

        public MainPage()
        { 
            InitializeComponent();


            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(1000);
            timer1.Tick += Timer_Tick;
            //timer2 = new DispatcherTimer();
            //timer2.Interval = TimeSpan.FromSeconds(30);
            //timer2.Tick += Timer_Tock;

           // InitGPIO();
            //if (pin != null)
            //{
            timer1.Start();
            // timer2.Start();
            //}        
        }

        //private void InitGPIO()
        //{
        //    var gpio = GpioController.GetDefault();

        //    for (int i = 0; i < ValvePins.Length; i++)
        //    {
        //        pin = gpio.OpenPin(ValvePins[i]);
        //        // pinValue = GpioPinValue.High;
        //        // pin.Write(pinValue);
        //        pin.SetDriveMode(GpioPinDriveMode.Output);

        //    }
          
    // // Show an error if there is no GPIO controller
    // if (gpio == null)
    // {
    //     pin = null;
    //     //GpioStatus.Text = "There is no GPIO controller on this device.";
    //     return;
    // }

    // //pin = gpio.OpenPin(LED_PIN);
    // // pinValue = GpioPinValue.High;
    // //pin.Write(pinValue);
    // //pin.SetDriveMode(GpioPinDriveMode.Output);

    //// GpioStatus.Text = "GPIO pin initialized correctly.";

//}



        private async void Timer_Tick(object sender, object e)
        {
            if ((DateTime.Now.Hour == KickOffHour) && (DateTime.Now.Minute == KickOffMinute) && (DateTime.Now.Second == KickOffSecond))
            {
                timer1.Stop();
                SwitchOnPump();
                var gpio = GpioController.GetDefault();


                for (int i = 0; i < ValvePins.Length; i++)
                {
                    pin = gpio.OpenPin(ValvePins[i]);
                    pin.SetDriveMode(GpioPinDriveMode.Output);
                    pinValue = GpioPinValue.High;
                    pin.Write(pinValue);
                    await Task.Delay(TimeSpan.FromSeconds(ValveTimes[t]));
                    pinValue = GpioPinValue.Low;
                    pin.Write(pinValue);
                    pin.Dispose();
                    t++;

                }
                timer1.Start();
                t = 0;
                          }

        }


        private async void SwitchOnPump()
        {
                      
            var gpio = GpioController.GetDefault();
            PumpPin = gpio.OpenPin(PumpPinNumber);
            PumpPin.SetDriveMode(GpioPinDriveMode.Output);
            pinValue = GpioPinValue.High;
            int PumpTime = 0;
            for (int i = 0; i < ValveTimes.Length; i++)
            {
                PumpTime += ValveTimes[i];
            }
                await Task.Delay(TimeSpan.FromSeconds(PumpTime));

            pinValue = GpioPinValue.Low;
            PumpPin.Write(pinValue);
            PumpPin.Dispose();
            
             }

    }
}
