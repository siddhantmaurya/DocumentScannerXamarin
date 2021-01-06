using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1
{
    public partial class MainPage : ContentPage , INotifyPropertyChanged
    {


        #region Properties

        private List<User> users;

        public List<User> Users
        {
            get { return users; }
            set 
            { 
                users = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor

        public MainPage()
        {
            InitializeComponent();
            
            this.BindingContext = this;

            PopulateUsers();
        }

        private void PopulateUsers()
        {
            List<User> list = GetUsers();

            Users = list;

        }

        private static List<User> GetUsers()
        {
            var list = new List<User>();

            var user1 = new User() { FirstName = "Mohan", Phone = "99999999", Age = 20, LastName = "Sharma" };
            var user2 = new User() { FirstName = "Rohan", Phone = "777777777", Age = 30, LastName = "Sharma" };
            var user3 = new User() { FirstName = "Sohan", Phone = "88888888", Age = 10, LastName = "Sharma" };

            list.Add(user1);
            list.Add(user2);
            list.Add(user3);
            return list;
        }

        #endregion

        #region Methods




        #endregion
    }
}
