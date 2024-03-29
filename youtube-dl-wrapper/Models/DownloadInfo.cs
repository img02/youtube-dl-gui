﻿using System;
using System.Globalization;

namespace youtube_dl_gui_wrapper.Models
{
    public class DownloadInfo : ObservableObject
    {
        private string _downloadPercentage;
        private string _downloadSpeed;
        private string _fileSize;
        private string _downloaded;
        private string _eta;
        private bool _isLiveStream;

        public string DownloadPercentage
        {
            get => _downloadPercentage;
            set
            {
                if (value == _downloadPercentage) return;
                _downloadPercentage = value;
                OnPropertyChanged(nameof(DownloadPercentage));
                UpdateTotalDownloaded();
            }
        }

        public string DownloadSpeed
        {
            get => _downloadSpeed;
            set
            {
                if (value == _downloadSpeed) return;
                _downloadSpeed = value;
                OnPropertyChanged(nameof(DownloadSpeed));
            }
        }

        public string FileSize
        {
            get => _fileSize;
            set
            {
                if (value == _fileSize) return;
                _fileSize = value;
                OnPropertyChanged(nameof(FileSize));
            }
        }

        public string Downloaded
        {
            get => _downloaded;
            set
            {
                if (value == _downloaded) return;
                _downloaded = value;
                OnPropertyChanged(nameof(Downloaded));
            }
        }

        public string ETA
        {
            get => _eta;
            set
            {
                if (value == _eta) return;
                _eta = value;
                OnPropertyChanged(nameof(ETA));
            }
        }

        public bool IsLiveStream
        {
            get => _isLiveStream;
            set
            {
                if (value == _isLiveStream) return;
                _isLiveStream = value;
                OnPropertyChanged(nameof(IsLiveStream));
            }
        }


        private void UpdateTotalDownloaded()
        {
            if (string.IsNullOrEmpty(_fileSize) || _downloadPercentage == string.Empty) return;
            string unit = _fileSize.Substring(_fileSize.Length - 3);
            double percent = Double.Parse(_downloadPercentage.Replace("%", ""));
            double size = Double.Parse(_fileSize.Remove(_fileSize.Length - 3));
            Downloaded = (Math.Round(percent / 100 * size, 2)).ToString(CultureInfo.InvariantCulture) + unit;
        }

        public override string ToString()
        {
            return $"%: {DownloadPercentage}\n" +
                   $" : {Downloaded}\n" +
                   $"of: {FileSize}\n" +
                   $"Speed: {DownloadSpeed}\n" +
                   $"Eta: {ETA}";
        }

    }
}