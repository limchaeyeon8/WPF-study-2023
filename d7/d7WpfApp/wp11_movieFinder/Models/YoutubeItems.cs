﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace wp11_movieFinder.Models
{
    public class YoutubeItem
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string ChannelTitle { get; set; }

        public string URL { get; set; }

        public BitmapImage Thumbnail { get; set; }


    }
}