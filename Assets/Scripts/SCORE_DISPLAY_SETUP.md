# 3자리 숫자 점수 표시 설정 가이드

## 1. 숫자 스프라이트 준비

- `number_0.png` ~ `number_9.png`를 Unity `Assets/Sprites`에 넣고, 각각 Sprite로 Import (Pixels Per Unit: 64).

## 2. 점수 표시용 오브젝트 만들기

1. **빈 GameObject 생성** (이름: `ScoreDisplay`)
2. `ScoreDisplay`에 **ScoreDigitDisplay** 스크립트 추가

3. **자릿수 오브젝트 3개 생성** (ScoreDisplay의 자식으로):
   - Hierarchy에서 `ScoreDisplay` 우클릭 → **Create Empty** 3번
   - 이름: `DigitHundreds` (왼쪽), `DigitTens` (가운데), `DigitOnes` (오른쪽)

4. **각 자릿수에 SpriteRenderer 추가**:
   - `DigitHundreds` 선택 → Add Component → **Sprite Renderer**
   - Sprite: `number_0` 할당
   - `DigitTens`, `DigitOnes`도 동일하게 Sprite Renderer 추가, Sprite는 `number_0`

5. **위치 배치** (화면 상단 등):
   - 예: `DigitOnes` (0, 0), `DigitTens` (-0.4, 0), `DigitHundreds` (-0.8, 0)  
     (월드/UI에 맞게 조정)

## 3. ScoreDigitDisplay 연결

1. `ScoreDisplay` 선택
2. Inspector에서 **ScoreDigitDisplay** 컴포넌트:
   - **Number Sprites**: Size = 10, Element 0~9에 `number_0` ~ `number_9` 스프라이트 순서대로 할당
   - **Digit Hundreds**: `DigitHundreds`의 SpriteRenderer 드래그
   - **Digit Tens**: `DigitTens`의 SpriteRenderer 드래그
   - **Digit Ones**: `DigitOnes`의 SpriteRenderer 드래그

## 4. GameUI에 연결

1. `GameUI` 오브젝트 선택
2. **GameUI** 스크립트에서 **Score Digit Display** 필드에 `ScoreDisplay` 오브젝트를 할당

## 5. 렌더 순서 (숫자가 가려질 때)

- **배경(Background)** SpriteRenderer: **Order in Layer** = 0 또는 -1
- **ScoreDigitDisplay** 스크립트의 **Sorting Order** = 10 (기본값)  
  → 숫자가 배경보다 앞에 그려짐
- 스네이크/음식: Order in Layer = 0~5 정도로 두면 배경 뒤, 숫자 앞

이제 게임 실행 시 먹은 사과 개수가 000, 001, 002 … 형태로 3자리 스프라이트로 표시됩니다.
