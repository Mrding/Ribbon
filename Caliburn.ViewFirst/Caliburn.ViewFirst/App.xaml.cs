namespace Caliburn.ViewFirst {
    using System.Windows;

    public partial class App : Application {
        public App() {
            new MefBootstrapper();
            InitializeComponent();
        }
    }
}