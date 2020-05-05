﻿using Plugin.Connectivity;
using PokeApp.Services.Interfaces;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokeApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Variables

        private IAPIService _apiService;
        private INavigationService _navigationService;

        #endregion

        public LoginViewModel(INavigationService navigationService, IAPIService apiService)
            : base(navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;

            #region Commands logic

            Login = new Command(async () =>
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    await App.Current.MainPage.DisplayAlert("Error",
                                                      "Email y Password son requeridos", "ok");
                    return;
                }

                if (!CrossConnectivity.Current.IsConnected)
                {
                    NoInternetAlert();
                    return;
                }

                if (await MakeLogin())
                    await _navigationService.NavigateAsync("NavigationPage/MainPage");

            });

            #endregion
        }

        #region Bindable Properties

        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Methods

        private async Task<bool> MakeLogin()
        {
            try
            {
                var service = DependencyService.Get<IFirebaseAuth>();
                var token = await service.LoginWithEmailPassword(Email, Password);

                if (!string.IsNullOrEmpty(token))
                    return true;
                else 
                {
                    await App.Current.MainPage.DisplayAlert("Error",
                                                     "Email o Password incorrectos", "ok");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorAlert();
                return false;
            }

        }

        #endregion

        #region Commands

        public ICommand Login { get; private set; }

        #endregion
    }
}