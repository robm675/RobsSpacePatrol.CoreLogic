namespace RobsSpacePatrol.DTO;

public static class SD
{
    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public const string SessionAccessToken = "JWTToken";
    public const string SessionRefreshToken = "RefreshToken";
}
