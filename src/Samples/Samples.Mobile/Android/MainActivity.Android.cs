namespace Elmish.Uno.Samples;

using Android.App;
using Android.Views;

[Activity(
        MainLauncher = true,
        ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
    )]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
}
