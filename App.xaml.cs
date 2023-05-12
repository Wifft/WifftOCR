﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

using Windows.Storage;

using WifftOCR.DataModels;
using WifftOCR.Interfaces;
using WifftOCR.Services;
using WifftOCR.ViewModels;
using WifftOCR.Views;


namespace WifftOCR
{
    public partial class App : Application
    {
        private MainWindow m_window;

        public const string SETTINGS_LOCATION_URI = "ms-appdata:///roaming/settings.json";

        public IHost Host { get; }

        public static T GetService<T>() where T : class
        {
            if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service) {
                throw new ArgumentException($"{typeof(T)} needs to be a registered in ConfigureServices within App.xaml.cs");
            }

            return service;
        }

        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureServices((context, services) => { 
                    services.AddSingleton<ISettingsService, SettingsService>();

                    services.AddTransient<ShellPage>();
                    services.AddTransient<ShellViewModel>();

                    services.AddTransient<WelcomePage>();

                    services.AddTransient<CaptureAreasPage>();
                    services.AddTransient<CaptureAreasViewModel>();

                    services.AddTransient<SettingsPage>();
                    services.AddTransient<SettingsViewModel>();
                })
                .Build();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            MainWindow.EnsurePageIsSelected();

            await CheckIfConfigFileExists();
        }

        private static async Task CheckIfConfigFileExists()
        {
            StorageFile configFile;

            try {
                configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(SETTINGS_LOCATION_URI));
                Console.WriteLine(configFile.Path);
            } catch (FileNotFoundException) {
                StorageFolder folder = ApplicationData.Current.RoamingFolder;

                await CreateConfigFile(folder);
            }
        }

        private static async Task CreateConfigFile(StorageFolder folder)
        {
            StorageFile file = await folder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);

            Settings settings = new()
            {
                Observer = null,
                ServerEndpoint = null,
                ServerKey = null,
                CaptureAreas = new ObservableCollection<CaptureArea>()
            };

            using Stream stream = await file.OpenStreamForWriteAsync();
            await JsonSerializer.SerializeAsync(stream, settings);
        }
    }
}
