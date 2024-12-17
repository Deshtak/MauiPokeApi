using MauiPokeApi.Components.Pages;
using RestSharp;

public class PokeApiService
{
    private readonly RestClient _client;

    public PokeApiService()
    {
        _client = new RestClient("https://pokeapi.co/api/v2/");
    }

    // Método para obtener un solo Pokémon por nombre
    public async Task<Pokemon?> GetPokemonAsync(string name)
    {
        var request = new RestRequest($"pokemon/{name}", Method.Get);
        return await _client.GetAsync<Pokemon>(request);
    }

    public async Task<List<ListaInfo>> GetPokeListAsync()
    {
        var request = new RestRequest("pokemon?limit=100000&offset=0", Method.Get);
        var response = await _client.GetAsync<ListPokemon>(request);
        return response?.Results ?? new List<ListaInfo>();
    }



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