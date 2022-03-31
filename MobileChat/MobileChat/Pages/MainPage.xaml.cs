using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileChat
{
    public partial class MainPage : ContentPage
    {
        public ChatProgram.Chat chat;
        int temp = 0;

        public MainPage()
        {
            chat = new ChatProgram.Chat();
            InitializeComponent();
            Settings.Clicked += OpenSettings;
            Contacts.Clicked += OpenContacts;
            SendButton.Clicked += (s, e) => { if (Input.Text.Length > 0) chat.SendMessage(Input.Text); else return; };
            ChatProgram.Chat.MessageReceived += (s, e) => { Device.BeginInvokeOnMainThread(Update); };
            ChatProgram.Chat.MessageSent += (s, e) => { Device.BeginInvokeOnMainThread(Update); };
            chat.ChangeServerIP(Dns.GetHostAddresses(Dns.GetHostName())[0].ToString());
            new Thread(chat.StartServer).Start();
            //Device.StartTimer(new TimeSpan(0, 0, 2), () => { Update(); return true; });
        }

        async void OpenSettings(object s, EventArgs e)
        {
            await Navigation.PushAsync(new Options(this), true);
        }

        async void OpenContacts(object s, EventArgs e)
        {
            await Navigation.PushAsync(new Contacts(this), true);
        }

        void Update()
        {
            if (chat.ChatHistory.Count == 0)
                return;

            Test.Text += chat.ChatHistory.Last() + "\n";
        }
    }
}
