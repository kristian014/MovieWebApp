using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MovieWebApp.Client.Infrastructure.ApiClient;
using MovieWebApp.Client.Shared;
using MudBlazor;

namespace MovieWebApp.Client.Pages.Catalog;

public class GenreAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<GenreAutocomplete> L { get; set; } = default!;
    [Inject]
    private IGenresClient GenresClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<GenreDto> _genres = new List<GenreDto>();

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Genre"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchGenres;
        ToStringFunc = GetGenreName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender &&
            _value != default &&
            await ApiHelper.ExecuteCallGuardedAsync(
                () => GenresClient.GetAsync(_value), Snackbar) is { } brand)
        {
            _genres.Add(brand);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<Guid>> SearchGenres(string value)
    {
        var filter = new SearchGenresRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => GenresClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfGenreDto response)
        {
            _genres = response.Data.ToList();
        }

        return _genres.Select(x => x.Id);
    }

    private string GetGenreName(Guid id) =>
        _genres.Find(b => b.Id == id)?.Name ?? string.Empty;
}
