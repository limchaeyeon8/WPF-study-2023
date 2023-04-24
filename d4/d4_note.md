#### NOTE4

# MahApps.Metro

1. NuGet 패키지 설치
    - MAhApps.Metro
    - MAhApps.Metro.IconPacks

2. App.xaml에 리소스 딕셔너리 추가
- "https://mahapps.com/docs/guides/quick-start"
    - MahApps build-in styles and themes -> ResourceDictionary부터 아래로
    - 빌드

3.  네임스페이스 추가
```cs
 // MainWindow.xaml
<mah:MetroWindow 
        xmlns:mah="http://metro.mahapps.com/winfx/xaml/controls"
```

4. MainWindow.xaml에서 Window ---> mah:MetroWindow

5. MainWindow.xaml.cs에서 Window ---> MetroWindow


### 테마(Theme). 타이틀바 (accent) 변경
    - Light.Blue -> Dark.Olive

<img src="https://raw.githubusercontent.com/limchaeyeon8/WPF-study-2023/main/d4/d4WpfApp/1_LightBlue.png" width="500" />

```cs
ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Olive.xaml" 
```

### 아이콘 사용
```cs
 // MainWindow.xaml
<mah:MetroWindow 
    xmlns:iconPacks="http://metro.mahapps.com/winfx/xaml/iconpacks"
```

- 버튼 아이콘 사용

<img src="https://raw.githubusercontent.com/limchaeyeon8/WPF-study-2023/main/d4/d4WpfApp/2_button_icon.gif" width="500" />

```cs
    <mah:MetroWindow.LeftWindowCommands>
        <mah:WindowCommands>
            <Button Click="LaunchGitHubSite" ToolTip="Github site">
                <iconPacks:PackIconModern Kind="SocialGithubOctocat" />
            </Button>
        </mah:WindowCommands>
    </mah:MetroWindow.LeftWindowCommands>
```

- 아이콘 속성 추가

<img src="https://raw.githubusercontent.com/limchaeyeon8/WPF-study-2023/main/d4/d4WpfApp/3_icon_set.png" width="500" />

```cs
<mah:MetroWindow.IconTemplate>
        <DataTemplate>
            <iconPacks:PackIconModern Kind="YoutubePlay" 
                                      Margin="10,4,0,0"
                                      Width="20" Height="20" 
                                      Foreground="White"/>
        </DataTemplate>
    </mah:MetroWindow.IconTemplate>
```


# MVVM

### MVC 패턴
- Model-View-Controller

## Model-View-ViewModel(<=컨트롤러)
- UI 및 비UI 코드를 분리하기 위한 UI 아키텍처 디자인 패턴
- 개발자와 디자이너의 분리
    - 디자이너 View(인터페이스)만 작업
    - 데이터처리 Model(통신담당)
    - 개발자 ViewModel(데이터 주고받는 연결자)

### DataBinding 다이어그램
#### 바인딩 모드
- One Way to Source
    - 타켓 → 바인딩 오브젝트 → 소스
- One Way
    - 타켓 ← 바인딩 오브젝트 ← 소스
- Two Way
    - 타켓 ↔ 바인딩 오브젝트 ↔ 소스


# Personal Info App 프로젝트 생성 - MVVM View

### (+)
- 솔루션 폴더(가상) 생성
    - 탐색기에선 보이지 않음
    - 프로젝트 관리
- 동일 솔루션 내 타 프로젝트에서 사용한 NuGet 패키지 사용
    - 솔루션용 NuGet 패키지 관리 -> 설치됨 -> MahApps.Metro / MahApps.Metro.IconPacks
    - -> 프로젝트 선택 후 설치

### MahApps.Style 

- Button 디자인

```cs
<Button Style="{StaticResource MahApps.Styles.Button.Circle}" />
```

- Label 디자인

```cs
<Lablel Style="{StaticResource MahApps.Styles.Label}" />
```

### DatePicker

```cs
<DatePicker Grid.Column="1" Grid.Row="3" FontSize="12"
            VerticalAlignment="Center" Margin="5"
            mah:TextBoxHelper.Watermark="생일을 선택하세요"
            mah:TextBoxHelper.UseFloatingWatermark="True"
            mah:TextBoxHelper.ClearTextButton="True" />
```

## Logics > Commons.cs
### 이메일 (정규표현식) 메서드 
#### Regular Expression; 정규표현식
```cs
    // Regular Expression (정규표현식)
    // 이메일 형식에 맞게 입력하도록 체크(검증체크)
public static bool IsValidEmail(string email)
{
    var strPattern = @"^([0-9a-zA-Z]+)@([0-9a-zA-Z]+)(\.[0-9a-zA-Z]+){1,}$";
    return Regex.IsMatch(email, strPattern);
}
```

#### (+) 핸드폰 번호 정규표현식
- ``` /^01([0|1|6|7|8|9]?)-?([0-9]{3,4})-?([0-9]{4})$/```
    - 시작을 숫자 01로 시작
    - 그 후에 0,1,6,7,8,9 중에 하나가 나올수도 있음
    - 하이픈 - 하나 존재할 수도 있음
    - 숫자 3~4개 이어짐
    - 하이픈 - 하나 존재할 수도 있음
    - 숫자 4개가 이어짐


### 만나이 메서드
```cs
        public static int GetAge(DateTime value)
        {
            // 입력된 날짜로 나이를 계산
            int result;
            if (DateTime.Now.Month < value.Month || DateTime.Now.Month < value.Month &&
                DateTime.Now.Day < value.Day) result = DateTime.Now.Year - value.Year - 1;        // 아직 생일이 안 지남
            else result = DateTime.Now.Year - value.Year;

            return result;
        }
```

### 띠 메서드
```cs
        public static string GetZodiac(DateTime value)
        {
            int reminder = value.Year % 12;
            switch (reminder)
            {
                case 4:
                    return "쥐띠";
                // ~~~
                case 11:
                    return "양띠";
                case 0:
                    return "원숭이띠";
                // ~~~
                case 3:
                    return "돼지띠";
                default:
                    return "--띠"
            }

        }
```


## Models > Person.cs

```cs
internal class Person
    {
        // 외부에서 접근 불가
        //private string firstName;                     // 오토프로퍼티면 지워도 됨
        //private string lastName;
        private string email;
        private DateTime date;

        // 필드 캡슐화
        public string FirstName { get; set; }           // 오토프로퍼티
        public string LastName { get; set; }            // 오토프로퍼티
```
- 이메일 형식에 일치하지 않은 이메일 입력 시 예외처리
```cs
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
```
- 정상 나이 범위에서 벗어나는 생일 입력 시 예외 처리
```cs
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
```

