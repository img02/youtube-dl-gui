﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using youtube_dl_gui_wrapper.Models;

namespace youtube_dl_gui_wrapper
{
    /// <summary>
    /// For use with yt-dlp <see cref="https://github.com/yt-dlp/yt-dlp"/>
    /// </summary>
    public class YtdlpProcess : BaseYtDlProcess
    {
        /// <summary>
        /// Creates a <see cref="YtdlpProcess"/> with an exe path.
        /// </summary>
        /// <param name="exe">Defaults to PATH - "yt-dlp.exe"</param>
        public YtdlpProcess(string exe = "yt-dlp.exe") : base(exe)
        {
        }

        /// <summary>
        /// Creates a <see cref="YtdlpProcess"/> with an exe path and output path.
        /// </summary>
        /// <param name="defaultOutputFolder">Defaults to Desktop</param>
        /// <param name="exe">Defaults to PATH - "yt-dlp.exe"</param>
        public YtdlpProcess(string defaultOutputFolder, string exe = "yt-dlp.exe") : base(defaultOutputFolder, exe)
        {
        }


        #region helpers

        #region available video format helpers


        protected override List<VideoFormat> ExtractInfoForFormats(List<string> formatList)
        {
            var videoFormats = new List<VideoFormat>();

            var toRemove = formatList.FindIndex(s => s.Contains("RESOLUTION"));
            Console.WriteLine($"\nTO REMOVE {toRemove}\n");

            formatList.RemoveRange(0, toRemove + 2); //remove lines that are not relevant.

            formatList.ForEach(Console.WriteLine);

            foreach (var str in formatList)
            {
                if (str.Contains("images")) continue;
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
            var resolutionLabel = split[^2];
            var height = string.Empty;
            var width = string.Empty;
            var fps = resolution == "audio" ? "" : split[3] + "fps"; //if 'resolution' is audio, no fps avail.
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
            Console.WriteLine(info);
            Trace.WriteLine(info);
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