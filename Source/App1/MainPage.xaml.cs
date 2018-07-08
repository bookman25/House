using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using HassSDK;
using HassSDK.Models;
using Newtonsoft.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public HassClient Client { get; }

        public MainPage()
        {
            this.InitializeComponent();

            Client = new HassClient("localhost");
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var response = await Client.AuthenticateAsync("");
            if (response)
            {
                var domains = await Client.Services.GetAsync();
                var climate = domains["climate"];
                var setTemp = climate.Services["set_temperature"];

                //var changeTemp = new ChangeTemp { EntityId = "climate.linear_unknown_type5442_id5437_cooling_1", Temperature = "78" };
                //var res = await Client.Services.CallServiceAsync(setTemp, changeTemp);
            }
        }
    }
}
