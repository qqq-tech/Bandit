# Bandit
[![GitHub](https://img.shields.io/github/license/junimiso04/Bandit?style=flat-square)](https://github.com/junimiso04/Bandit/blob/master/LICENSE)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/junimiso04/Bandit?logo=GitHub&logoColor=White&style=flat-square)](https://github.com/junimiso04/Bandit/releases/latest)

```
※ NOTICE
2020년 9월 15일 부로 더이상의 추가 업데이트는 제공되지 않을 예정입니다.
만약 네이버 밴드 시스템의 업데이트로 인해 프로그램을 사용할 수 없게 된다면
본 레포지토리를 포크한 후 직접 소스코드를 수정해서 사용해주시기 바랍니다.
```

| |master|develop|
|------|------|------|
|Code Quality(Codacy)|[![Codacy branch grade](https://img.shields.io/codacy/grade/b3afd7c34f1d4cc8a8e8092015731f92/master?logo=Codacy&logoColor=white&style=flat-square)](https://app.codacy.com/manual/junimiso04/Bandit/dashboard)|[![Codacy branch grade](https://img.shields.io/codacy/grade/b3afd7c34f1d4cc8a8e8092015731f92/develop?logo=Codacy&logoColor=white&style=flat-square)](https://app.codacy.com/manual/junimiso04/Bandit/dashboard)|
|Build State(AppVeyor)|[![AppVeyor branch](https://img.shields.io/appveyor/build/junimiso04/Bandit/master?logo=AppVeyor&logoColor=white&style=flat-square)](https://ci.appveyor.com/project/junimiso04/bandit)|[![AppVeyor branch](https://img.shields.io/appveyor/build/junimiso04/Bandit/develop?logo=AppVeyor&logoColor=white&style=flat-square)](https://ci.appveyor.com/project/junimiso04/bandit)|

당신의 멘탈과 더불어 가정의 화목함을 지켜줄 유일한 친구, 노상강도 입니다.

## Overview
밴딧은 네이버 밴드를 이용한 원격 수업 중 출석체크의 피곤함을 덜어주기 위해 만들어진 도구입니다. 밴딧은 Selenium 프로젝트와 Chrome을(를) 사용하여 출석을 자동화해줍니다.

## Screenshots
![Bandit](./images/guide.png)

## Install
[GitHub Releases](https://github.com/junimiso04/Bandit/releases)을(를) 통해 Bandit의 최신 릴리즈를 다운로드하실 수 있습니다.
 * Bandit은(는) Windows 7 서비스 팩 1 이후 버전의 Windows 환경에서만 사용할 수 있습니다.
 * 32비트 시스템에서는 **Bandit_(릴리즈 버전)_i386.zip**을(를), 64비트 시스템에서는 **Bandit_(릴리즈 버전)_amd64.zip**을(를) 다운로드 해주시기 바랍니다.
 * 프로그램을 사용하시려면 배포 패키지의 압축을 해제하시고 __Bandit.exe__(을)를 실행하십시오.
 
만약 프로그램이 실행되지 않거나, 런타임 도중 오류가 발생한다면 아래의 사항들을 점검해주시기 바랍니다.
 * 사용자의 시스템에 맞는 패키지를 다운로드 받지 않았을 경우.
 * 패키지의 압축을 풀지 않고 프로그램을 실행했을 경우.
 * 패키지에 동봉되어 있는 DLL(Dynamic Link Library)의 일부가 손실되었을 경우.
 * 사용자의 컴퓨터에 .NET Framework 4.8 런타임이 설치되어 있지 않을 경우.

## Usage
※ 밴딧을 사용하기 위해서는 사용자의 컴퓨터에 Chrome 브라우저가 설치되어 있어야합니다. 크롬이 설치되어 있지 않을 경우에는 이 [링크](https://www.google.com/intl/en/chrome/)를 통해 크롬을 설치할 수 있습니다.

1. 크롬을 실행한 뒤, 주소창에 __chrome://version/__ 을(를) 입력하여 사용자의 컴퓨터에 설치된 크롬의 버전을 확인합니다.
2. 밴딧을 실행한 후 설정창으로 들어가 앞서 확인한 버전과 맞는 크롬 드라이버를 설치해주시기 바랍니다. (만약, 설치가 되지 않는 경우에는 밴딧을 관리자 권한으로 실행시켜주시기 바랍니다.)
3. 자동으로 피드를 갱신할 지, 지정된 시간에만 피드를 갱신할 지를 선택하고 사용자의 취향에 따라 추가적인 프로그램 설정을 합니다. 더 자유로운 설정 조정은 __'밴딧 설치 경로'/data/settings.json__ 의 수동 편집을 통해 가능합니다. (단, 자동 피드 갱신 모드에서 갱신 간격이 극도로 짧을 경우에는 비정상적인 이용으로 인해 밴드 계정이 차단될 가능성이 있습니다.)
4. 로그인 버튼을 누르고 네이버 밴드 이메일 계정을 이용하여 로그인합니다. 밴딧을 처음 사용하시는 경우 문자 메시지를 이용한 PIN 인증이 필요할 수 있습니다.
5. 밴딧 작업 버튼을 클릭하면 자동 출석 감지 작업이 시작됩니다.



## Description
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

## Open-source Software Notice
본 레포지토리의 모든 소스 코드는 MIT 라이선스에 의거하여 자유롭게 사용이 가능합니다. (단, 본 레포지토리에서 사용된 제 3자 오픈소스 프로젝트의 경우 각 프로젝트의 고유 라이선스를 따라주시기 바랍니다.) 

 * __DotNetSeleniumExtras.WaitHelpers__ - Unknown License
 * __DotNetZip.Semverd__ - Microsoft Public License(MS-PL)
 * __HtmlAgilityPack__ - MIT License
 * __markdig__ - BSD 2-Clause "Simplified" License
 * __MaterialDesignThemes__ - MIT License
 * __MaterialDesignColors__ - MIT License
 * __Newtonsoft.Json__ - MIT License
 * __Selenium.Support__ - Apache License Version 2.0
 * __Selenium.WebDriver__ - Apache License Version 2.0
 * __Selenium.WebDriver.Chromedriver__ - Unlicense
 
## Contributers
__Main Developer(+Maintainer)__ - June(aka junimiso04)

## Contact
본 레포지토리의 소스 코드에 대한 문의사항이나 릴리즈 된 프로그램에 대한 버그 정보를 제공하시려면 GitHub Issues(이)나 아래의 이메일을 이용해주시기 바랍니다.

 * E-Mail : junimiso04@naver.com
