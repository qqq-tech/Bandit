# Bandit
[![Build status](https://ci.appveyor.com/api/projects/status/0p305aob0r46febp?svg=true)](https://ci.appveyor.com/project/junimiso04/bandit)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/junimiso04/Bandit)

당신의 멘탈과 더불어 가정의 화목함을 지켜줄 유일한 친구, 노상강도 입니다.

## Overview
밴딧은 네이버 밴드를 이용한 원격 수업 중 출석체크의 피곤함을 덜어주기 위해 장인의 손길로 한땀한땀 만들어진 도구입니다. 밴딧은 Selenium 프로젝트와 Chrome을(를) 사용하여 출석을 자동화해줍니다.

### Description
밴딧의 동작 과정을 살펴보기에 앞서 밴딧에는 두가지의 동작 모드가 있습니다. 하나는 일정 시간(Tick)마다 동작 시퀀스를 실행하는 모드이며, 다른 하나는 지정된 시간에만 동작 시퀀스를 실행하는 모드입니다. 이 두 모드는 프로그램 내에서 사용자 지정이 가능하며 이를 위한 설정창까지 마련되어 있습니다.

밴딧의 자세한 동작 과정은 아래와 같습니다.

```
 # 작업 전 처리 #
 
 1. 사용자의 컴퓨터에 설치된 크롬과 크롬 드라이버의 버전 호환 여부를 검사합니다. 
 2. 밴드 로그인을 진행합니다.
 
 # 동작 시퀀스 #
 
 1-1. 새 글 피드의 내용을 가져옵니다. 
 1-2. 새 글 피드에 존재하는 게시글들의 주소를 가져옵니다. 
 1-3. 이미 출석 작업이 완료된 게시글들의 목록과 새롭게 가져온 게시글들을 비교하여 아직 출석 작업이 진행되지 않은 글들을 골라냅니다.
 1-4. 골라낸 게시글들에서 출석 체크를 담당하는 input 태그를 감지하여 클릭함으로서 출석 작업을 완료합니다.
 
```

## TO DO
 * 한 게시물에서 여러개의 출석 체크를 처리하기.
 * 여러 게시물이 동시에 입력될 경우에 제일 최근에 작성된 게시물의 출석만 처리되는 오류를 해결하기.
 * 로그인과 로그아웃 기능, 새 글 피드 불러오기 기능, 출석 체크 기능 등의 메소드들을 비동기 처리하기.
 * 코드 최적화 및 주석 정리하기.
 * 추가적인 예외 경우를 찾아낸 후 처리하기.

## License
본 레포지토리의 모든 소스 코드는 MIT 라이선스에 의거하여 자유롭게 사용이 가능합니다. (단, 본 레포지토리에서 이용된 제 3자 오픈소스 프로젝트들은 각 프로젝트에 부여된 라이선스를 기반으로 사용이 가능합니다.)

 * __DotNetSeleniumExtras.WaitHelpers__ - Unknown License
 * __DotNetZip.Semverd__ - Microsoft Public License(MS-PL)
 * __HtmlAgilityPack__ - MIT License
 * __MaterialDesignThemes__ - MIT License
 * __MaterialDesignColors__ - MIT License
 * __Newtonsoft.Json__ - MIT License
 * __Selenium.Support__ - Apache License Version 2.0
 * __Selenium.WebDriver__ - Apache License Version 2.0
 * __Selenium.WebDriver.Chromedriver__ - Unlicense
 
## Contact
본 레포지토리의 소스 코드에 대한 문의사항이나 릴리즈 된 프로그램에 대한 버그 정보를 제공하시려면 GitHub Issues(이)나 아래의 이메일을 이용해주시기 바랍니다.

 * E-Mail : junimiso04@naver.com
