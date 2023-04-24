using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using wp08_personalInfoApp.Models;

namespace wp08_personalInfoApp.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        // View에서 사용할 멤버변수 선언

        #region < 입력쪽 변수 >
        private string inFirstName;
        private string inLastName;
        private string inEmail;
        private DateTime inBirth = DateTime.Now;        //  = DateTime.Now ----> 최초 현재날짜로 세팅
        #endregion

        #region < 결과출력쪽 변수 >
        private string outFirstName;
        private string outLastName;
        private string outEmail;
        private string outBirth;  // 생일 날짜 출력할 때는 문자열 대체
        private string outAdult;
        private string outBday;
        private string outZodiac;
        #endregion

        //////// 일이 많아짐.. MainView에서 실제로 사용할 속성들(프로퍼티)
        #region < 입력을 위한 속성들 >
        public string InFirstName
        {
            get => inFirstName;
            set
            {
                inFirstName = value;
                RaisePropertyChange(nameof(InFirstName));   // "InFirstName" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public string InLastName
        {
            get => inLastName;
            set
            {
                inLastName = value;
                RaisePropertyChange(nameof(InLastName));   // "InLastName" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public string InEmail
        {
            get => inEmail;
            set
            {
                inEmail = value;
                RaisePropertyChange(nameof(InEmail));   // "InEmail" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public DateTime InBirth
        {
            get => inBirth;
            set
            {
                inBirth = value;
                RaisePropertyChange(nameof(InBirth));   // "InFirstName" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }

        #endregion

        #region < 출력을 위한 속성들 >
        public string OutFirstName
        {
            get => outFirstName;
            set
            {
                outFirstName = value;
                RaisePropertyChange(nameof(OutFirstName));   // "InEmail" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public string OutLastName
        {
            get => outLastName;
            set
            {
                outLastName = value;
                RaisePropertyChange(nameof(OutLastName));   // "InEmail" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public string OutEmail
        {
            get => outEmail;
            set
            {
                outEmail = value;
                RaisePropertyChange(nameof(OutEmail));   // "InEmail" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public string OutBirth
        {
            get => outBirth;
            set
            {
                outBirth = value;
                RaisePropertyChange(nameof(OutBirth));   // "InEmail" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public string OutAdult
        {
            get => outAdult;
            set
            {
                outAdult = value;
                RaisePropertyChange(nameof(OutAdult));   // "InEmail" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public string OutBday
        {
            get => outBday;
            set
            {
                outBday = value;
                RaisePropertyChange(nameof(OutBday));   // "InEmail" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }
        public string OutZodiac
        {
            get => outZodiac;
            set
            {
                outZodiac = value;
                RaisePropertyChange(nameof(OutZodiac));   // "InEmail" 문자열이 들어감 -> 전체 시스템에 알려줌
            }
        }

        #endregion


        #region < 버튼 클릭에 대한 이벤트 처리((명령)) >
        // 릴레이커맨드로 클릭 등과 같은 이벤트 만들기
        // MVVM에서는 클릭이벤트 사용 안 하기 때문에 명령이라 부름
        private ICommand proceedCommand;
        public ICommand ProceedCommand          // 제일 중요!!!!!!
        {
            get
            {
                return proceedCommand ?? (proceedCommand = new RelayCommand<object>(         // 프로시드커맨드가 널이라면
                    o => Proceed(), o => !string.IsNullOrEmpty(inFirstName) &&
                                         !string.IsNullOrEmpty(inLastName) &&
                                         !string.IsNullOrEmpty(inEmail) &&
                                         !string.IsNullOrEmpty(inBirth.ToString())
                    ));
            }


        }

        // 버튼 클릭시 실제로 처리를 수행하는 메서드
        private void Proceed()
        {
            try
            {
                Person person = new Person(inFirstName, inLastName, inEmail, inBirth);

                OutFirstName = person.FirstName;
                OutLastName = person.LastName;
                OutEmail = person.Email;
                OutBirth = person.Date.ToString("yyyy년 MM월 dd일");
                OutAdult = person.IsAdult == true ? "성년" : "미성년";
                OutBday = person.IsBirthday == true ? "생일" : "-";
                OutZodiac = person.Zodiac;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예외발생 {ex.Message}");
            }
            #endregion
        }
    }
}
