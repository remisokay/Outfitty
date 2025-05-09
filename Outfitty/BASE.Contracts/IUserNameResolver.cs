namespace BASE.Contracts;

public interface IUserNameResolver
{
    // tracking who makes changes to entities
    string CurrentUserName { get; }
}