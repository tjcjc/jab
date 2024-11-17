using System;
using Jab;
using Xunit;

namespace JabTests;

public interface IMyService : IDisposable
{
}

public interface IRootService : IMyService
{
}

public class ChildService: IDisposable
{
    public IScopeService _scopeService;
    public bool Disposed { get; private set; }

    public ChildService(IScopeService scopeService)
    {
        _scopeService = scopeService;
    }

    public void Dispose()
    {
        Disposed = true;
    }
}

public interface IScopeService : IMyService;

public class ScopeServiceIMp : IScopeService
{
    public bool IsDisposed;
    public void Dispose()
    {
        // TODO release managed resources here
        IsDisposed = true;
    }
}

public class ImpConf
{
}

public class RootServiceImp : IRootService
{
    public RootServiceImp(ImpConf conf)
    {
    }

    public bool _disposed;

    public void Dispose()
    {
        _disposed = true;
        // TODO release managed resources here
    }
}

[ServiceProviderModule]
[Singleton(typeof(ImpConf), Factory = nameof(MyImpCon))]
[Singleton(typeof(IRootService), typeof(RootServiceImp), Name = "RootA")]
[Singleton(typeof(IRootService), typeof(RootServiceImp), Name = "RootB")]
[Scoped(typeof(IScopeService), typeof(ScopeServiceIMp))]
internal interface IRootServiceModule
{
    public ImpConf MyImpCon();
}

[ServiceProvider]
[Import(typeof(IRootServiceModule))]
internal partial class HowRotUseRootContainer: IRootServiceModule
{
    public ImpConf MyImpCon()
    {
        return new ImpConf();
    }
}

[ServiceProvider]
[Import(typeof(IRootServiceModule), nameof(_rootContainer))]
[Scoped(typeof(ChildService))]
internal partial class ChildContainer
{
    private HowRotUseRootContainer.Scope _rootContainer;

    public ChildContainer(HowRotUseRootContainer.Scope rootContainer)
    {
        _rootContainer = rootContainer;
    }
}

public class RootContainerTest
{
    [Fact]
    public void Test()
    {
        var container = new HowRotUseRootContainer();
        var scope = container.CreateScope();
        RootServiceImp service1 = (RootServiceImp)scope.GetService<IRootService>("RootA");
        var childContainer = new ChildContainer(scope);
        var childRootService = childContainer.GetService<IRootService>("RootA");
        var scopeService = scope.GetService<IScopeService>();
        var childService = childContainer.GetService<ChildService>();
        Assert.Same(scopeService, childService._scopeService);
        scopeService.Dispose();
        childContainer.Dispose();
        Assert.False(service1._disposed);
        Assert.True(((ScopeServiceIMp)scopeService).IsDisposed);
        Assert.True(childService.Disposed);
    }
}