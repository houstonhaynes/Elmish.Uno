namespace Elmish.Uno.Samples.Skia.Gtk;

using System;

using GLib;

internal static class Program
{
    private static void Main(string[] args)
    {
        ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
        {
            Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
            expArgs.ExitApplication = true;
        };

        var host = new GtkHost(() => new App(), args);

        host.Run();
    }
}
