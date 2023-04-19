#### NOTE2

## 기본 컨트롤2
- Button - 누르고 나가면 무변화
- ToggleButton - 눌린채로 고정
- CheckBox
- RadioButton
    - IsChecked="True"
- Button 자식 StackPanel - 버튼에 이미지 삽입

#### 그리기
- Ellipse - 원,타원 그리기
    - Ellipse.Fill > ImageBrush - 이미지 삽입
- Rectangle - 사각형 그리기 
- Path - 복잡한 도형 그리기 
    - ElipseGeometry

- 도형그리기 옵션
    - Fill   - 채우기 색상
    - Stroke - 테두리 색상
    - StrokeThickness - 테두리 두께

#### View Box : 모든 컨텐츠의 크기조정 가능
    - 많이 쓰이지는 않음
    - Viewbox Stretch="Fill" - 꽉채움

## 레이아웃
- Panel 사용
    - 창 크기 조절 시 컨트롤 크기가 같이 변하지 X 경우
    - 창이 컨트롤들 구성된 크기보다 작아지면 컨트롤들 일부가 가려지는 경우
    - 컨트롤이 페이지 중심에 있는 경우

## Panel 컨트롤
    - Responsive Design; 응답형(반응형) 디자인
        - 애플리케이션 화면을 사용가능한 화면 영역에 맞춰 조정할 수 있음
    - 하나의 컨트롤만 허용하는 여러 개의 컨트롤 표시
    - 사용 가능한 크기에 따라 컨트롤 배치
    
### 크기 할당
- Canvas - 자식 컨트롤 크기 제한X
    -  부모창 사이즈를 넘어가면 컨트롤 영역 잘림
- Grid - 자식 컨트롤 크기 제한
    - 하위 자식이 자신과 같도록 만들어서 부모/자식 크기설정 안 하면 꽉 채움
- StackPanel; 부모가 가진 사이즈를 자식에게 물려주지 않음
    - Orientation="Horizontal" => 컨트롤 일렬정렬
- DockPanel - Orientation 안 먹힘
    - DockPanel.Top/Left/Bottom/Right (상속)
- WrapPanel, UniformGrid는 잘 안 씀
```
                [크기강제]     [사용편의]
Canvas      -      X            디자인뷰   
Grid        -      O            디자인뷰
DockPanel   -      O              XAML
StackPanel  -      O              XAML
WrapPanel   -      O              XAML 
UniformGrid -      O              XAML 
```

- Grid > Grid.RowDefinitions > RowDefinitions       - 열
- Grid > Grid.ColumnDefinitions > ColumnDefinitions - 행
    - Button Grid.Row="1" Grid.Column="2" ==> 2열3행에 Button 배치
    - Height / Width="90" ==> 높이/넓이 지정 픽셀로 고정
    - Height / Width = *, * ==> 남은 높이/넓이에 비례한 1:1 비율
    - ex> 두 개의 열 정의, RowDefinition Height="1*", RowDefinition Height="4*" => 1:4 비율
```cs
<RowDefinition Height="1*" /><RowDefinition Height="4*"/>
```

## 탐색; Navigation
- 이전 화면 되돌아가기 위치기록에서 되돌아가기
- WPF 내 NavigationWindow 기능
- 코드 비하인드 / xaml 상 모두 구현 가능
```cs
// MenuPage.xaml
<Frame NavigationUIVisibility="Visible" />
```

#### 버튼 클릭 시 네비게이트 구현
- MenuPage.xaml > Button 이벤트 추가 ( ex>Click="BtnMenuSupport_Click" )
- MenuPage.xaml.cs > 파셜 클래스 내부에 이벤트 핸들러 추가
```cs
 private void BtnMenuContact_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/ContactPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void BtnMenuSupport_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/SuppoertPage.xaml", UriKind.RelativeOrAbsolute));
        }
```
    - Relative - 상대 URI / Absolute - 절대 URI / RelativeOrAbsolute - URI 종류 정하지 않음

## 새폴더 추가 > 클래스 추가
- 폴더명 - Logics / 클래스명 - Car.cs
- 클래스를 사용할 xaml에서 xmlns:Logics="clr-namespace:wp05_bikeShop.Logics" 추가
    - 클래스를 컴파일을 거쳐 xaml에서 사용 가능한 객체로 만들어주기 위해 매핑(빌드)해줘야 함 (바로 사용 불가)
    - 비하인드 코드는 바로 사용 가능
- 다른 앨리먼트 아래에서 사용


## 데이터 바인딩


## (+)
- URI(Uniform Resouce Identifier)
    - 특정 리소스를 식별하는 통합 자원 식별자
- URL(Uniform Resouce Locator) 
    - 인터넷에서 리소스(웹페이지, 이미지, 비디오 등)의 위치 가리키는 문자열


- 창 안에 창 못 넣음 - 메인윈도우에서 페이지 추가 가능

- 각 없는 버튼 만들기 - CornerRadius
```cs
<Button.Resources>
    <Style TargetType="Border">
        <Setter Property="CornerRadius" Value="10"/>
    </Style>
</Button.Resources>
```

- Grid.RowSpan/ColumnSpan="n" - n개의 열/행 병합

- x:Name="" 의미
    - xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml.Name"

- WPF 묶음주석 : ctrl + K + C