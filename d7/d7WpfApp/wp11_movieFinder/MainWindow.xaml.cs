﻿using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using wp11_movieFinder.Logics;
using wp11_movieFinder.Models;

namespace wp11_movieFinder
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
            string tmdb_apiKey = "4ba1adb2876f1eb673543d5ca4bd4a98";

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
                request = WebRequest.Create(openApiUri);    // URL 넣어서 객체 생성
                response = await request.GetResponseAsync();// 요청한 결과를 비동기 응답에 할당 // await ~~.GetResponseAsync() 비동기화

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

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtMovieName.Focus();	// 텍스트박스에 포커스 셋
        }

        // 그리드에서 셀을 선택하면
        private async void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                string posterPath = string.Empty;

                if (GrdResult.SelectedItem is MovieItem)                    // openAPI로 검색된 영화의 포스터
                {
                    var movie = GrdResult.SelectedItem as MovieItem;
                    posterPath = movie.Poster_Path; 
                }
                else if (GrdResult.SelectedItem is FavMovieItem)            // 즐겨찾기 DB에서 가져온 영화 포스터
                {
                    var movie = GrdResult.SelectedItem as FavMovieItem;
                    posterPath = movie.Poster_Path;

                }

                //var movie = GrdResult.SelectedItem as MovieItem;

                Debug.WriteLine(posterPath);     // 

                if (string.IsNullOrEmpty(posterPath))        // 포스터 이미지가 없으면 No_Picture
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
                }
                else        // 포스터 이미지 경로가 있으면
                {
                    var base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
                    ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{posterPath}", UriKind.RelativeOrAbsolute));
                }
            }
            catch
            {
                await Commons.ShowMessageAsync("오류", $"이미지로드 오류발생");
            }

        }

        // 영화 예고편 유튜브보기
        private async void BtnViewTrail_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 선택하세요♥");
                return;
            }

            if (GrdResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 하나만 선택하세요♥");
                return;
            }

            string movieName = string.Empty;
            if (GrdResult.SelectedItem is MovieItem)
            {
                var movie = GrdResult.SelectedItem as MovieItem;

                movieName = movie.Title;

            }
            else if (GrdResult.SelectedItem is FavMovieItem)
            {
                var movie = GrdResult.SelectedItem as FavMovieItem;

                movieName = movie.Title;
            }

            
            //await Commons.ShowMessageAsync("유튜브", $"예고편 볼 영화 {movieName}");
            var trailer = new Trailer(movieName);
            trailer.Owner = this;       // Trailer의 부모는 MainWindow
            trailer.WindowStartupLocation = WindowStartupLocation.CenterOwner;  // 부모창의 정중앙에 위치
            // trailer.Show();      // 모달리스로 창을 열면 부모창을 손 댈 수 있기 때문에 사용하지 말기 (ex> 부모창 닫으면 같이 닫힘)
            trailer.ShowDialog();   // ★모달창
        }

        // 검색 결과 중 좋아하는 영화 저장

        private async void BtnAddFav_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택하세요(복수선택 가능)♥");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 영화입니다♥");
                return;
            }

            List<FavMovieItem> list = new List<FavMovieItem>();
            foreach (MovieItem item in GrdResult.SelectedItems)
            {
                var favMovie = new FavMovieItem()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Original_Title = item.Original_Title,
                    Release_date = item.Release_date,
                    Original_Language = item.Original_Language,
                    Adult = item.Adult,
                    Popularity = item.Popularity,
                    Vote_Average = item.Vote_Average,
                    Poster_Path = item.Poster_Path,
                    OverView = item.OverView,
                    Reg_Date = DateTime.Now
                };
                list.Add(favMovie);
            }

            //await Commons.ShowMessageAsync("저장할 데이터 수", list.Count.ToString());
            //return;

            #region < MySQL 테스트 >
            // MySQL 데이터 입력 (테스트용)
            /*using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                try
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    // 
                    var query = @"INSERT INTO  favmovieitem
                                            ( Id
                                            , Title
                                            , Original_Title
                                            , Release_date
                                            , Original_Language
                                            , Adult
                                            , Popularity
                                            , Vote_Average
                                            , Poster_Path
                                            , OverView
                                            , Reg_Date)
                                        VALUES
                                            ( @Id
                                            , @Title
                                            , @Original_Title
                                            , @Release_date
                                            , @Original_Language
                                            , @Adult
                                            , @Popularity
                                            , @Vote_Average
                                            , @Poster_Path
                                            , @OverView
                                            , @Reg_Date)";


                    var insRes = 0;
                    foreach (FavMovieItem item in list)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);   // 여기 안에 들어가야함

                        // 파라미터명, 아이템 속성명, 필드명 맞추면 이렇게 편함
                        //SqlParameter prmId = new SqlParameter("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Release_date", item.Release_date);
                        cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Overview", item.OverView);
                        cmd.Parameters.AddWithValue("@Reg_Date", item.Reg_Date);

                        insRes += cmd.ExecuteNonQuery();        // 들어간 갯수만큼 결과가 나올 것
                    }

                    if (list.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장성공♡");
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장 오류\n 관리자에게 문의하세요♥");
                    }

                }
                catch (Exception ex)
                {
                    await Commons.ShowMessageAsync("오류", $"DB 저장 오류\n {ex.Message}♥");

                }
            }
            */
            #endregion


            try
            {
                // DB 연결 확인이 먼저
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    // 
                    var query = @"INSERT INTO  [FavMovieItem]
                                               ( [Id]
                                               , [Title]
                                               , [Original_Title]
                                               , [Release_date]
                                               , [Original_Language]
                                               , [Adult]
                                               , [Popularity]
                                               , [Vote_Average]
                                               , [Poster_Path]
                                               , [OverView]
                                               , [Reg_Date])
                                         VALUES
                                               ( @Id
                                               , @Title
                                               , @Original_Title
                                               , @Release_date
                                               , @Original_Language
                                               , @Adult
                                               , @Popularity
                                               , @Vote_Average
                                               , @Poster_Path
                                               , @OverView
                                               , @Reg_Date)";


            var insRes = 0;
            foreach (MovieItem item in GrdResult.SelectedItems)     // openAPI로 조회된 결과라서 MovieItem
            {
                SqlCommand cmd = new SqlCommand(query, conn);   // 여기 안에 들어가야함

                // 파라미터명, 아이템 속성명, 필드명 맞추면 이렇게 편함
                //SqlParameter prmId = new SqlParameter("@Id", item.Id);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.Parameters.AddWithValue("@Title", item.Title);
                cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                cmd.Parameters.AddWithValue("@Release_date", item.Release_date);
                cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                cmd.Parameters.AddWithValue("@Adult", item.Adult);
                cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                cmd.Parameters.AddWithValue("@Overview", item.OverView);
                cmd.Parameters.AddWithValue("@Reg_Date", DateTime.Now);

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

                    //var result = cmd.ExecuteScalar();

                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장 오류\n {ex.Message}♥");

            }
        }

        private async void BtnFav_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            TxtMovieName.Text = string.Empty;

            List<FavMovieItem> list = new List<FavMovieItem>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"SELECT Id
                                       , Title
                                       ,  Original_Title
                                       ,  Release_date
                                       ,  Original_Language
                                       ,  Adult
                                       ,  Popularity
                                       ,  Vote_Average
                                       ,  Poster_Path
                                       ,  Overview
                                       ,  Reg_Date
                                    FROM FavMovieItem
                                   ORDER BY Id ASC";
                    var cmd = new SqlCommand(query, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavMovieItem");

                    foreach (DataRow dr in dSet.Tables["FavMovieItem"].Rows)
                    {
                        list.Add(new FavMovieItem
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            Title = Convert.ToString(dr["title"]),
                            Original_Title = Convert.ToString(dr["title"]),
                            Release_date = Convert.ToString(dr["Release_date"]),
                            Original_Language = Convert.ToString(dr["Original_Language"]),
                            Adult = Convert.ToBoolean(dr["Adult"]),
                            Popularity = Convert.ToDouble(dr["Popularity"]),
                            Poster_Path = Convert.ToString(dr["Poster_Path"]),
                            Vote_Average = Convert.ToDouble(dr["Vote_Average"]),
                            OverView = Convert.ToString(dr["OverView"]),
                            Reg_Date = Convert.ToDateTime(dr["Reg_Date"])
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

        private async void BtnDelFav_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", $"즐겨찾김나 삭제할 수 있습니다♥");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0) // 아무선택도 안 됨
            {
                await Commons.ShowMessageAsync("오류", $"삭제할 영화를 선택하세요♥");
                return;
            }

            try   // 삭제
            {
                using (SqlConnection conn =  new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = "DELETE FROM FavMovieItem WHERE Id = @Id";
                    var delRes = 0;

                    foreach (FavMovieItem item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
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
            catch ( Exception ex )
            {
                await Commons.ShowMessageAsync("오류", $"DB 삭제 오류\n{ex.Message} ♥");

            }

            BtnFav_Click(sender, e);    // 즐겨찾기 보기 이벤트핸들러를 한 번 실행
        }
    }
}

