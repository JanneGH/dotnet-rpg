namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>() {
            new Character(),
            new Character { Id = 1, Name = "Ali Baba" }
        };

        public async Task<ServiceResponse<List<Character>>> AddCharacter(Character newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<Character>>();
            characters.Add(newCharacter);
            serviceResponse.Data = characters;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<Character>>();
            serviceResponse.Data = characters;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Character>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<Character>();

            // Use LINQ to find character by Id. 
            var character = characters.FirstOrDefault(character =>
                character.Id == id
            );

            serviceResponse.Data = character;
            return serviceResponse;
        }
    }
}

/// More on the ServiceResponse Wrapper:

// BEFORE 
/*  public async Task<List<Character>> GetAllCharacters()
    {
        return characters;
    }*/

// AFTER
/*  public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
    {
        var serviceResponse = new ServiceResponse<List<Character>>();
        serviceResponse.Data = characters;
        return serviceResponse;
    }*/