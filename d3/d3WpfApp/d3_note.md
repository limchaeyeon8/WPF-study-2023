#### NOTE3

## 데이터바인딩 - 변환기
- 변환기(컨버터); 실제데이터와 다르게 만들어주기 위해 사용하는 것
- ex> 섭씨온도 데이터 들어있는 것을 화씨로 바꿈(국제기준변환)

- namespace wp05_bikeShop.Logics 네임스페이스에 MyConverter 클래스 생성
- internal class MyConverter : IValueConverter
    - using System.Windows.Data; 추가
    - 인터페이스 구현
```cs
 // MyConverter.cs
    internal class MyConverter : IValueConverter
    {
        // 대상에다 표현할 때 값을 변환 ((OneWay))
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) + "km/h";
        }

        // 대상값이 바뀌어서 원본(소스)의 값을 변환, 표현 ((TwoWay))
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) * 3; // Logics 네임스페이스에서 만들어진 클래스 바로 못 씀 
        }
    }
```
- 원웨이
- public object Convert - 대상에다 표현할 때 값을 변환 ((OneWay))
        
```cs
<TextBox x:Name="TxtSource" Text="150"/>
<TextBox Text="{Binding Text, ElementName=TxtSource}"/>
```
- 위 텍스트 박스를 바꾸면 아래도 바뀌지만 아래를 바꾸면 안 바뀜
    - 클래스 생성 파일 > Converter 메소드에서 return int.Parse(value.ToString()) + "km/h"; 
    - -> 위의 텍스트박스는 150, 아래의 텍스트 박스는 150km/h 로 출력됨

- 투웨이
- public object ConvertBack - 대상값이 바뀌어서 원본(소스)의 값을 변환, 표현 ((TwoWay))
    - Binding 내부에 Mode=TwoWay 추가
    - 아래 텍스트 박스를 바꾸면 위의 텍스트도 바뀜

- 클래스 사용을 위해 외부(SupportPage.xaml)에서 추가 > 다시빌드
```cs
    <Page.Resources>
        <Logics:MyConverter x:Key="myConv"/>
    </Page.Resources>
```

- 클래스 사용
```cs
<TextBox Text="{Binding Text, ElementName=TxtSource, Mode=OneWay, Converter={StaticResource myConv}}" />
```


## 데이터바인딩 - 데이터 컬렉션
#### DataContext ((중요!))
- 하나의 공용 소스에 여러 개의 속성을 바인딩하는 경우 DataContext 속성을 사용
- 이 속성은 모든 데이터바인딩된 속성이 공용 소스를 상속받게 되는 범위를 설정하는 편리한 방법 제공
- Page, Grid, Label, ListBox(<-이름지정해서 사용),...의 속성 또는 this.DataContext로 사용가능

-  리스트박스에 바인딩하기 위한 Car 리스트
```cs
car cars = new List<Car>();
for (...){...}

// 코드비하인드에서 만든 데이터(DB, excel,,,)를 xaml에 있는 컨트롤에 Binding하려면
this.DataContext = cars;    // 중요!!!!
```

- StackPanel 아래의 ComboBox x:Name="CtlCars" 이름 설정 > 
```cs
// 코드비하인드에서 만든 데이터(DB, excel,,,)를 xaml에 있는 컨트롤에 Binding하려면
this.DataContext = cars;    // 중요!!!! // 페이지 전체에 바인딩하기 위한 데이터 연동
CtlCars.DataContext = cars;     // 새로 만든 리스트/콤보박스 아래에만!
```


### 데이터바인딩 요약
- ItemTemplate              - 각 항목의 모양*
- ItemContainerStyle        - 요소 효과
- ItemPanel                 - 항목 레이아웃
- Template                  - 목록 주위(테두리, 배경, 스크롤바,...)


## 데이터바인딩 - INotifyPropertyChanged 
- 사용자가 속성 업데이트(변경)하면 바인딩된 컨트롤도 업데이트 되어야 할 때 이벤트 발생
    - 데이터 객체에 속성 변경 이벤트가 필요
    - 이게 있어야 데이터 제대로 핸들링할 수 있음!


