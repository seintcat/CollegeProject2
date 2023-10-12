ColorDict
- 값 > Name
- None > R
- None > G
- None > B
- None > A
- rgba는 float로 저장

SpriteOne
- 값 > 파일 경로
- None >  스프라이트 이름
- 이름 값이 없다면 파일 이름으로 적용
- 최종적으로 스프라이트 이중 리스트로 반환
- 사용되는 프리팹 등에서 해당 컴포넌트가 존재하는 부분 오브젝트 이름과 대응되어야 함

SpriteAtlas
- 값 > 파일 경로
- None >  스프라이트 이름
- 이름 값이 없다면 파일 이름으로 적용
- 최종적으로 스프라이트 이중 리스트로 반환
- 사용되는 프리팹 등에서 해당 컴포넌트가 존재하는 부분 오브젝트 이름과 대응되어야 함

SpriteType
- 값 >  스프라이트 이름
- None > 타입
- 타입 값이 없다면 그냥 Simple로 씀
- 사용되는 프리팹 등에서 해당 컴포넌트가 존재하는 부분 오브젝트 이름과 대응되어야 함

Prefab
- 값 > 파일 경로

ProgressType
- 값 > 가로세로 여부(Vertical, Horizontal)
- 스프라이트 타입이 Filled가 아니라면 앵커 사용함
- 로딩창 뷰 관련해서 사용함
- 이름은 Progress 고정

로딩창 텍스트 설명도 Text로 오브젝트 이름고정