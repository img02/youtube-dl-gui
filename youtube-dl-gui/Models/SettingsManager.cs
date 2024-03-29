﻿using Newtonsoft.Json;
using System.IO;
using youtube_dl_gui_wrapper.Models;

namespace youtube_dl_gui.Models
{
    public class SettingsManager : ObservableObject
    {
        private const string ConfigFileLocation = "./config.json";

        public Settings UserSettings { get; private set; }


        public SettingsManager()
        {
            LoadSettings();
            //just save settings every time something changes.
            UserSettings.PropertyChanged += ((sender, args) => SaveSettings());
        }

        public void LoadSettings()
        {
            if (File.Exists(ConfigFileLocation))
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                UserSettings =
                    JsonConvert.DeserializeObject<Settings>(File.ReadAllText(ConfigFileLocation), jsonSettings);
                return;
            }
            CreateDefaultSettings();
        }

        public void SaveSettings()
        {
            File.WriteAllText(ConfigFileLocation, JsonConvert.SerializeObject(UserSettings, Formatting.Indented));
            OnPropertyChanged(nameof(UserSettings)); //not needed?
        }

        private void CreateDefaultSettings()
        {
            File.Create(ConfigFileLocation).Close();
            UserSettings = new Settings();
            SaveSettings();
        }
    }
}