```cs
 // ProductManagementPage.xaml.cs
private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        DgrProducts.ItemsSource = factory.FindProducts(TxtSearch.Text);
    }
```
- 검색어 입력하는 TxtSearch TextBox에 텍스트를 입력하여 검색하면 데이터그리드의 DgrProducts에서 찾을 수 있음

```cs
        // ProductManagementPage.xaml
        // 데이터 
        <DataGrid x:Name="DgrProducts" Grid.Row="2" Grid.Column="0" Margin="50, 15, 15, 50"/>

        <GroupBox Grid.Row="2" Grid.Column="1" Margin="10, 15, 50, 50" Header="제품정보" 
                  DataContext="{Binding SelectedItem, ElementName=DgrProducts}">
            <StackPanel>
                <Label Content="제품명" FontWeight="Bold" FontSize="14" Margin="0, 10, 0, 0"/>
                <TextBox FontSize="13" Margin="5, 0" Text="{Binding Title}"/>

                <Label Content="가격" FontWeight="Bold" FontSize="14" Margin="0, 5, 0, 0"/>
                <TextBox FontSize="13" Margin="5, 0" Text="{Binding Price}"/>

                <Label Content="색상" FontWeight="Bold" FontSize="14" Margin="0, 5, 0, 0"/>
                    <TextBox FontSize="13" Margin="5, 0" Text="{Binding Color}"/>

                <Border Background="{Binding Color}" Height="10" Margin="5, 0"/>

                <Label Content="코드" FontWeight="Bold" FontSize="14" Margin="0, 5, 0, 0"/>
                        <TextBox FontSize="13" Margin="5, 0" Text="{Binding Reference}"/>
            </StackPanel>
        </GroupBox>
```
<img src="https://raw.githubusercontent.com/limchaeyeon8/WPF-study-2023/main/d3/d3WpfApp/wp05_bikeShop/PrdMng.png" width="500" />
- 제품정보 그룹 박스 내에서 제품명/가격/색상/코드 값 수정 가능
    - ProductsFactory.cs > Product 클래스의 OnProperyChanged 이벤트

```cs
public class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)   // 메서드 생성
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
```
    - Notifier.cs > 클래스에 INotifyPropertyChanged 인터페이스 구현
        - 소스의 객체의 속성값 변경 통지를 위해 INotifyPropertyChanged 인터페이스를 상속 받음
    - PropertyChanged 이벤트 정의
        - 첫 번째 인자: this
        - 두 번째 인자: PropertyChangedEventArgs 타입의 객체
            - string 타입의 PropertyName 프로퍼티 정의 => 프로퍼티 구별 위해 사용
            - PropertyChangedEventArgs를 new 한다
        - 데이터 값 수정 시 바인딩 된 클래스의 OnProperyChanged 메서드로 인해 데이터변경이 바로 적용


# 컨트롤 디자인
## 컨트롤 템플릿
- 거의 모든 WPF 컨트롤에 Template 속성 존재
- 컨트롤에 새로운 모양 적용하려면 ControlTemplate 인스턴스 할당

## 템플릿 바인딩 (TemplateBinding)
- 버튼이 기존 갖고 있는 콘텐트 가져오겠다는 의미
- 바인딩 수정 시 다시빌드 필수

<img src="https://raw.githubusercontent.com/limchaeyeon8/WPF-study-2023/main/d3/d3WpfApp/wp05_bikeShop/temBin.png" width="400" />


## 컨트롤 공유화
- 컨트롤에 대한 템플릿 작성 => 애플리케이션 전체에 사용 가능
- 템플릿 코드 복제하는 것은 유지보수 불가능하기 때문에 리소스 사용

- 리소스 저장 장소 두 군데
    - 화면
    - 애플리케이션

- 모든 페이지에 적용하고 싶다면 App.xaml에 입력

```cs
 // App.xaml
    <Application.Resources>
        <Button x:Key="button" Content="전송" Background="{x:Null}" Foreground="CornflowerBlue"/>
        <SolidColorBrush x:Key="accentBrush" Color="PeachPuff"/>
    </Application.Resources>
```

위 코드를 BrushDictionary.xaml로 이동 후 App.xaml에 소스 지정

