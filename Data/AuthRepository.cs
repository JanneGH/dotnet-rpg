namespace dotnet_rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var serviceResponse = new ServiceResponse<string>();

            // get user
            var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower().Equals(username.ToLower()));

            if (user is null)
            {
                serviceResponse.IsSuccess = false;
                // TODO: Change in production. Message for testing purposes only!
                serviceResponse.Message = "User not found";
            }
            // check password
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                serviceResponse.IsSuccess = false;
                // TODO: Change in production. Message for testing purposes only!
                serviceResponse.Message = "Wrong password";
            }
            else
            {
                serviceResponse.Data = user.Id.ToString();
                serviceResponse.Message = $"{user.Username} logged in";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var serviceResponse = new ServiceResponse<int>();

            // first check if user exists
            if (await UserExists(user.Username))
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = "User already exists";
                return serviceResponse;
            }

            try
            {
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = user.Id;
            }
            catch (Exception e)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = e.Message;
            }
            return serviceResponse;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(
                u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }

            return false;
        }

        // Using out parameter to set them in the Register method in the user object.
        // The values are not returned because the out parameter is used. That is why they can be set or even somehow returned. 
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // creating an instance of the HMAC class already generates a key that can be used
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}

// Password hashes are not safe. The algorithm can be reversed and it is always the same so equal passwords generate the same hash.
// PasswordSalt is used to create different hashes even if the same pw is used by different users. 
// the salt is stored in the db to be able to retrieve it and used as the key together with the hash to decode the password.