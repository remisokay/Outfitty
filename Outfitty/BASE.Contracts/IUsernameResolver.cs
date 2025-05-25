namespace BASE.Contracts;

public interface IUsernameResolver
{
    // tracking who makes changes to entities
    string CurrentUserName { get; }
}