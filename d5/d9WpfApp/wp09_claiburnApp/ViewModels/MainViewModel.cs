using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using wp09_claiburnApp.Models;

namespace wp09_claiburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        // Caliburn version 업으로 인해 프로퍼티 붙이는 걸로 변경
        private string firstName = "ChaeYeon";

        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName);    // 속성값이 변경된 걸 시스템에 알려주는 역할
                NotifyOfPropertyChange(nameof(CanClearName));   // 값이 들어가면 클리어 버튼 활성화
                NotifyOfPropertyChange(nameof(FullName));
            }
        }

        private string lastName = "임";

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => FullName); // 변화통보 
            }
        }

        public string FullName
        {
            get => $"{LastName} {FirstName}";
        }

        //
        //
        private BindableCollection<Person> managers = new BindableCollection<Person> ();
        public BindableCollection<Person> Managers
        {
            get => managers;
            set => managers = value;
        }

        //

        private Person selectedManager;

        public Person SelectedManager
        {
            get => selectedManager;
            set
            {
                selectedManager = value;
                LastName = selectedManager.LastName;
                FirstName = selectedManager.FirstName;
                NotifyOfPropertyChange(nameof(SelectedManager));
            }
        }




        /// 생성자 생성
        public MainViewModel()
        {
            // DB 사용하면 여기서 DB 접속 >> 데이터 Select 까지,,,
            Managers.Add(new Person { FirstName = "John", LastName = "Carmack" });
            Managers.Add(new Person { FirstName = "Steve", LastName = "Jobs" });
            Managers.Add(new Person { FirstName = "Bill", LastName = "Gates" });
            Managers.Add(new Person { FirstName = "Elon", LastName = "Musk" });
        }


        public void ClearName()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        // 메서드와 이름동일하게 앞에 Can을 붙임
        public bool CanClearName
        {
            get => !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName);
        }
    }
}
