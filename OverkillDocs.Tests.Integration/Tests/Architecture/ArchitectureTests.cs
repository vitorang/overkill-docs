using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using ArchUnitArchitecture = ArchUnitNET.Domain.Architecture;

namespace OverkillDocs.Tests.Integration.Tests.Architecture;

public class ArchitectureTests
{
    private static readonly ArchUnitArchitecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Core.DependencyInjection).Assembly,
            typeof(Infrastructure.DependencyInjection).Assembly
        ).Build();

    [Fact]
    public void AllClassesShouldBeSealed()
    {
        IArchRule sealedRule = Classes()
            .That().AreNotAbstract()
            .Should().BeSealed();

        sealedRule.Check(Architecture);
    }

    [Fact]
    public void InfrastructureClassesShouldBeInternal()
    {
        IArchRule internalRule = Classes()
            .That().ResideInNamespaceMatching("OverkillDocs.Infrastructure.*")
            .And().DoNotHaveName("DependencyInjection")
            .And().DoNotHaveName("AppDbContext")
            .Should().BeInternal();

        internalRule.Check(Architecture);
    }

    [Fact]
    public void ServicesClassesShouldBeInternal()
    {
        IArchRule internalRule = Classes()
            .That().HaveNameEndingWith("Service")
            .Should().BeInternal();

        internalRule.Check(Architecture);
    }

    [Fact]
    public void RepositoriesClassesShouldBeInternal()
    {
        IArchRule internalRule = Classes()
            .That().HaveNameEndingWith("Repository")
            .Should().BeInternal();

        internalRule.Check(Architecture);
    }
}
