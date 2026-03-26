# Wild Tempest

[Unity] 뱀서라이크 생존 액션 게임

## 프로젝트 개요

와일드 템페스트는 다양한 무기와 액세서리를 습득하며 끝없이 몰려오는 적들을 물리치는 생존형 뱀서라이크 게임입니다. 플레이어는 레벨업 시 아이템을 선택하고, 무기를 진화시키며 보스를 처치해 스테이지를 클리어해야 합니다.

## 담당 기능

### 게임 시스템
- **플레이어 컨트롤**: 가상 조이스틱 기반 이동, 자동 공격 시스템
- **무기 시스템**: 10종 이상의 무기 (검, 도끼, 스태프, 마법서, 마법링, 불꽃 등)
- **아이템 시스템**: 다양한 액세서리 (장갑, 망토, 신발, 시계, 쿼버, 로브 등)
- **레벨업 시스템**: 경험치 획득 시 랜덤 아이템 선택, 무기/액세서리 슬롯 관리
- **진화 시스템**: 특정 조건 만족 시 무기 진화 가능

### 전투 시스템
- **적 스포너**: 스테이지별 다양한 적 생성, 보스전 처리
- **데미지 시스템**: 넉백, 데미지 연출, HP 관리
- **투사체 시스템**: 단일/광역/홈킹 등 다양한 투사체 패턴

### 네트워크 & 서비스
- **Google Play Games Services**: 구글 로그인, 클라우드 저장, 리더보드, 업적
- **인앱결제(IAP)**: Unity IAP 통합, 재화 패키지 구매
- **애드몹(AdMob)**: 리워드 광고 구현

### 시스템 유틸리티
- **오브젝트 풀링**: 성능 최적화를 위한 오브젝트 재사용 시스템
- **어드레서블 에셋**: 에셋 라벨 기반 동적 로딩
- **다국어 지원**: 한국어, 영어 지원

## 기술 스택

- **엔진**: Unity
- **로그인**: Google Play Games Services
- **광고**: Google AdMob
- **결제**: Unity In-App Purchasing
- **애니메이션**: DOTween
- **에셋 관리**: Unity Addressables
- **언어**: C#

## 프로젝트 구조

```
Scripts/
├── System/               # 시스템 관리
│   ├── GameManager.cs    # 게임 전체 관리, 상태 관리
│   ├── DataManager.cs    # 데이터 저장/로드
│   ├── EnemySpawner.cs   # 적 스폰 시스템
│   ├── LevelUpManager.cs # 레벨업 아이템 선택
│   ├── ObjectPoolManager.cs # 오브젝트 풀링
│   ├── AssetManager.cs   # 어드레서블 에셋 관리
│   ├── GPGSManager.cs    # GPGS 연동
│   ├── Leaderboard.cs    # 리더보드
│   └── Shop/
│       ├── IAPManager.cs # 인앱결제
│       ├── AdMobManager.cs # 광고
│       └── ShopManager.cs # 상점
├── Player/               # 플레이어
│   ├── Player.cs         # 플레이어 메인 컨트롤러
│   ├── PlayerMovement.cs # 이동 시스템
│   ├── PlayerInventory.cs # 인벤토리 관리
│   ├── Armors/           # 액세서리 아이템들
│   └── Weapons/          # 무기 아이템들
│       └── Projectile/    # 투사체 시스템
├── Object/               # 게임 오브젝트
│   ├── Enemy.cs          # 적 AI
│   ├── EXPOrb.cs         # 경험치 구슬
│   ├── Money.cs          # 재화 드롭
│   └── Treasure.cs       # 보물
└── UI/                   # UI 시스템
    ├── UIManager.cs      # UI 관리
    ├── VirtualJoystick.cs # 가상 조이스틱
    ├── UIInventory.cs    # 인벤토리 UI
    ├── LevelUpItem.cs    # 레벨업 아이템 UI
    └── PopUpManager.cs   # 팝업 관리
```

## 구현 포인트

### 오브젝트 풀링 시스템
- 재사용 가능한 오브젝트 풀 관리
- 적, 투사체, 드롭 아이템 등의 성능 최적화

### 레벨업 시스템
- 무기/액세서리 슬롯 가용성 체크 및 랜덤 아이템 표시
- 진화 조건 만족 시 진화 아이템 우선 제시

### 무기 시스템
- 추상 기본 클래스를 통한 무기 공통 로직 구현
- 쿨다운 시스템 및 추가 투사체 로직
- 다양한 타겟팅 (가장 가까운, 가장 먼, 범위 내 전체)

### 클라우드 저장
- Google Play Games Services를 통한 클라우드 저장
- 충돌 해결 전략 (가장 긴 플레이타임 우선)

## 개발 환경

```
Unity
Windows 11
Visual Studio Code
```

## 연락처

포트폴리오 및 프로젝트 문의: asd315406@gmail.com
