using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnrollmentSystem.ViewModel
{
    public class LoginVM : ViewModelBase
    {
        private string _account;

        public string Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }
    }
}
