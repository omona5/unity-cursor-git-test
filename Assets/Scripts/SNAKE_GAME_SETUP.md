# 스네이크 게임 설정 가이드

## 1. 프리팹 생성

### 스네이크 헤드 프리팹
1. Hierarchy에서 빈 GameObject 생성 (이름: SnakeHead)
2. SpriteRenderer 컴포넌트 추가
3. Sprite를 생성하거나 기본 Quad 사용
4. 색상: 초록색
5. SnakeController 스크립트 추가
6. Assets 폴더에 프리팹으로 저장 (Prefabs/SnakeHead.prefab)

### 스네이크 세그먼트 프리팹
1. Hierarchy에서 빈 GameObject 생성 (이름: SnakeSegment)
2. SpriteRenderer 컴포넌트 추가
3. Sprite를 생성하거나 기본 Quad 사용
4. 색상: 초록색 (헤드보다 약간 어두운 색)
5. SnakeSegment 스크립트 추가
6. Assets 폴더에 프리팹으로 저장 (Prefabs/SnakeSegment.prefab)

### 음식 프리팹
1. Hierarchy에서 빈 GameObject 생성 (이름: Food)
2. SpriteRenderer 컴포넌트 추가
3. Sprite를 생성하거나 기본 Quad 사용
4. 색상: 빨간색
5. Food 스크립트 추가
6. Assets 폴더에 프리팹으로 저장 (Prefabs/Food.prefab)

## 2. 게임 매니저 설정

1. Hierarchy에서 빈 GameObject 생성 (이름: GameManager)
2. SnakeGameManager 스크립트 추가
3. Inspector에서 설정:
   - Game Speed: 0.2
   - Grid Size: 20
   - Game Area Min: (-10, -10)
   - Game Area Max: (10, 10)
   - Snake Head Prefab: 위에서 만든 SnakeHead 프리팹 할당
   - Snake Segment Prefab: 위에서 만든 SnakeSegment 프리팹 할당
   - Food Prefab: 위에서 만든 Food 프리팹 할당

## 3. UI 설정

1. Hierarchy에서 UI > Canvas 생성
2. Canvas 아래에 UI > Text - TextMeshPro 생성 (이름: ScoreText)
   - 위치: 상단 중앙
   - 텍스트: "점수: 0"
3. UI > Panel 생성 (이름: GameOverPanel)
   - 초기 상태: 비활성화
   - Panel 아래에:
     - Text - TextMeshPro (이름: GameOverText): "게임 오버!"
     - Text - TextMeshPro (이름: RestartHintText): "R 키를 눌러 재시작"
4. 빈 GameObject 생성 (이름: GameUI)
5. GameUI 스크립트 추가
6. Inspector에서 설정:
   - Score Text: ScoreText 할당
   - Game Over Text: GameOverText 할당
   - Restart Hint Text: RestartHintText 할당
   - Game Over Panel: GameOverPanel 할당
7. GameManager의 Game UI 필드에 GameUI 할당

## 4. 카메라 설정

1. Main Camera 선택
2. Projection: Orthographic
3. Size: 11 (게임 영역에 맞게 조정)

## 5. 스프라이트 크기 조정

모든 프리팹의 SpriteRenderer에서:
- Scale: (1, 1, 1)
- 또는 Sprite 크기를 1x1로 설정

## 6. 게임 실행

Play 버튼을 눌러 게임을 실행하고 방향키로 스네이크를 조종하세요!
