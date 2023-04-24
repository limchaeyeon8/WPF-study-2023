using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wp10_employeesApp;
using wp10_employeesApp.ViewModels;

namespace wp10_employeesApp
{
    // Caliburn으로 MVVM 실행할 때 
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();       // Caliburn MVVM 초기화
        }

        // 시작한 후에 초기화 진행
        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<MainViewModel>();
        }
    }
}
