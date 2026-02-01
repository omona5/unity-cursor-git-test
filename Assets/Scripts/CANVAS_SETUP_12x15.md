# Canvas 배경 설정 가이드 (월드 12×15 유닛)

월드에서 **가로 12유닛, 세로 15유닛** 안에서 게임이 진행되도록 이미 설정한 상태에서,  
2D 스프라이트 배경 대신 **UI Canvas + 배경 이미지**로 바꾸는 방법입니다.

- **기존**: 2D 스프라이트 배경 — 768×960 픽셀, PPU 64 → 월드 크기 12×15 ✅
- **목표**: 같은 12×15 영역을 **UI 배경**으로 채우고, 레터박스/필러박스도 12:15로 유지

---

## 1. Main Camera / Letterboxer (게임 영역 비율)

- **Main Camera**에 **Letterboxer**가 붙어 있어야 합니다.
- **Letterboxer** Inspector:
  - **Design Aspect Width**: `12`
  - **Design Aspect Height**: `15`
- **Main Camera** (Orthographic):
  - **Size (Orthographic Size)**: `7.5`  
    → 보이는 세로 = 2 × 7.5 = **15 유닛**, 가로 = 15 × (12/15) = **12 유닛**

이렇게 하면 카메라가 그리는 영역이 정확히 **12×15 월드 유닛**입니다.

---

## 2. Canvas 만들기

1. 메뉴 **GameObject → UI → Canvas**
2. Hierarchy에 **Canvas**, **EventSystem** 생성됨
3. **Canvas** 선택 후 Inspector:

| 항목 | 값 |
|------|-----|
| **Render Mode** | Screen Space - Camera |
| **Render Camera** | Main Camera (Letterboxer 붙은 카메라) |
| **Plane Distance** | 100 |

→ UI가 **Main Camera가 그리는 12:15 영역 안**에만 그려집니다.

---

## 3. Canvas Scaler (스케일은 스크립트가 담당)

- **Canvas Scaler**:
  - **UI Scale Mode**: **Constant Pixel Size**
  - **Scale Factor**: `1`
- 나중에 **Canvas Viewport Scaler**가 뷰포트 크기에 맞춰 스케일을 적용합니다.

---

## 4. Canvas Viewport Scaler 추가

1. **Canvas**에 **Add Component** → **Canvas Viewport Scaler** 검색 후 추가
2. Inspector:
   - **Reference Width**: `768` (12 × 64)
   - **Reference Height**: `960` (15 × 64)

→ Canvas 좌표 768×960이 **카메라 뷰포트(레터박스 영역) 픽셀 크기**와 1:1로 맞습니다.  
→ 기존 2D 배경(768×960 px, PPU 64 → 12×15 유닛)과 같은 비율·크기로 보입니다.

---

## 5. 배경용 UI Image 추가

1. **Canvas** 우클릭 → **UI → Image**
2. 이름을 **Background** 등으로 변경
3. **Image** 컴포넌트:
   - **Source Image**: `background.png` (768×960 픽셀 권장)
   - **Color**: 흰색(255,255,255) 또는 원하는 색
4. **Rect Transform**:
   - **Anchor Preset**: 가운데 (0.5, 0.5) / (0.5, 0.5)
   - **Width**: `768`
   - **Height**: `960`
   - **Pos X, Y, Z**: `0`, `0`, `0`

이렇게 하면 **768×960 Canvas 단위 = 12×15 월드 유닛**과 같은 영역을 채웁니다.

---

## 6. (선택) 배경을 Canvas 전체로 채우기

- Anchor를 **stretch**(좌하 stretch 아이콘에서 Shift+Alt로 stretch-stretch)로 바꾸고  
  **Left, Right, Top, Bottom** 을 모두 `0`으로 두면,  
  Canvas 전체(768×960)를 꽉 채우므로 위와 같은 효과입니다.

---

## 7. 정리

| 구분 | 내용 |
|------|------|
| **월드** | 가로 12 유닛, 세로 15 유닛 |
| **Letterboxer** | 12:15 비율 → 레터박스/필러박스 영역 = 12×15 |
| **Canvas** | Screen Space - Camera, Main Camera |
| **Canvas Viewport Scaler** | Reference 768×960 → 뷰포트 픽셀에 맞춤 |
| **배경 UI Image** | 768×960 크기 → 12×15 영역과 동일하게 표시 |

이후 점수, 버튼 등 UI는 **같은 Canvas** 자식으로 추가하면, 모두 12×15 게임 영역 안에만 그려집니다.

---

## 8. 기존 2D 배경 스프라이트 제거

- 2D 스프라이트 배경 오브젝트는 **비활성화**하거나 **삭제**해도 됩니다.
- 배경은 이제 Canvas의 **Background (UI Image)** 가 담당합니다.
