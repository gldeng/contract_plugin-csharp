namespace ContractGenerator.Tests;

public class ContractContainerGeneratorTests : TestBase
{
    [Fact]
    public void TestGenerateContainer_NoErrors()
    {
        var indentPrinter = new IndentPrinter();
        var fileDescriptors = GetFileDescriptors("helloworld");
        var svc = fileDescriptors[^1].Services.Last();

        var flags = FlagConstants.GenerateContractWithEvent;
        flags |= FlagConstants.GenerateStubWithEvent;
        flags &= FlagConstants.GenerateContract;

        ContractContainerGenerator.Generate(indentPrinter, svc, flags);
        var contractContainerCodeStr = indentPrinter.PrintOut();
        const string expectedContainerCodeStr = @"public static partial class HelloWorldContainer
{
  static readonly string __ServiceName = ""HelloWorld"";
  #region Marshallers
  static readonly aelf::Marshaller<global::Google.Protobuf.WellKnownTypes.StringValue> __Marshaller_google_protobuf_StringValue = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Google.Protobuf.WellKnownTypes.StringValue.Parser.ParseFrom);
  static readonly aelf::Marshaller<global::Google.Protobuf.WellKnownTypes.Empty> __Marshaller_google_protobuf_Empty = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Google.Protobuf.WellKnownTypes.Empty.Parser.ParseFrom);
  #endregion

  #region Methods
  static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.StringValue, $response$> __Method_Update = new aelf::Method<global::Google.Protobuf.WellKnownTypes.StringValue, global::Google.Protobuf.WellKnownTypes.Empty>(
      aelf::MethodType.Action,
      __ServiceName,
      ""Update"",
      __Marshaller_google_protobuf_StringValue,
      __Marshaller_google_protobuf_Empty);

  static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, $response$> __Method_Read = new aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::Google.Protobuf.WellKnownTypes.StringValue>(
      aelf::MethodType.View,
      __ServiceName,
      ""Read"",
      __Marshaller_google_protobuf_Empty,
      __Marshaller_google_protobuf_StringValue);

  #endregion

  #region Descriptors
  public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
  {
    get { return global::AElf.Contracts.HelloWorld.ContractReflection.Descriptor.Services[0]; }
  }

  public static global::System.Collections.Generic.IReadOnlyList<global::Google.Protobuf.Reflection.ServiceDescriptor> Descriptors
  {
    get
    {
      return new global::System.Collections.Generic.List<global::Google.Protobuf.Reflection.ServiceDescriptor>()
      {
        global::AElf.Contracts.HelloWorld.ContractReflection.Descriptor.Services[0],
      };
    }
  }
  #endregion

  /// <summary>Base class for the contract of HelloWorldBase</summary>
  public abstract partial class HelloWorldBase : AElf.Sdk.CSharp.CSharpSmartContract<AElf.Contracts.HelloWorld.HelloWorldState>
  {
    public abstract global::Google.Protobuf.WellKnownTypes.Empty Update(global::Google.Protobuf.WellKnownTypes.StringValue input);
    public abstract global::Google.Protobuf.WellKnownTypes.StringValue Read(global::Google.Protobuf.WellKnownTypes.Empty input);
  }

  public static aelf::ServerServiceDefinition BindService(HelloWorldBase serviceImpl)
  {
    return aelf::ServerServiceDefinition.CreateBuilder()

.AddDescriptors(Descriptors)

.AddMethod(__Method_Update, serviceImpl.Update)

.AddMethod(__Method_Read, serviceImpl.Read)
        .Build();
  }

}

";
        Assert.Equal(expectedContainerCodeStr, contractContainerCodeStr);
    }

    [Fact]
    public void TestGenerateContractBaseClass_NoErrors()
    {
        var indentPrinter = new IndentPrinter();
        var fileDescriptors = GetFileDescriptors("helloworld");
        var svc = fileDescriptors[^1].Services.Last();

        ContractContainerGenerator.GenerateContractBaseClass(indentPrinter, svc);
        var contractBaseCodeStr = indentPrinter.PrintOut();
        const string expectedCodeStr =
            @"/// <summary>Base class for the contract of HelloWorldBase</summary>
public abstract partial class HelloWorldBase : AElf.Sdk.CSharp.CSharpSmartContract<AElf.Contracts.HelloWorld.HelloWorldState>
{
  public abstract global::Google.Protobuf.WellKnownTypes.Empty Update(global::Google.Protobuf.WellKnownTypes.StringValue input);
  public abstract global::Google.Protobuf.WellKnownTypes.StringValue Read(global::Google.Protobuf.WellKnownTypes.Empty input);
}

";
        Assert.Equal(expectedCodeStr, contractBaseCodeStr);
    }
}
