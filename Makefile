osx-build:
	dotnet publish -c Release -r osx.13-arm64 --self-contained true
	
format:
	dotnet format