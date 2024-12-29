namespace CleanArchitectureSampleProject.Tests.Integration.Setups;

using CleanArchitectureSampleProject.Tests.Integration.Setups.Fixtures;
using WireMock.Server;

public abstract class IntegrationTestSetup
{
    //Podemos adicionar fields protected aqui para expor o WiremockServer, assim como Fixtures, Faker, e outras ferramentas compartilhadas entre testes

    protected static readonly WireMockServer WiremockServer = WireMockServer.Start(port:7435, useSSL: true);
    protected static readonly HttpClient MinimalApi = MyApiFixture.Client;

    protected IntegrationTestSetup()
    {
        MyApiFixture.Setup();
        WiremockServer.Reset();
    }
}
