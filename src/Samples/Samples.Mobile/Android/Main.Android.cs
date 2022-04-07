﻿namespace Elmish.Uno.Samples.Droid;

using System;

using Android.Runtime;

using Com.Nostra13.Universalimageloader.Core;

using Microsoft.UI.Xaml.Media;

[global::Android.App.ApplicationAttribute(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/icon",
        LargeHeap = true,
        HardwareAccelerated = true,
        Theme = "@style/AppTheme"
    )]
public class Application : Microsoft.UI.Xaml.NativeApplication
{
    public Application(IntPtr javaReference, JniHandleOwnership transfer)
        : base(() => new App(), javaReference, transfer)
    {
        ConfigureUniversalImageLoader();
    }

    private static void ConfigureUniversalImageLoader()
    {
        // Create global configuration and initialize ImageLoader with this config
        using var builder = new ImageLoaderConfiguration.Builder(Context);
        ImageLoaderConfiguration config = builder.Build();

        ImageLoader.Instance.Init(config);

        ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
    }
}
