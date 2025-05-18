namespace APP.DAL.EF.DataSeeding;

public static class InitialData
{
    public static readonly (string roleName, Guid? id)[]
        Roles = [ ("admin", null), ("user", null)];

    public static readonly (string email, string username, string bio, string password, Guid? id, string[] roles)[]
        Users =
        [
            ("admin@outfitty.com", "OutfittyBoss", "The administrator of this wonderful website", "outfittyPass1", null, ["admin"]),
            ("youlikejazz@user.com", "JazzLover", "Fashion enthusiast with a love for jazz", "userPass1", null, ["user"]),
        ];
}