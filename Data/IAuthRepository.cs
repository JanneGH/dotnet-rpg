namespace dotnet_rpg.Data
{
    public interface IAuthRepository
    {
        // register
        Task<ServiceResponse<int>> Register(User user, string password);
        // login
        Task<ServiceResponse<string>> Login(string username, string password);
        // user exists check
        Task<bool> UserExists(string username);
    }
}