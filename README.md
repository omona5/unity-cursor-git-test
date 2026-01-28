# Unity Snake Game

간단한 스네이크 게임 프로젝트입니다.

## 기능

- 방향키로 스네이크 조종
- 음식을 먹으면 길이 증가 및 점수 증가
- 벽이나 자신의 몸에 부딪히면 게임 오버
- R 키로 재시작

## 기술 스택

- Unity 6000.3.5f2
- Universal Render Pipeline (URP)
- Input System (새로운 Input System)
- C#

## 게임 설정

- 해상도: 768x960
- 게임 영역: 가로 11칸, 세로 11칸
- 카메라: Orthographic, Size = 7.0

## 프로젝트 구조

- `Assets/Scripts/`: 게임 로직 스크립트
  - `SnakeGameManager.cs`: 게임 전체 관리
  - `SnakeController.cs`: 스네이크 헤드 컨트롤
  - `SnakeSegment.cs`: 스네이크 본체 세그먼트
  - `Food.cs`: 음식 오브젝트
  - `GameUI.cs`: UI 관리
- `Assets/python_scripts/`: 스프라이트 생성 스크립트
  - `generate_snake_sprites.py`: 스네이크 스프라이트 생성
  - `generate_background.py`: 배경 스프라이트 생성

## 시작하기

1. Unity Hub에서 프로젝트 열기
2. `Assets/Scenes/SampleScene.unity` 열기
3. Play 버튼으로 게임 실행
