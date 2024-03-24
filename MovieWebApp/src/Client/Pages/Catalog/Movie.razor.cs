using FSH.WebApi.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using MovieWebApp.Client.Components.EntityTable;
using MovieWebApp.Client.Infrastructure.ApiClient;

namespace MovieWebApp.Client.Pages.Catalog;

public partial class Movie
{
    [Inject]
    protected IMoviesClient MoviesClient { get; set; } = default!;
    [Inject]
    protected IGenresClient GenresClient { get; set; } = default!;

    protected EntityServerTableContext<MovieDto, Guid, MovieDto> Context { get; set; } = default!;

    private EntityTable<MovieDto, Guid, MovieDto> _table = default!;

    private Guid _searchGenreId;

    private Guid SearchGenreId
    {
        get => _searchGenreId;
        set
        {
            _searchGenreId = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private IEnumerable<string> _orderByOptions { get; set; } = new HashSet<string>();
    private IEnumerable<string> OrderByOptions
    {
        get => _orderByOptions;
        set
        {
            _orderByOptions = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private string _value { get; set; } = "Nothing selected";

    protected override void OnInitialized() =>
        Context = new(
            entityName: L["Movie"],
            entityNamePlural: L["Movie"],
            entityResource: FSHResource.Movies,
            deleteAction: string.Empty,
            createAction: string.Empty,
            updateAction: string.Empty,
            exportAction: string.Empty,
            fields: new()
            {
                new(movie => movie.Title, L["Title"], "Title"),
                new(movie => movie.ReleaseDate.ToString("dddd, dd MMMM yyyy"), L["Release Date"], "ReleaseDate"),
                new(movie => movie.Genre?.Name, L["Genre"], "Genre"),
                new(movie => movie.Popularity, L["Popularity"], "Popularity"),
                new(movie => movie.VoteCount, L["VoteCount"], "Vote Count"),
                new(movie => movie.VoteAverage, L["Vote Average"], "VoteAverage"),
                new(movie => movie.Language?.Abbreviation, L["Abbreviation"], "Abbreviation"),
                new(movie => movie.PosterUrl, L["PosterUrl"], "PosterUrl"),
            },
            enableAdvancedSearch: true,
            idFunc: movie => movie.Id,
            searchFunc: async filter =>
            {
                var movieFilter = filter.Adapt<SearchMoviesRequest>();
                movieFilter.Title = movieFilter.Keyword;
                movieFilter.GenreId = SearchGenreId == default ? null : SearchGenreId;
                if(OrderByOptions is not null && OrderByOptions.Count() > 0)
                {
                    movieFilter.OrderBy = OrderByOptions.ToList();
                }

                var result = await MoviesClient.SearchAsync(movieFilter);
                return result.Adapt<PaginationResponse<MovieDto>>();
            });
}