```cs
 // App.xaml
    <Application.Resources>
        <ResourceDictionary Source="/BrushDictionary.xaml"/>
    </Application.Resources>
```

<img src="https://raw.githubusercontent.com/limchaeyeon8/WPF-study-2023/main/d3/d3WpfApp/wp05_bikeShop/UseStaticResource.png" width="400" />
- 버튼 / 백그라운드에 리소스 사전의 Static Resource backgroundBrush / accentBrush / originButton 적용

```cs
 // BrushDictionary.xaml - 리소스 사전
    // backgroundBrush 리소스
    <LinearGradientBrush x:Key="backgroundBrush">
        <GradientStop Color="#cffcff" Offset="0"/>
        <GradientStop Color="#d7ccff" Offset="1"/>
    </LinearGradientBrush>

    // accentBrush 리소스
    <SolidColorBrush x:Key="accentBrush" Color="CornflowerBlue"/>

    // originButton 리소스
    <Style x:Key="originButton" TargetType="{x:Type Button}">
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type Button}">
                    <Grid>
                        <Rectangle Fill="PeachPuff" Stroke="Thistle" StrokeThickness="3"
                                   RadiusX="12" RadiusY="5"/>
                        <Label Content="{TemplateBinding Content}" FontSize="15" Foreground="#9fbcf4" 
                               HorizontalAlignment="Center" VerticalAlignment="Center"/>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
```
- 스타일 사용 - 여러 컨트롤의 배경을 설정할 경우 리소스 없이 만들 수도 있으나 리소스로 작업하는 것이 간편


## (+) 
- DI; Dependent Injection   - 의존관계를 외부에서 결정(주입)해주는 것
- IoC; Inversion of Control - 컨트롤 제어; 외부 라이브러리의 코드가 프로그래머가 작성한 코드를 호출

- 스크롤 - ScrollViewer.VerticalScrollBarVisibility="Visible" Height="(지정숫자)"

- 랜덤컬러
```cs
 // TestPage.xaml.cs
 // 리스트박스에 바인딩하기 위한 Car 리스트
private void InitCar()
{
    var cars = new List<Car>();
    var rand = new Random();        // (+) 랜덤컬러
    for (int i = 0; i < 10; i++)    // 스피드: 0, 10, 20, 30, ... , 90
    {    
        cars.Add(new Car()
        {
            Speed = i * 10 ,
            Colour = Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
        });
    }
}
```

<img src="https://raw.githubusercontent.com/limchaeyeon8/WPF-study-2023/main/d3/d3WpfApp/wp05_bikeShop/RandColLabel.png" width="500" />


- 템플릿 정의에 TargetType ControlTemplate.이 포함된 경우 ContentPresenter 속성 필요 (이런것도 있다~ 보고 넘어감)

<img src="https://raw.githubusercontent.com/limchaeyeon8/WPF-study-2023/main/d3/d3WpfApp/wp05_bikeShop/contenPresenter.png" width="600" />


- 델리게이트; 메소드를 참조하는 변수
- 프로퍼티; 속성이란 의미
    - 프로퍼티를 사용하게 되면 => 속성 값을 반환하거나 새 값을 할당할 수
- 인스턴스; 객체 지향 프로그래밍(OOP)에서 클래스(class)에 소속된 개별적인 '객체'
- 객체(오브젝트)의 인스턴스; 데이터베이스나 SGA, 백그라운드 프로세스등 광범위한 컴퓨터시스템 자원의 접근에 할당된 물리 메모리의 일부


#### 윈도우 사이즈 및 타이틀 수정불가 오류 잡기
- 각 Page에서 WindowTitle="바이크샵" 입력하는 것으로 메인윈도우에서 입력하는 게 아니더라도 타이틀 생성 가능
- 하지만 여전히 윈도우 사이즈가 변경되지 않음
- App.xaml의 StarupUri가 MenuPage.xaml로 돼있는 것을 발견 (시작페이지 지정에 문제가 있었음)
    - MainWindow.xaml로 변경
    - MainWindow.xaml에서 Frame 속성에 Source="MenuPage.xaml" 추가 
- 결론 : App.xaml - StarupUri가 Page가 되면 안 됨!!!
