# 모바일 SNS 콘텐츠 편집기 프로젝트

## 1. 개발 환경
- **Engine:** Unity 2022.3.62f2
- **Language:** C#, HLSL
- **Project Type:** 2D
- **Target Platform:** Android
- **개발 기간:** 2026-05-21 ~ 2026-07-10
- **Tools:** Visual Studio, GitHub Desktop

## 2. 팀 구성
- **개발:** 4명
- **기획:** 8명

## 3. 주요 기술 스택
- **Architecture:** Singleton, SRP(단일 책임 원칙), Method Chaining, Data-Driven Design
- **Framework & Library:** Addressables, DOTween, Firebase, GoogleSheetParse, New Input System

## 4. 핵심 구현 기능

### SNS 콘텐츠 편집 시스템
- **입력 시스템:** `New Input System` 기반의 `TouchInputHandler`를 설계하여 드래그, 핀치 줌, 회전 등 멀티 터치 상호작용 구현.
- **편집 UI:** `PlaceHolder`를 활용한 동적 드래그 앤 드롭 정렬 알고리즘 적용 및 버튼 조합형 코멘트/해시태그 작성 시스템.
- **이미지 프로세싱:** HLSL 셰이더를 통해 실시간 명도, 대비, 채도 조절 기능 구현.
- **선택 및 관리:** `Dictionary` 매핑을 통해 스티커와 토글 버튼 상태를 관리하여 $O(1)$의 효율적인 상태 제어 구현.

### 시스템 및 데이터 아키텍처
- **Firebase 프레임워크:** `BaseFirestore` 추상 클래스를 통한 CRUD 모듈화.
    - **성능 최적화:** 리플렉션 사용 시 발생할 수 있는 런타임 성능 저하를 방지하기 위해 객체 매핑 정보를 캐싱(`Dictionary`)하여 시스템 부하 최소화.
    - **생산성 향상:** 메서드 체이닝 기법을 도입하여 직관적인 API 제공.
- **데이터 기반 설계 (Data-Driven Design):**
    - `GoogleSheetParse`: `UnityWebRequest` 기반의 범용 파싱기 구현 및 `ISheetParsable` 인터페이스를 통해 데이터 조립 로직 통일.
    - `Addressables` 캐싱: `PopupSpriteCacheManager`를 통한 리소스 사전 로드 및 최적화.
- **알고리즘 최적화:**
    - **랜덤 데이터 추출:** `RandomId` 필드와 기준점 쿼리를 활용한 효율적인 문서 탐색 및 예외 처리(6개 미만 추출 시 보완 로직 구현).
    - **뽑기 시스템:** `Fisher-Yates Shuffle` 알고리즘을 적용한 무작위 데이터 셔플 및 `DOTween` 연출 구현.

### 시즌 및 이벤트 콘텐츠
- **날짜 동기화:** 서버/로컬 시간 비교 및 코루틴 기반 실시간 초기화 로직 구현.
- **보상 시스템:** 누적 뽑기 카운터(10회 단위 보상) 로직 및 `DOTween` 인터랙션 연출.

### 미리보기 및 업로드 (New_Preview_Canvas)
- **종합 렌더링:** `SNSPostDTO`를 기반으로 이미지, 스티커, 코멘트, 해시태그를 종합하여 최종 게시물 형태를 렌더링.
- **데이터 동기화:** `SubscribeManager`를 통해 이전 패널들과 데이터 동기화 유지.
- **안전한 업로드:** `SNSPostManager`를 통한 비동기 업로드 처리. 업로드 실패 시 UI 잠금 해제 및 에러 핸들링 처리.
- **화면 전환:** 업로드 성공 시 데이터 초기화 및 프로필 화면으로 자동 전환.

## 5. 조작 방법

### 스티커 편집
1. 스티커 패널에서 스티커 선택 및 생성.
2. 선택 시 삭제 버튼 활성화 및 포커스 변경(다른 스티커 클릭 또는 토글 버튼 이용).
3. **상호작용:** 투 터치 회전, 드래그 앤 드롭 이동, 핀치 줌(확대/축소) 지원.

### 코멘트 패널
1. 카테고리 선택 시 해시 코멘트 목록 생성 및 블록 뷰 영역 배치.
2. **제한 사항:** 최대 50자, 동일 단어 반복 최대 3회 제한 및 팝업 안내.
3. **편집:** `PlaceHolder`를 활용한 코멘트 블럭 드래그 앤 드롭 정렬 및 실시간 미리보기 기능.
