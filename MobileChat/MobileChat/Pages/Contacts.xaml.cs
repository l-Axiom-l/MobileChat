using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class Contacts : ContentPage
    {
        MainPage mainPage;
        Dictionary<string, string> contacts = new Dictionary<string, string>();

        public Contacts(MainPage Page)
        {
            InitializeComponent();
            mainPage = Page;
            AddContact.Clicked += AddContactMethod;
            Load();
        }

        async void AddContactMethod(object s, EventArgs e)
        {
            string newContactName = await DisplayPromptAsync("Name", "Enter the Contacts Name");
            string newContactIP = await DisplayPromptAsync("IP", "Enter the Contacts IPAddress", keyboard: Keyboard.Telephone);

            Button button = new Button();
            {
                button.Text = newContactName + "\n" + newContactIP;
                button.HeightRequest = 80;
                button.Clicked += ContactClicked;
                button.Opacity = 0;
            }
            ContactList.Children.Add(button);
            contacts.Add(newContactName, newContactIP);
            await button.FadeTo(1);
            Save();
        }

        async void ContactClicked(object s, EventArgs e)
        {
            string[] info = (s as Button).Text.Split('\n');
            switch (await DisplayActionSheet("Options", "Back", "Delete", "Use", "ChangeIP", "ChangeName"))
            {
                case "Delete":
                    await (s as Button).FadeTo(0);
                    ContactList.Children.Remove(s as Button);
                    contacts.Remove(info[0]);
                    break;

                case "ChangeName":
                    string temp = await DisplayPromptAsync("ChangeName", "Enter new Name", placeholder: info[0]);
                    contacts.Remove(info[0]);
                    contacts.Add(temp, info[1]);
                    (s as Button).Text = temp + "\n" + info[1];
                    Save();
                    break;

                case "ChangeIP":
                    string newIP = await DisplayPromptAsync("ChangeIP", "Enter new IP", placeholder: info[1], keyboard: Keyboard.Telephone);
                    contacts[info[0]] = newIP;
                    (s as Button).Text = info[0] + "\n" + newIP;
                    Save();
                    break;

                case "Use":
                    mainPage.chat.ChangeClientIP(info[1]);
                    new Thread(mainPage.chat.StartClient).Start();
                    break;
            }
        }

        void Load()
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Contacts.txt"))
                return;

            string[] temp = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Contacts.txt");
            foreach (string tempItem in temp)
            {
                string[] info = tempItem.Split('-');

                Button button = new Button();
                {
                    button.Text = info[0] + "\n" + info[1];
                    button.HeightRequest = 80;
                    button.Clicked += ContactClicked;
                }
                ContactList.Children.Add(button);
                contacts.Add(info[0], info[1]);

            }
        }

        void Save()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            File.Open(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Contacts.txt", FileMode.Create).Close();
            //DisplayAlert("Test", Environment.GetFolderPath(Environment.SpecialFolder.Personal), "OK");
            
            foreach (KeyValuePair<string, string> contact in contacts)
            {
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Contacts.txt", contact.Key + "-" + contact.Value);
            }
        }
    }
}