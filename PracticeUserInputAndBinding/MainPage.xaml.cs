/*
Work Cited:
https://stackoverflow.com/questions/32667408/how-to-implement-inotifypropertychanged-in-xamarin-forms
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PracticeUserInputAndBinding
{

    public class ObservableProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public abstract class ViewModelBase : ObservableProperty
    {
        public Dictionary<string, ICommand> Commands { get; protected set; }

        public ViewModelBase()
        {
            Commands = new Dictionary<string, ICommand>();
        }
    }
    class LoginViewModel : ViewModelBase
    {
        #region fields
        string userName;
        string password;
        #endregion

        #region properties
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        #endregion

        #region ctor
        public LoginViewModel()
        {
            //Add Commands
            Commands.Add("Login", new Command(CmdLogin));
        }
        #endregion


        #region UI methods

        private void CmdLogin()
        {
            // do your login jobs here
            Console.WriteLine(UserName);
            Console.WriteLine(Password);
        }
        #endregion
    }
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        LoginViewModel loginViewModel;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
            //loginViewModel = new LoginViewModel();
        }
    }
}
