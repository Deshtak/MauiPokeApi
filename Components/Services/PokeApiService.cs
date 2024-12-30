using RestSharp;
using System.Text.Json.Serialization;

public class PokeApiService
{
    private readonly RestClient _client;

    public PokeApiService()
    {
        _client = new RestClient("https://pokeapi.co/api/v2/");
    }


    public class SharedPokemonService
    {
        public string? SelectedPokemon { get; private set; }

        public event Action? OnPokemonSelected;

        public void SetSelectedPokemon(string pokemonName)
        {
            SelectedPokemon = pokemonName;
            OnPokemonSelected?.Invoke();
        }
    }

    public class SharedMoveService
    {
        public string? SelectedMove { get; private set; }

        public event Action? OnMoveSelected;

        public void SetSelectedMove(string pokemonMove)
        {
            SelectedMove = pokemonMove;
            OnMoveSelected?.Invoke();
        }
    }


    // Método para obtener un solo Pokémon por nombre
    public async Task<Pokemon?> GetPokemonAsync(string name)
    {
        var request = new RestRequest($"pokemon/{name}", Method.Get);
        return await _client.GetAsync<Pokemon>(request);
    }



    public async Task<List<ListaInfo>> GetPokeListAsync()
    {
        try
        {
            var request = new RestRequest("pokemon?limit=100000&offset=0", Method.Get);
            var response = await _client.GetAsync<ListPokemon>(request);

            if (response == null || response.Results == null)
            {
                throw new Exception("No se pudo obtener la lista de Pokémon de la API.");
            }

            return response.Results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la lista de Pokémon: {ex.Message}");
            return new List<ListaInfo>();
        }
    }

    public async Task<Movelist> GetMovesAsync(string name)
    {
        var request = new RestRequest($"move/{name}", Method.Get);
        return await _client.GetAsync<Movelist>(request);
    }


    public async Task<List<string>> GetAllMovesAsync()
    {
        var request = new RestRequest("move?limit=1000", Method.Get);
        var response = await _client.GetAsync<MoveListResponse>(request);

        return response?.Results?.Select(r => r.Name).ToList() ?? new List<string>();
    }




    //Cadena evolutiva

    public async Task<string?> GetEvolutionChainUrlAsync(string pokemonName)
    {
        try
        {
            var request = new RestRequest($"pokemon-species/{pokemonName}", Method.Get);
            var response = await _client.GetAsync<SpeciesResponse>(request);

            if (response == null || string.IsNullOrEmpty(response.evolution_chain?.url))
            {
                throw new Exception("No se pudo obtener la URL de la cadena evolutiva.");
            }

            return response.evolution_chain.url;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la URL de la cadena evolutiva: {ex.Message}");
            return null;
        }
    }

    // Obtener cadena evolutiva
    public async Task<List<string>> GetEvolutionChainAsync(string url)
    {
        try
        {
            var request = new RestRequest(url, Method.Get);
            var response = await _client.GetAsync<EvolutionChainResponse>(request);

            if (response == null || response.chain == null)
            {
                throw new Exception("No se pudo obtener la cadena evolutiva.");
            }

            var evolutionNames = new List<string>();
            var currentChain = response.chain;

            // Recorremos la cadena evolutiva para extraer nombres
            while (currentChain != null)
            {
                evolutionNames.Add(currentChain.species.name);
                currentChain = currentChain.evolves_to.FirstOrDefault();
            }

            return evolutionNames;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la cadena evolutiva: {ex.Message}");
            return new List<string>();
        }
    }

    //clase para movimientos en lista

    public class MoveListResponse
    {
        public List<MoveResult> Results { get; set; }
    }

    public class MoveResult
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }



    //Clase para la cadena evolutiva


    public class SpeciesResponse
    {
        public EvolutionChainInfo evolution_chain { get; set; }
    }

    public class EvolutionChainInfo
    {
        public string url { get; set; }
    }

    // Clases para la respuesta de `evolution-chain`
    public class EvolutionChainResponse
    {
        public Chain chain { get; set; }
    }

    public class Chain
    {
        public Species species { get; set; }
        public List<Chain> evolves_to { get; set; }
    }

    public class Species
    {
        public string name { get; set; }
    }


    // Clases para representar la respuesta de la API

    //clases para el listado de pokemones completo

    public class ListPokemon
    {
        public List<ListaInfo> Results { get; set; } = new List<ListaInfo>();
    }

    public class ListaInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    //


    // Clases para los detalles de cada Pokémon, primera pagina
    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TypeWrapper> Types { get; set; }
        public List<AbilityWrapper> Abilities { get; set; }
        public List<MoveWrapper> Moves { get; set; }
    }

    public class Movelist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Power { get; set; }
        public int? Pp { get; set; }
        public int? Priority { get; set; }
        public int? Accuracy { get; set; }
        [JsonPropertyName("effect_entries")]
        public List<EffectEntries> Effect_Entries { get; set; }



    }

    public class EffectEntries
    {
        [JsonPropertyName("effect")]
        public string Effect { get; set; }

        [JsonPropertyName("short_effect")]
        public string ShortEffect { get; set; }
    }

    public class TypeWrapper
    {
        public TypeInfo Type { get; set; }
    }

    public class TypeInfo
    {
        public string Name { get; set; }
    }

    public class MoveWrapper
    {
        public MoveInfo Move { get; set; }
    }

    public class MoveInfo
    {
        public string Name { get; set; }
    }

    public class AbilityWrapper
    {
        public AbilityInfo Ability { get; set; }
    }

    public class AbilityInfo
    {
        public string Name { get; set; }
    }
}