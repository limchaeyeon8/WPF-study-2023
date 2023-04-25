using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wp11_movieFinder.Logics;
using wp11_movieFinder.Models;

namespace wp11_movieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 검색버튼 - 네이버API 영화 검색
        private async void BtnSearchMovie_Click(object sender, RoutedEventArgs e)
        {
            // 입력검증
            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색할 영화명을 입력하세요♡");
                return;
            }

            // 검색어 글자수
            /*if (TxtMovieName.Text.Length <= 2)
            {
                await Commons.ShowMessageAsync("검색", "검색어를 2자 이상 입력하세요♡");
                return;
            }*/

            try
            {
                SearchMovie(TxtMovieName.Text);
            }
            catch
            {
                await Commons.ShowMessageAsync("오류", "오류발생♥\n{ex.Message}");
            }
        }

        // 텍스트박스에서 키를 입력할 때 엔터를 누르면 검색 시작
        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchMovie_Click(sender, e);
            }
        }

        // 실제 검색 메서드 생성
        private async void SearchMovie(string movieName)
        {
            string tmdb_apiKey= "4ba1adb2876f1eb673543d5ca4bd4a98";

            string encoding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apiKey}" + 
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_movieName}";
            // ↑ 아무렇게나 줄바꾸면 안 됨

            string result = string.Empty;   // 결과값


            // api 실행할 객체
            WebRequest request = null;
            WebResponse response = null;

            StreamReader reader = null;


            // TMDBAPI 요청
            try
            {
                request = WebRequest.Create(openApiUri);        // URL 넣어서 객체 생성
                response = request.GetResponse();               // 요청한 결과를 응답에 할당

                reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();        // json 결과 텍스트로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                response.Close();
            }

            // result를 json으로 변경
            var jsonResult = JObject.Parse(result);     // string 문자열 -> json으로 바뀜

            var total = Convert.ToInt32(jsonResult["total_results"]);       // 전체 검색결과 수

            //await Commons.ShowMessageAsync("검색결과", total.ToString()); 

            var items = jsonResult["results"];
            // items를 데이터그리드에 표시
            var json_array = items as JArray;

            var movieItems = new List<MovieItem>();     // json에서 넘어온 배열을 담을 장소
            foreach (var val in json_array)
            {
                var MovItem = new MovieItem()
                {
                    Id = Convert.ToInt32(val["id"]),
                    Title = Convert.ToString(val["title"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Release_date = Convert.ToString(val["release_date"]),
                    Vote_Average = Convert.ToDouble (val["vote_average"]),
                    Adult = Convert.ToBoolean(val["adult"]),
                };
                movieItems.Add(MovItem);
            }

            this.DataContext = movieItems;
        }

    }
}

