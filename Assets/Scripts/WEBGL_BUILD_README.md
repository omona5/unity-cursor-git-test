# WebGL 빌드 (Chrome 등 브라우저에서 실행)

## 1. Unity 에디터에서 빌드

1. Unity에서 프로젝트 열기
2. 상단 메뉴 **Build → WebGL (Chrome 등 브라우저)** 클릭
3. 빌드가 끝나면 프로젝트 루트에 **Builds/WebGL** 폴더가 생성됨

## 2. 브라우저에서 실행

WebGL은 보안상 **로컬 파일( file:// )로 직접 열면 동작하지 않을 수 있습니다.**  
반드시 **로컬 웹 서버**로 띄운 뒤 접속하세요.

### 방법 A: Python으로 서버 띄우기

```powershell
cd E:\UnityProjects\cursor_test\Builds\WebGL
python -m http.server 8080
```

브라우저에서 **http://localhost:8080** 접속 후 `index.html` 클릭 (또는 http://localhost:8080/index.html )

### 방법 B: Node.js (npx)

```powershell
cd E:\UnityProjects\cursor_test\Builds\WebGL
npx serve -p 8080
```

브라우저에서 **http://localhost:8080** 접속

### 방법 C: VS Code Live Server

1. VS Code에서 **Builds/WebGL** 폴더 열기
2. **Live Server** 확장 설치 후 `index.html` 우클릭 → **Open with Live Server**

## 3. 주의사항

- **Input System**: WebGL에서는 키보드 포커스가 게임 영역에 있어야 방향키가 동작합니다. 페이지 로드 후 **게임 화면을 한 번 클릭**한 뒤 조작하세요.
- **해상도/성능**: 브라우저에 따라 성능이 다를 수 있습니다. Player Settings → WebGL에서 해상도/품질을 낮출 수 있습니다.
- **빌드 시간**: WebGL 빌드는 첫 빌드 시 시간이 꽤 걸릴 수 있습니다.

## 4. 커맨드라인 빌드 (선택)

Unity가 PATH에 있으면 터미널에서:

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.5f2\Editor\Unity.exe" -quit -batchmode -projectPath "E:\UnityProjects\cursor_test" -executeMethod WebGLBuild.BuildWebGL
```

(Unity 버전 경로는 설치 경로에 맞게 수정)
