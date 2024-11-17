namespace Jab;

internal record ServiceRegistration(
    ServiceLifetime Lifetime,
    INamedTypeSymbol ServiceType,
    string? Name,
    INamedTypeSymbol? ImplementationType,
    ISymbol? InstanceMember,
    ISymbol? FactoryMember,
    Location? Location,
    MemberLocation MemberLocation,
    string? Parent = null)
{
    public INamedTypeSymbol ServiceType { get; } = ServiceType;
    public string? Name { get; } = Name;
    public INamedTypeSymbol? ImplementationType { get; } = ImplementationType;
    public ISymbol? InstanceMember { get; } = InstanceMember;
    public ISymbol? FactoryMember { get; } = FactoryMember;
    public ServiceLifetime Lifetime { get; } = Lifetime;
    public Location? Location { get; } = Location;
    public MemberLocation MemberLocation { get; } = MemberLocation;
    public string? Parent { get; } = Parent;
}

internal record RootService(INamedTypeSymbol Service, Location? Location);