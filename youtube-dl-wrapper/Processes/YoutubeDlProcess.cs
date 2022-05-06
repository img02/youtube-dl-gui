﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using youtube_dl_gui_wrapper.Models;

namespace youtube_dl_gui_wrapper
{
    /// <summary>
    /// For use with youtube-dl https://github.com/ytdl-org/youtube-dl
    /// </summary>
    public class YoutubeDlProcess : BaseYtDlProcess
    {
        /// <summary>
        /// Creates a <see cref="YoutubeDlProcess"/> with an exe path.
        /// </summary>
        /// <param name="exe">Defaults to PATH - "youtube-dl.exe"</param>
        public YoutubeDlProcess(string exe = "youtube-dl.exe") : base(exe)
        {
        }
        /// <summary>
        /// Creates a <see cref="YoutubeDlProcess"/> with an exe path and output path.
        /// </summary>
        /// <param name="defaultOutputFolder">Defaults to Desktop</param>
        /// <param name="exe">Defaults to PATH - "youtube-dl.exe"</param>
        public YoutubeDlProcess(string defaultOutputFolder, string exe = "youtube-dl.exe") : base(defaultOutputFolder, exe)
        {
        }



        #region helpers

        #region available video format helpers

        protected override List<VideoFormat> ExtractInfoForFormats(List<string> formatList)
        {
            var videoFormats = new List<VideoFormat>();

            formatList.RemoveRange(0, 3); //remove the first 3 lines as they are not relevant.

            foreach (var str in formatList)
            {
                if (str == null) continue;
                var vf = GetVideoFormatFromString(str);
                videoFormats.Add(vf);
            }

            return videoFormats;
        }

        
        protected override VideoFormat GetVideoFormatFromString(string formatStringArr)
        {
            var split = Regex.Replace(formatStringArr, @"\s+", " ").Split(" ");

            var formatCode = split[0];
            var ext = split[1];
            var resolution = split[2];
            var resolutionLabel = resolution == "audio"? "audio" : split[3];
            var height = string.Empty;
            var width = string.Empty;
            var fps = Regex.Match(formatStringArr, @"\d+fps").Groups[0].Value;
            var size = split[^1] == "(best)" ? "Unknown" : split[^1];

            if (resolution.Contains("x"))
            {
                var widthXheight = resolution.Split("x");
                width = widthXheight[0];
                height = widthXheight[1];
            }

            return new VideoFormat(formatCode, ext, resolution, resolutionLabel, height, width, fps, size);
        }
        #endregion

        #region Download info string helpers

        protected override void UpdateDownloadInfo(DownloadInfo toUpdate, string info)
        {
            //Example output:
            //[download]   0.2% of 151.34MiB at 83.58KiB/s ETA 30:50
            var infoArr = Regex.Replace(info, @"\s+", " ").Split(" "); //get rid of extra spaces, then split

            var percent = infoArr[1];
            //for edge case issue where the final line can be cut short resulting index out of range.
            if (percent == "100%")
            {
                toUpdate.DownloadPercentage = percent;
                return;
            };

            var size = infoArr[3].Replace("~", "");
            var speed = infoArr[5];
            var eta = infoArr[7];


            toUpdate.FileSize = size;
            toUpdate.DownloadPercentage = percent;
            toUpdate.DownloadSpeed = speed;
            toUpdate.ETA = eta;
            //string stuff to get pieces of info
        }

        #endregion

        #endregion
    }
}