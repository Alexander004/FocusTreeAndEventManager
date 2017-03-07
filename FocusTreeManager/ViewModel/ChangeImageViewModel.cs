﻿using FocusTreeManager.Helper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChangeImageViewModel : ViewModelBase
    {
        public class ImageData
        {
            public ImageSource Image { get; set; }
            public string Filename { get;  set; }
            public int MaxWidth { get; set; }
        }

        public RelayCommand<string> SelectCommand { get; private set; }

        private string focusImage;

        public string FocusImage
        {
            get
            {
                return focusImage;
            }
            set
            {
                focusImage = value;
                RaisePropertyChanged(() => FocusImage);
            }
        }

        public ObservableCollection<ImageData> ImageList { get; }

        /// <summary>
        /// Initializes a new instance of the ChangeImageViewModel class.
        /// </summary>
        public ChangeImageViewModel()
        {
            ImageList = new ObservableCollection<ImageData>();
            SelectCommand = new RelayCommand<string>(SelectFocus);
        }

        public void LoadImages(string SubFolder, string CurrentImage)
        {
            int MaxWidth = 250;
            ImageType type = ImageType.Event;
            switch (SubFolder)
            {
                case "Focus":
                    MaxWidth = 100;
                    type = ImageType.Goal;
                    break;
                case "Event":
                    MaxWidth = 250;
                    type = ImageType.Event;
                    break;
            }
            
            ImageList.Clear();
            foreach (KeyValuePair<string, ImageSource> source in ImageHelper.findAllGameImages(type))
            {
                ImageList.Add(new ImageData
                {
                    Image = source.Value,
                    Filename = source.Key,
                    MaxWidth = MaxWidth
                });
            }
            focusImage = CurrentImage;
            RaisePropertyChanged(() => ImageList);
        }

        public void SelectFocus(string path)
        {
            FocusImage = Path.GetFileNameWithoutExtension(path);
            Messenger.Default.Send(new NotificationMessage(this, "HideChangeImage"));
        }
    }
}