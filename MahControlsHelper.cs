using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace TestMahApps
{
    public class MahControlsHelper
    {
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

            return await metroWindow.ShowMessageAsync(title, message, style, settings);
        }

        public static async Task<MessageDialogResult> ShowMessage(Window window, string title, string message, MessageDialogStyle dialogStyle)
        {
            var metroWindow = (window as MetroWindow);
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;

            return await metroWindow.ShowMessageAsync(title, message, dialogStyle, metroWindow.MetroDialogOptions);
        }

        //public static Task ShowChildWindowAsync(ChildWindow dialog, ChildWindowManager.OverlayFillBehavior overlayFillBehavior = ChildWindowManager.OverlayFillBehavior.WindowContent)
        //{
        //    var metroWindow = (Application.Current.MainWindow as MetroWindow);
        //    if (metroWindow != null)
        //        metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

        //    return metroWindow?.ShowChildWindowAsync(dialog, overlayFillBehavior);
        //}

        //public static Task ShowChildWindowAsync(ChildWindow dialog, Panel container)
        //{
        //    var metroWindow = (Application.Current.MainWindow as MetroWindow);
        //    if (metroWindow != null)
        //        metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

        //    return metroWindow?.ShowChildWindowAsync(dialog, container);
        //}

        //public static Task ShowChildWindowAsync(ChildWindow dialog, Window window, ChildWindowManager.OverlayFillBehavior overlayFillBehavior = ChildWindowManager.OverlayFillBehavior.WindowContent)
        //{
        //    var metroWindow = (window as MetroWindow);
        //    if (metroWindow != null)
        //        metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

        //    //var grid = WPFExtensions.FindVisualChildren<Grid>(window).FirstOrDefault();

        //    return metroWindow?.ShowChildWindowAsync(dialog, overlayFillBehavior);
        //}

        //public static async void ShowChildWindowAsync(ChildWindow dialog, Window window, string GridNameToPlace)
        //{
        //    var metroWindow = (window as MetroWindow);
        //    if (metroWindow != null)
        //        metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

        //    var grid = WPFExtensions.FindVisualChildren<Grid>(window).Where(g => g.Name.Equals(GridNameToPlace)).FirstOrDefault();

        //    await metroWindow?.ShowChildWindowAsync(dialog, grid);
        //}
    }
}

