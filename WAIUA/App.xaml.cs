﻿using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using AutoUpdaterDotNET;
using MVVMEssentials.Services;
using MVVMEssentials.Stores;
using MVVMEssentials.ViewModels;
using WAIUA.Helpers;
using WAIUA.Properties;
using WAIUA.ViewModels;
using static WAIUA.Helpers.ValAPI;

namespace WAIUA
{
    public partial class App : Application
    {
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly NavigationStore _navigationStore;

        public App()
        {
            //this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            _navigationStore = new NavigationStore();
            _modalNavigationStore = new ModalNavigationStore();

            if (string.IsNullOrEmpty(Settings.Default.Language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
                Settings.Default.Language = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
                Settings.Default.Save();
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var navigationService = CreateHomeNavigationService();
            navigationService.Navigate();

            MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(_navigationStore, _modalNavigationStore)
            };

            MainWindow.Show();
            base.OnStartup(e);
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.Start("https://raw.githubusercontent.com/Soneliem/WAIUA/master/WAIUA/VersionInfo.xml");
            Constants.CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WAIUA";
            await CheckAndUpdateJson();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }

        //void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        //{
        //    string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
        //    //MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    e.Handled = true;
        //}

        private INavigationService CreateHomeNavigationService()
        {
            return new NavigationService<HomeViewModel>(_navigationStore, CreateHomeViewModel);
        }

        private HomeViewModel CreateHomeViewModel()
        {
            return new HomeViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(),
                CreateSettingsNavigationService(), CreateMatchNavigationService());
        }

        private INavigationService CreateInfoNavigationService()
        {
            return new NavigationService<InfoViewModel>(_navigationStore, CreateInfoViewModel);
        }

        private InfoViewModel CreateInfoViewModel()
        {
            return new InfoViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(),
                CreateSettingsNavigationService());
        }

        private INavigationService CreateSettingsNavigationService()
        {
            return new NavigationService<SettingsViewModel>(_navigationStore, CreateSettingsViewModel);
        }

        private SettingsViewModel CreateSettingsViewModel()
        {
            return new SettingsViewModel(CreateHomeNavigationService(), CreateInfoNavigationService(),
                CreateSettingsNavigationService());
        }

        private INavigationService CreateMatchNavigationService()
        {
            return new NavigationService<MatchViewModel>(_navigationStore, CreateMatchViewModel);
        }

        private MatchViewModel CreateMatchViewModel()
        {
            return new MatchViewModel(CreateHomeNavigationService(), CreateMatchNavigationService());
        }
    }
}