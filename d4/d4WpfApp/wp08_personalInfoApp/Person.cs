using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wp08_personalInfoApp.Logics;

namespace wp08_personalInfoApp
{
    internal class Person
    {
        // 외부에서 접근 불가
        //private string firstName;                     // 오토프로퍼티면 지워도 됨
        //private string lastName;
        private string email;
        private DateTime date;

        public string FirstName { get; set; }           // 오토프로퍼티
        public string LastName { get; set; }            // 오토프로퍼티
        public string Email 
        { 
            get => email;
            set
            {
                if (Commons.IsValidEmail(value) != true)// 이메일 형식에 일치X
                {
                    throw new Exception("유효하지 않은 이메일 형식");
                }
                else
                {
                    email = value;
                }
            }
        }
        public DateTime Date 
        { 
            get => date; 
            set
            {
                var result = Commons.GetAge(value);
                if (result > 130 || result <= 0)
                {
                    throw new Exception("유효하지 않은 생일");
                }
                else
                {
                    date = value;
                }
            }
        }

        public bool IsAdult
        {
            // set 필요 없음
            get
            {
                return Commons.GetAge(date) > 18; // 19살 이상이면 true
            }
        }

        public bool IsBirthday
        {
            get => DateTime.Now.Month == date.Month &&
                    DateTime.Now.Day == date.Day; // 오늘과 월,일 같으면 생일
        }

        public string Zodiac
        {
            get => Commons.GetZodiac(date);         // 12지로 띠 받아옴
        }


        public Person(string firstName, string lastName, string email, DateTime date)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Date = date;
        }
    }
}
