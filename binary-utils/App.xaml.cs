using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace binary_utils
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;
        private GlobalHotkeys _globalHotkeys;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyIcon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon) FindResource("NotifyIcon");
            _globalHotkeys = new GlobalHotkeys(MainWindow); //TODO: no main window - make a dummy?
            _globalHotkeys.AddHotKey(Key.B, ModifierKeys.Control | ModifierKeys.Shift, () => { 
                if (!ActualUtil.ReverseHex()) ShowBalloon("couldn't interpret as hex"); //TODO: code duplication
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

        public void ShowBalloon(string text)
        {
            _notifyIcon.ShowBalloonTip(null, text, BalloonIcon.None);
        }
    }
}
