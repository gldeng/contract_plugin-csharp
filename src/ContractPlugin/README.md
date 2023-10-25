# ContractPlugin

## Introduction

This is the actual protoc-plugin executable that is to be built & distributed to clients (i.e protoc users).

## Build

``
dotnet build
``

Though to build a single platform-specific binary executable you would need to use this command:
``
dotnet publish -c Release -r osx.13-arm64 --self-contained true
``

## Using with protoc

You'll need to locate the complete self-contained single binary.
``
cd <ProjectRoot>/src/ContractPlugin/bin/Release/net7.0/osx.13-arm64/publish
``
Then copy it to your /usr.. bin folder (i.e your PATH) so that the plugin is accessible via `protoc`
``
cp ContractPlugin /usr/local/bin/protoc-gen-contract_csharp_plugin
``

Now you can find a project with the required proto contract files and try it out.
``
protoc -I ./protos_test/aelf/ -I ./protos_test/ --plugin=contract_csharp_plugin --contract_csharp_plugin_out=. protos_test/hello_world_contract.proto
``
you can also try with a protoc-param e.g "stub" to get a different C# output.
``
protoc -I ./protos_test/aelf/ -I ./protos_test/ --plugin=contract_csharp_plugin --contract_csharp_plugin_out=. --contract_csharp_plugin_opt="stub" protos_test/hello_world_contract.proto
``

## Testing in AElf project

1. Complete `Build` step and run the following command to copy plugin to AElf project.

``
cp <UserRoot>/contract_plugin-csharp/src/ContractPlugin/bin/Release/net7.0/osx.13-arm64/publish/ContractPlugin <UserRoot>/AElf/scripts/contract_csharp_plugin
``

2. Go to system contract module of AElf project, and run build command.

