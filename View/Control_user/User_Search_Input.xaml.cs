using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using EchoVibe.Backend.Classes;
using EchoVibe.Backend.Services;
using EchoVibe.View.App_Windows;
using EchoVibe.View.Control_User;

namespace EchoVibe.View.Control_User
{
    public partial class User_Search_Input : UserControl, INotifyPropertyChanged
    {
        public User_Search_Input()
        {
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string imagesource;
        public string ImageSource
        {
            get { return imagesource; }
            set
            {
                imagesource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        private string placeholder;
        public string Placeholder
        {
            get { return placeholder; }
            set
            {
                placeholder = value;
                OnPropertyChanged(nameof(Placeholder));
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }



        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = textInput.Text;

            if (string.IsNullOrEmpty(Text))
                tbPlaceholder.Visibility = Visibility.Visible;
            else
                tbPlaceholder.Visibility = Visibility.Hidden;
        }


        public event EventHandler EnterPressed;
        private void TextInput_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                EnterPressed?.Invoke(this, EventArgs.Empty);


            }
        }


        public event EventHandler SearchButton_Event_Clicked;
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            SearchButton_Event_Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
