﻿using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using wp12_finedustCheck.Logics;
using wp12_finedustCheck.Models;

namespace wp12_finedustCheck
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

        #region <김해시 OpenAPI 조회>
        private async void BtnReqRealTime_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://smartcity.gimhae.go.kr/smart_tour/dashboard/api/publicData/dustSensor";
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

                    var dustSensors = new List<DustSensor>();
                    foreach (var sensor in json_array)
                    {
                        
                        dustSensors.Add(new DustSensor
                        {
                            Id = 0,
                            Dev_id = Convert.ToString(sensor["dev_id"]),
                            Name = Convert.ToString(sensor["name"]),
                            Loc = Convert.ToString(sensor["loc"]),
                            Coordx = Convert.ToDouble(sensor["coordx"]),
                            Coordy = Convert.ToDouble(sensor["coordy"]),
                            Ison = Convert.ToBoolean(sensor["ison"]),
                            Pm10_After = Convert.ToInt32(sensor["pm10_after"]),
                            Pm25_After = Convert.ToInt32(sensor["pm25_after"]),
                            State = Convert.ToInt32(sensor["state"]),
                            TimeStamp = Convert.ToDateTime(sensor["timestamp"]),
                            Company_Id = Convert.ToString(sensor["company_id"]),
                            Company_Name = Convert.ToString(sensor["company_name"])
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

        // 검색한 결과 DB (MySQL)에 저장
        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회 함 돌리고 저장하세요♥");
                return;
            }
            
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO dustsensor
                                            
                                        (Name,
                                        Dev_id,
                                        Loc,
                                        Coordx,
                                        Coordy,
                                        Ison,
                                        Pm10_After,
                                        Pm25_After,
                                        State,
                                        TimeStamp,
                                        Company_Id,
                                        Company_Name)
                                        VALUES
                                            
                                        (@Name,
                                        @Dev_id,
                                        @Loc,
                                        @Coordx,
                                        @Coordy,
                                        @Ison,
                                        @Pm10_After,
                                        @Pm25_After,
                                        @State,
                                        @TimeStamp,
                                        @Company_Id,
                                        @Company_Name)";

                    var insRes = 0;
                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is DustSensor)
                        {
                            var item = temp as DustSensor;
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Name", item.Name);
                        cmd.Parameters.AddWithValue("@Dev_id", item.Dev_id);
                        cmd.Parameters.AddWithValue("@Loc", item.Loc);
                        cmd.Parameters.AddWithValue("@Coordx", item.Coordx);
                        cmd.Parameters.AddWithValue("@Coordy", item.Coordy);
                        cmd.Parameters.AddWithValue("@Ison", item.Ison);
                        cmd.Parameters.AddWithValue("@Pm10_After", item.Pm10_After);
                        cmd.Parameters.AddWithValue("@Pm25_After", item.Pm25_After);
                        cmd.Parameters.AddWithValue("@State", item.State);
                        cmd.Parameters.AddWithValue("@TimeStamp", item.TimeStamp);
                        cmd.Parameters.AddWithValue("@Company_Id", item.Company_Id);
                        cmd.Parameters.AddWithValue("@Company_Name", item.Company_Name);

                        insRes += cmd.ExecuteNonQuery();
                        }  
                    }

                    await Commons.ShowMessageAsync("저장", "DB 저장 성공♡");
                    StsResult.Content = $"DB 저장 {insRes} 건 성공♡";

                }
            }
            catch (System.Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장 오류!\n{ex.Message} ♥");

            }

        }

        // DB (MySQL)에서 조회 리스트 뿌리기
        private void CboReqDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 콤보박스에 들어갈 날짜를 DB에서 불러와서
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT DATE_FORMAT(Timestamp, '%Y-%m-%d') AS Save_Date
                                FROM dustsensor
                               GROUP BY 1
                               ORDER BY 1";     // 날짜 순서대로
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet(); 
                adapter.Fill(ds);

                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["Save_Date"]));
                }

                CboReqDate.ItemsSource = saveDateList;
            }
                

            
        }
    }
}
