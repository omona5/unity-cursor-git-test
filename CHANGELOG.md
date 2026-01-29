# Changelog

프로젝트 변경 사항을 날짜별로 기록합니다.

---

## 2025-01-28

### Added
- **점수 표시 (3자리 스프라이트)**: 먹은 사과 개수를 000~999 숫자 스프라이트로 표시
  - `ScoreDigitDisplay.cs`, `SCORE_DISPLAY_SETUP.md` 추가
  - 32×64 숫자 스프라이트 생성 스크립트 `generate_number_sprites.py`
- **WebGL 빌드**: Chrome 등 브라우저에서 실행 가능
  - `Assets/Editor/WebGLBuild.cs` (메뉴: Build → WebGL)
  - `WEBGL_BUILD_README.md` (빌드·실행 방법)
- **배경 스프라이트**: 게임 영역(짙은 초록) / 외부(검정) 구분
  - `generate_background.py` (768×960)
- **점수에 따른 게임 속도 상승**: `baseMoveInterval`, `moveIntervalDecreasePerScore`, `minMoveInterval`로 조절

### Changed
- **입력 처리**: 구 Input → 새 Input System (`UnityEngine.InputSystem`)으로 변경
  - `SnakeController.cs`, `ArrowKeyDebug.cs` 수정
- **게임 경계**: `IsOutOfBounds()`에서 경계값 포함 (`>=`, `<=`)하여 -5~5, -6.5~3.5 정확히 적용
- **스네이크 시작 위치**: (0, 0.5)에서 시작해 아래쪽 0.5유닛 여유 확보
- **음식 생성**:
  - 스네이크(헤드·세그먼트)가 있는 칸에는 생성하지 않도록 빈 칸 목록에서만 랜덤 선택
  - y 좌표 0.5 오프셋 적용해 스네이크 격자와 일치
  - `gameAreaMax.y` 초과 방지 (`maxY` 계산 시 0.5 반영)
- **헤드·세그먼트 회전**: 이동 방향에 맞춰 스프라이트 Z 회전
  - 헤드: 시계 방향 90° 보정
  - 세그먼트: 앞쪽으로 이어지는 방향으로 회전
- **이동 간격 변수**: `gameSpeed` → `baseMoveInterval` (작을수록 빠름), 점수 기반 감소 로직 추가
- **SnakeGameManager**: `RefreshScoreDisplay()`, `scoreDigitDisplay` 직접 참조로 점수 UI 갱신 보강
- **스프라이트**: 스네이크 머리/몸 margin 0으로 변경 (64×64 꽉 참), 배경 짙은 초록색 조정

### Fixed
- Vector2 기본값에 `f` 접미사 추가 (CS1503 double→float 오류)
- 음식이 y=5 등 경계 밖에 생성되던 문제 수정

---

## (이전 기록)

초기 스네이크 게임 구현: 방향키 조종, 음식·벽·자기몸 충돌, 게임 오버, R 재시작, 기본 UI 등.
