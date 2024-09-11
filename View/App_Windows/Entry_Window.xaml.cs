using EchoVibe.View.Pages;
using System;
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
using System.Windows.Shapes;
using EchoVibe.View.Control_User.ScrollViewers;


namespace EchoVibe.View.App_Windows
{
    
    public partial class Entry_Window : Window
    {
        public Entry_Window()
        {
            InitializeComponent();
            Main_Frame.Content = new Sign_In_Page();
        }
    }
}
