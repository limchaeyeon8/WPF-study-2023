using FestivalInfo.Logics;
using MahApps.Metro.Controls;
using Org.BouncyCastle.Utilities;
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
using System.Xml.Linq;

namespace FestivalInfo
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

        // 검색버튼
        private async void BtnSearchFest_Click(object sender, RoutedEventArgs e)
        {
            // 입력검증
            if (string.IsNullOrEmpty(TxtFestName.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색할 영화명을 입력하세요♡");
                return;
            }

            try
            {
                SearchFest(TxtFestName.Text);
            }
            catch
            {
                await Commons.ShowMessageAsync("오류", "오류발생♥\n{ex.Message}");
            }
        }
        // 텍스트박스에서 키를 입력할 때 엔터를 누르면 검색 시작

        private void TxtFestName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                        {
                            BtnSearchFest_Click(sender, e);
                        }
        }

        // 실제 검색 메서드
        private async Task SearchFest(string festName)
        {
            string fest_apiKey = "8ca9e42a-45fe-4c84-8a06-fac2ab0682d1";

            string encoding_festeName = HttpUtility.UrlEncode(festName, Encoding.UTF8);
            string openApiUri = $"http://api.kcisa.kr/openapi/service/rest/meta16/getkopis07?serviceKey={fest_apiKey}" +
                                $"8ca9e42a-45fe-4c84-8a06-fac2ab0682d1&numOfRows=100&pageNo=1";
            // ↑ 아무렇게나 줄바꾸면 안 됨

            string result = string.Empty;   // 결과값


            // api 실행할 객체
            WebRequest request = null;
            WebResponse response = null;

            StreamReader reader = null;


            // API 요청
            try
            {
                request = WebRequest.Create(openApiUri);    // URL 넣어서 객체 생성
                response = await request.GetResponseAsync();// 요청한 결과를 비동기 응답에 할당 // await ~~.GetResponseAsync() 비동기화

                reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();                // json 결과 텍스트로 저장

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
                    Title = Convert.ToString(val["title"]),
                    Poster_Path = Convert.ToString(val["poster_path"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Original_Language = Convert.ToString(val["original_language"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"]),
                    Popularity = Convert.ToDouble(val["popularity"]),
                    Release_date = Convert.ToString(val["release_date"]),
                    Adult = Convert.ToBoolean(val["adult"]),
                    Id = Convert.ToInt32(val["id"]),
                    OverView = Convert.ToString(val["overview"]),
                };
                movieItems.Add(MovItem);
            }

            this.DataContext = movieItems;
            isFavorite = false;     // 얘는 즐겨찾기 아니야~
            StsResult.Content = $"OpenAPI {movieItems.Count} 건 조회완료";
        }

        private void TxtFestName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



    }
}
