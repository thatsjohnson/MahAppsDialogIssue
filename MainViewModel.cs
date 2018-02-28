using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using System.Windows.Input;

namespace TestMahApps
{
    public class MainViewModel : BindableBase
    {
        private RelayCommand _buttonClickCommand;

        public ICommand ButtonClickCommand
        {
            get
            {
                return _buttonClickCommand ?? (_buttonClickCommand = new RelayCommand(async obj =>
                {
                    var task = MahControlsHelper.ShowMessage(Application.Current.MainWindow,
                        $"Confirm Kicking Trader off", $"Are you sure you want to kick off Trader?",
                        MessageDialogStyle.AffirmativeAndNegative);

                    var result = await task;

                    if (result == MessageDialogResult.Affirmative)
                    {
                        Console.WriteLine("Success");
                    }
                }));
            }
        }
    }
}