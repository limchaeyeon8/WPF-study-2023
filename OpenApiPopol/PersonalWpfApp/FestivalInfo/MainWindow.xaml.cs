using FestivalInfo.Logics;
using FestivalInfo.Models;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace FestivalInfo
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        Boolean isFavorite = false; // false -> openApi로 검색해온 결과 / true -> 즐겨찾기 보기


        public MainWindow()
        {
            InitializeComponent();
        }

        // 검색버튼
        private async void BtnSearchFest_Click(object sender, RoutedEventArgs e)
        {
            // 입력검증


            try
            {
                await SearchFest();
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류발생♥\n{ex.Message}");
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
        private async Task SearchFest()
        {
            string fest_apiKey = "8ca9e42a-45fe-4c84-8a06-fac2ab0682d1";

            ///string encoding_festeName = HttpUtility.UrlEncode(festName, Encoding.UTF8);
            string openApiUri = $"http://api.kcisa.kr/openapi/service/rest/meta16/getkopis07?serviceKey={fest_apiKey}" +
                                $"&numOfRows=100&pageNo=1";
            string result = string.Empty;   // 결과값
            string xmlResult = string.Empty;

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
                xmlResult = reader.ReadToEnd();                // json 결과 텍스트로 저장

                result = Commons.XmlDocumentToJson(xmlResult);

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

            var total = Convert.ToInt32(jsonResult["response"]["body"]["totalCount"]);      // 전체 검색결과 수

            //await Commons.ShowMessageAsync("검색결과", total.ToString()); 

            var items = jsonResult["response"]["body"]["items"]["item"];
            // items를 데이터그리드에 표시
            var json_array = items as JArray;

            var FestItems = new List<FestItem>();     // json에서 넘어온 배열을 담을 장소
            foreach (var val in json_array)
            {
                var MovItem = new FestItem()
                {
                    Id = Convert.ToInt32(val["id"]),
                    Title = Convert.ToString(val["title"]),
                    SpatialCoverage = Convert.ToString(val["spatialCoverage"]),
                    TemporalCoverage = Convert.ToString(val["temporalCoverage"]),
                    SubjectCategory = Convert.ToString(val["subjectCategory"]),
                    Url = Convert.ToString(val["url"]),
                    CollectionDb = Convert.ToString(val["collectionDb"]),
                    ReferenceIdentifier = Convert.ToString(val["referenceIdentifier"]),
                    SubDescription = Convert.ToString(val["subDescription"])

                };
                FestItems.Add(MovItem);
            }

            this.DataContext = FestItems;
            isFavorite = false;     // 즐겨찾기 아님
            StsResult.Content = $"OpenAPI {FestItems.Count} festivals";
        }

        // 그리드에서 셀을 선택하면
        private async void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                string ReferenceIdentifier = string.Empty;

                if (GrdResult.SelectedItem is FestItem)                    // openAPI로 검색된 영화의 포스터
                {
                    var movie = GrdResult.SelectedItem as FestItem;
                    ReferenceIdentifier = movie.ReferenceIdentifier;
                }
                else if (GrdResult.SelectedItem is FavFestItem)            // 즐겨찾기 DB에서 가져온 영화 포스터
                {
                    var movie = GrdResult.SelectedItem as FavFestItem;
                    ReferenceIdentifier = movie.ReferenceIdentifier;

                }

                //var movie = GrdResult.SelectedItem as FestItem;

                Debug.WriteLine(ReferenceIdentifier);     // 

                if (string.IsNullOrEmpty(ReferenceIdentifier))        // 포스터 이미지가 없으면 No_Picture
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
                }
                else        // 포스터 이미지 경로가 있으면
                {
                    var base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
                    ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{ReferenceIdentifier}", UriKind.RelativeOrAbsolute));
                }
            }
            catch
            {
                await Commons.ShowMessageAsync("오류", $"이미지로드 오류발생♥");
            }

        }



        #region < 즐겨찾기 추가 버튼 >
        private async void BtnAddFav_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 축제를 선택하세요(복수선택 가능)♥");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 축제입니다♥");
                return;
            }

            List<FavFestItem> list = new List<FavFestItem>();
            foreach (FestItem item in GrdResult.SelectedItems)
            {
                var favFest = new FavFestItem()
                {
                    Id = item.Id,
                    Title = item.Title,
                    SpatialCoverage = item.SpatialCoverage,
                    TemporalCoverage = item.TemporalCoverage,
                    SubjectCategory = item.SubjectCategory,
                    Url = item.Url,
                    CollectionDb = item.CollectionDb,
                    ReferenceIdentifier = item.ReferenceIdentifier,
                    SubDescription = item.SubDescription
                };
                list.Add(favFest);
            }

            try
            {
                // DB 연결 확인이 먼저
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    // 
                    var query = @"INSERT INTO FestItem
                                                ( Title
                                                , SpatialCoverage
                                                , TemporalCoverage
                                                , SubjectCategory
                                                , Url
                                                , CollectionDb
                                                , ReferenceIdentifier
                                                , SubDescription)
                                                VALUES
                                                ( @Title
                                                , @SpatialCoverage
                                                , @TemporalCoverage
                                                , @SubjectCategory
                                                , @Url
                                                , @CollectionDb
                                                , @ReferenceIdentifier
                                                , @SubDescription)";


                    var insRes = 0;
                    foreach (FestItem item in GrdResult.SelectedItems)     // openAPI로 조회된 결과라서 FestItem
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);   // 여기 안에 들어가야함

                        // 파라미터명, 아이템 속성명, 필드명 맞추면 이렇게 편함
                        //SqlParameter prmId = new SqlParameter("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@SpatialCoverage", item.SpatialCoverage);
                        cmd.Parameters.AddWithValue("@TemporalCoverage", item.TemporalCoverage);
                        cmd.Parameters.AddWithValue("@SubjectCategory", item.SubjectCategory);
                        cmd.Parameters.AddWithValue("@Url", item.Url);
                        cmd.Parameters.AddWithValue("@CollectionDb", item.CollectionDb);
                        cmd.Parameters.AddWithValue("@ReferenceIdentifier", item.ReferenceIdentifier);
                        cmd.Parameters.AddWithValue("@SubDescription", item.SubDescription);

                        insRes += cmd.ExecuteNonQuery();        // 들어간 갯수만큼 결과가 나올 것
                    }

                    if (list.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장성공♡");
                        StsResult.Content = $"즐겨찾기 {insRes} 건 저장완료";

                    }
                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장 오류\n 관리자에게 문의하세요♥");
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장 오류\n {ex.Message}♥");
            }
        }

        #endregion

        #region < 즐겨찾기 보기 버튼 >
        private async void BtnFav_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            //TxtFestName.Text = string.Empty;

            List<FestItem> list = new List<FestItem>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"SELECT Id
	                                  , Title
                                      , SpatialCoverage
                                      , TemporalCoverage
                                      , SubjectCategory
                                      , Url
                                      , CollectionDb
                                      , ReferenceIdentifier
                                      , SubDescription
                                   FROM festitem
                                   ORDER BY Id ASC";
                    var cmd = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavFestItem");

                    foreach (DataRow dr in dSet.Tables["FavFestItem"].Rows)
                    {
                        list.Add(new FestItem
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            Title = Convert.ToString(dr["title"]),
                            SpatialCoverage = Convert.ToString(dr["spatialCoverage"]),
                            TemporalCoverage = Convert.ToString(dr["temporalCoverage"]),
                            SubjectCategory = Convert.ToString(dr["subjectCategory"]),
                            Url = Convert.ToString(dr["url"]),
                            CollectionDb = Convert.ToString(dr["collectionDb"]),
                            ReferenceIdentifier = Convert.ToString(dr["referenceIdentifier"]),
                            SubDescription = Convert.ToString(dr["subDescription"])
                        });
                    }

                    this.DataContext = list;
                    isFavorite = true;      // 즐겨찾기에서 가져온 내용~
                    StsResult.Content = $"즐겨찾기 {list.Count} 건 조회완료";


                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB조회 오류\n{ex.Message} ♥");
            }
        }

        #endregion

        #region < 즐겨찾기 삭제 >
        private async void BtnDelFav_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", $"즐겨찾기만 삭제할 수 있습니다♥");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0) // 아무선택도 안 됨
            {
                await Commons.ShowMessageAsync("오류", $"삭제할 축제를 선택하세요♥");
                return;
            }

            try   // 삭제
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = "DELETE FROM FestItem WHERE Id = @Id";
                    var delRes = 0;

                    foreach (FestItem item in GrdResult.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 성공!!!");
                        StsResult.Content = $"즐겨찾기 {delRes} 건 삭제완료";        // 화면에 안 나옴

                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제 일부 성공♥!!!");  // 발생할 일이 거의 전무
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 삭제 오류\n{ex.Message} ♥");

            }

            BtnFav_Click(sender, e);    // 즐겨찾기 보기 이벤트핸들러를 한 번 실행
        }

        #endregion

        #region < 조회 >
        private async void BtnReqRealTime_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "http://api.kcisa.kr/openapi/service/rest/meta16/getkopis07?serviceKey=8ca9e42a-45fe-4c84-8a06-fac2ab0682d1&numOfRows=100&pageNo=1";
            string result = string.Empty;

            // webRequest, webResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"OpenAPI 조회 오류\n{ex.Message} ♥");
            }

            var jsonResult = JObject.Parse(result);
            var status = Convert.ToInt32(jsonResult["status"]);

            try
            {
                if (status == 200)      // 정상이면 데이터 받아서 처리
                {
                    var data = jsonResult["data"];
                    var json_array = data as JArray;

                    var dustSensors = new List<Models.FestItem>();
                    foreach (var sensor in json_array)
                    {

                        dustSensors.Add(new FestItem
                        {
                            Id = 0,
                            Title = Convert.ToString(sensor["title"]),
                            SpatialCoverage = Convert.ToString(sensor["spatialCoverage"]),
                            TemporalCoverage = Convert.ToString(sensor["temporalCoverage"]),
                            SubjectCategory = Convert.ToString(sensor["subjectCategory"]),
                            Url = Convert.ToString(sensor["url"]),
                            CollectionDb = Convert.ToString(sensor["collectionDb"]),
                            ReferenceIdentifier = Convert.ToString(sensor["referenceIdentifier"]),
                            SubDescription = Convert.ToString(sensor["subDescription"])

                        });

                    }
                    this.DataContext = dustSensors;
                    StsResult.Content = $"OpenAPI {dustSensors.Count} 건 조회완료♡";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리 오류\n{ex.Message} ♥");

            }
        }
        #endregion

        private async void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", $"선택한 데이터가 없습니다 ♥");
                return;
            }

            if (GrdResult.SelectedItem is FestItem)
            {
                var item = GrdResult.SelectedItem as FestItem;

                ImgPoster.Source = new BitmapImage(new Uri(item.ReferenceIdentifier, UriKind.RelativeOrAbsolute));
            }
        }
    }

}
