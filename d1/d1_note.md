#### NOTE1

# WPF; Windows Presentation Foundation
- 기존 WinForms 단점 보완 UI 개발 프레임워크
- XAML ( eXtensible Application Markup Language ) 이용하여 애플리케이션 프로그래밍을 위한 선언적 모델을 제공
    - 모바일, UWP, WPF, ... 다 개발 가능
    - VIsual Studio도 WPF로 만듦

- .NET Framework   - WPF, Window Forms, ASP.NET 
- .NET Core        - UWP, ASP.NET Core
- Mono for Xamarin - 모바일(Android, iOS, macOS)

## WinForms / WPF 장점 비교
#### WinForms 장점
- 오래된 기술 => 많은 테스트, 사용
- 상용, 무료 서드파티 컨트롤 이미 많이 존재
- 혼자 WinForms 디자인부터 많은 작업을 해야할 때 WPF 보다 WinForms가 더 빨리 개발 가능

#### WPF 장점
- 비교적 최신 기술 => 최근 기준들에 부합
- 디자인 화면은 xml 기반 - 소스를 비하인드로 넘기는 것은 Android 등과 매핑됨
    - (+) UI 렌더용 HTML 페이지와 비하인드 파일 분리
    - (+) UI와 코드를 완벽하게 분리할 수
        => 소스 디버그, 유지, 보수, 관리 이점
- xaml 디자인에 익숙해지면
    => Android 개발, Qt디자인에도 동일하게 적용 가능
- MS에서 출시하는 새로운 응용프로그램은 WPF로 개발
    (ex> 2015 버전이상의 Visual Studio)
- 새로운 컨트롤을 구매하지 않아도 직접 제작 가능(단, 디자인 감각 요)
- 세련된 UI 디자인 / 많은 무료 컴포넌트 존재
- XAML
    - GUI 제작, 편집에 용이
    - 디자이너-프로그래머 업무 분리하여 작업
- 하드웨어 가속 장치 사용 => 더 나은 성능 제공
- Windows 기반 프로그램과 Web 기반 프로그램 사용자 인터페이스 제작가능 (Silverlight / XBAP)

## WPF 특징
- 개발자-디자이너 각자의 개발툴을 가지고 서로간에 작업을 공유
- 디자이너의 작업이 개발자의 비즈니스 코드에 영향을 미치지 않도록 가능

- 작업순서 (딪 - 디자이너 / 갭 - 개발자)
    - (딪)골격 디자인 -> (갭)비즈니스 로직 -> (딪)디자인 개선 <--> (갭)디버깅

# WPF 프로젝트 생성
- WPF앱(.NET Framework)
- (+) 도구 > 옵션 > 프로젝트 위치 설정

- 클래스
MainWindow.xaml.cs
```cs
public partial class MainWindow : Window
``` 
partial class - 복수의 장소에서 class를 정의할 수 있도록 지원
↓
MainWindow.xaml
```cs
< Window x:Class="wp02_simpleControls.MainWindow"
```
#### App.xaml; 메인메소득 하는 일을 대신 해줌
    - StartupUri="MainWindow.xaml" 지우면 실행은 되나, 아무것도 안 뜸

## 컨트롤 추가 : XAML 화면에서 컨트롤 추가
    - 도구상자에서 컨트롤 드래그
    - XML 요소를 코딩하여 추가

### 기본 컨트롤
1. MainWindow.xaml 클릭 
2. Grid 태그를 StackPanel로 변경 
3. 컨트롤 코드 입력 
4. 솔루션 빌드 
5. 실행

- Winforms : MessageBoxIcon.Error -> WPF : MessageBoxImage.Error
- WPF는 Program.cs가 없음 / 없어도 실행 가능 => 대신 App.xaml

#### 추가한 도구
StackPanel>
- TextBlock
- Label
- PasswordBox
- Slider
- ProgressBar
- Image
- MediaElement

## 

