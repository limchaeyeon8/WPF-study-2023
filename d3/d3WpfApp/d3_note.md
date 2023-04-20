#### NOTE3

## 데이터바인딩 - 변환기
- 변환기(컨버터); 실제데이터와 다르게 만들어주기 위해 사용하는 것
- ex> 섭씨온도 데이터 들어있는 것을 화씨로 바꿈(국제기준변환)

- namespace wp05_bikeShop.Logics 네임스페이스에 MyConverter 클래스 생성
- internal class MyConverter : IValueConverter
    - using System.Windows.Data; 추가
    - 인터페이스 구현
```cs
    internal class MyConverter : IValueConverter
    {
        // 대상에다 표현할 때 값을 변환 ((OneWay))
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // 대상값이 바뀌어서 원본(소스)의 값을 변환, 표현 ((TwoWay))
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
```
- 원웨이
- public object Convert - 대상에다 표현할 때 값을 변환 ((OneWay))
        
```cs
<TextBox x:Name="TxtSource" Text="150"/>
<TextBox Text="{Binding Text, ElementName=TxtSource}"/>
```
- 위를 바꾸면 아래도 바뀌지만 아래를 바꾸면 안 바뀜
    - 클래스 생성 파일 > Converter 메소드에서 return int.Parse(value.ToString()) + "km/h"; 
    - >>> 위의 텍스트박스는 150, 아래의 텍스트 박스는 150km/h 로 출력됨

- 투웨이
- public object ConvertBack - 대상값이 바뀌어서 원본(소스)의 값을 변환, 표현 ((TwoWay))
    - Binding 내부에 Mode=TwoWay 추가
    - 아래를 바꾸면 위의 텍스트도 바뀜

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
#### DataContext
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

- ComboBox x:Name="CtlCars" 이름 설정 > 
```cs
// 코드비하인드에서 만든 데이터(DB, excel,,,)를 xaml에 있는 컨트롤에 Binding하려면
this.DataContext = cars;    // 중요!!!! // 페이지 전체에 바인딩하기 위한 데이터 연동
CtlCars.DataContext = cars;     // 새로 만든 콤보박스 아래에만!
```

### 데이터바인딩 요약
- ItemTemplate              - 각 항목의 모양*
- ItemContainerStyle        - 요소 효과
- ItemPanel                 - 
- Template                  - 

## 데이터바인딩 - INotifyPropertyChanged 
- 사용자가 속성 업데이트(변경)하면 바인딩된 컨트롤도 업데이트 되어야 할 때 이벤트 발생
    - 데이터 객체에 속성 변경 이벤트가 필요

이게 있어야 데이터 제대로 핸들링할 수 있음!

## (+) 
- DI; Dependent Injection   - 
- IoC; Inversion of Control - 컨트롤 제어

- 스크롤 - ScrollViewer.VerticalScrollBarVisibility="Visible" Height="(지정숫자)"

- 랜덤컬러
```cs
 // 리스트박스에 바인딩하기 위한 Car 리스트
            var cars = new List<Car>();
            var rand = new Random();        // (+) 랜덤컬러
            for (int i = 0; i < 10; i++)
            {
                
                cars.Add(new Car()
                {
                    Speed = i * 10 ,
                    Colour = Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))

                });
                
            }
```
