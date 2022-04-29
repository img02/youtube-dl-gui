﻿using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using youtube_dl_gui_wrapper.Models;

namespace youtube_dl_gui_wrapper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var url = @"https://www.youtube.com/watch?v=Xpt9TyFzwJA";

            var source = new VideoSource(url);

            source.DownloadLog.PropertyChanged += Updated;
            source.Download();

            //await source.GetVideoFormats();

            Thread.Sleep(5000);
            source.Cancel();

            source.Formats.ForEach(Console.WriteLine);

            YtdlpProcess x = new YtdlpProcess();


        }

        public static async Task Start(VideoSource source)
        {
            try
            {
                await source.GetVideoFormats();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Updated(object? sender, PropertyChangedEventArgs e)
        {
            var info = sender as DownloadInfo;

            Console.WriteLine($"{e.PropertyName} updated.\n" +
                              $"{info?.ToString()}\n\n");


        }
    }
}
