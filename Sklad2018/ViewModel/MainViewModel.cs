using System;
using System.Windows;
using BusinessStruct;
using Sklad2018.Model.ViewModelBase;

namespace Sklad2018.ViewModel
{
    public class MainViewModel : BindBase
    {
        public MainViewModel(Window mainwindow)
        {
            MainWindow = mainwindow;

            try
            {
                if (!DatabaseConnection.TestConnecting())
                {
                    SetupConnectionWindow
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ProgramInitioalisation error ", e);
            }
        }
    }
}