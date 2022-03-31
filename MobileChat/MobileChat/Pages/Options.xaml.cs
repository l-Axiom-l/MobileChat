using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Options : ContentPage
    {
        MainPage mainPage;
        public Options(MainPage Page)
        {
            InitializeComponent();
            mainPage = Page;
            ClientIPInput.TextChanged += (s, e) => mainPage.chat.ClientIPAddress = ClientIPInput.Text;
            ServerIPInput.TextChanged -= (s, e) => mainPage.chat.ServerIPAddress = ServerIPInput.Text;
            StartClient.Clicked += StartClientMethod;
            StartServer.Clicked += StartServerMethod;
            ClientIPInput.Text = Preferences.Get("ClientIP", "ClientIP");
            ServerIPInput.Text = mainPage.chat.ServerIPAddress;
        }

        void StartClientMethod(object s, EventArgs e)
        {
            Thread temp = new Thread(mainPage.chat.StartClient);
            Preferences.Set("ClientIP", ClientIPInput.Text);
            temp.Start();
        }

        void StartServerMethod(object s, EventArgs e)
        {
            Thread temp = new Thread(mainPage.chat.StartServer);
            temp.Start();
        }
    }
}