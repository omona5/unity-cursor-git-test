#!/bin/bash
# WebGL 빌드만 gh-pages 브랜치에 올리는 스크립트
#
# 사용법:
#   ./deploy-gh-pages.sh                    # Build/WebGL 또는 Builds/WebGL 사용
#   ./deploy-gh-pages.sh /경로/빌드폴더     # 지정한 폴더 사용 (권장: 프로젝트 밖)
#   WEBGL_BUILD_DIR=/경로 ./deploy-gh-pages.sh
#
# 권장: Unity에서 빌드 출력을 프로젝트 밖으로 두면 브랜치 전환 시 삭제되지 않음.
#   예) Build → ../unity-cursor-git-test-WebGL
#
# 현재 사용 중인 빌드 경로 (프로젝트 상위 폴더):
#   /Users/omona/UnityProject/cursor_test/Build/WebGL
#   실행 예: ./deploy-gh-pages.sh /Users/omona/UnityProject/cursor_test/Build/WebGL
#
# GitHub Pages: 압축 끄고 빌드. Settings → Pages: Branch = gh-pages, Folder = /docs

set -e
cd "$(dirname "$0")"

# 빌드 경로: 인자 > 환경변수 > Build/WebGL > Builds/WebGL
if [ -n "$1" ]; then
  BUILD_DIR="$1"
elif [ -n "$WEBGL_BUILD_DIR" ]; then
  BUILD_DIR="$WEBGL_BUILD_DIR"
else
  BUILD_DIR="Build/WebGL"
  if [ ! -f "$BUILD_DIR/index.html" ]; then
    BUILD_DIR="Builds/WebGL"
  fi
fi

if [ ! -f "$BUILD_DIR/index.html" ]; then
  echo "오류: 빌드 폴더에 index.html 이 없습니다: $BUILD_DIR"
  echo ""
  echo "  방법 1) Unity에서 File → Build Settings → WebGL → Build 실행"
  echo "  방법 2) 프로젝트 밖으로 빌드 후 경로 지정:"
  echo "          ./deploy-gh-pages.sh ../unity-cursor-git-test-WebGL"
  exit 1
fi

echo "빌드 폴더 사용: $BUILD_DIR"
TMP_BUILD=$(mktemp -d)
cp -r "$BUILD_DIR"/. "$TMP_BUILD/"

git checkout master
git branch -D gh-pages 2>/dev/null || true
git checkout --orphan gh-pages

git rm -rf . 2>/dev/null || true
find . -maxdepth 1 ! -name . ! -name .git -exec rm -rf {} + 2>/dev/null || true

mkdir -p docs
(cd "$TMP_BUILD" && tar cf - .) | (cd docs && tar xf -)
rm -rf "$TMP_BUILD"

git add .
git commit -m "Deploy WebGL build to GitHub Pages"
git push -u origin gh-pages --force

echo "완료. GitHub Pages: Branch = gh-pages, Folder = /docs"
git checkout master
