---
name: blazor-components
description: Ensures proper Blazor Server component design with MudBlazor patterns
---

# Blazor Components Best Practices

## Component Structure

### File Organization
```
src/Corelio.Blazor/
├── Components/
│   ├── Common/          # Reusable components
│   │   ├── LoadingSpinner.razor
│   │   ├── ErrorBoundary.razor
│   │   └── ConfirmDialog.razor
│   ├── Products/        # Feature-specific
│   │   ├── ProductList.razor
│   │   ├── ProductForm.razor
│   │   └── ProductCard.razor
│   └── Layout/
│       ├── MainLayout.razor
│       ├── NavMenu.razor
│       └── UserMenu.razor
├── Pages/               # Routable pages
│   ├── Index.razor
│   ├── Products/
│   │   ├── ProductsPage.razor
│   │   └── ProductDetailsPage.razor
└── Services/            # Client-side services
    └── ApiClient.cs
```

### Component Template
```razor
@* ProductList.razor *@
@using Corelio.Application.Products.Queries
@using MudBlazor
@inject IProductsClient ProductsClient
@inject ISnackbar Snackbar

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">Products</MudText>
        </CardHeaderContent>
        <CardHeaderActions>
            <MudButton Variant="Variant.Filled"
                       Color="Color.Primary"
                       StartIcon="@Icons.Material.Filled.Add"
                       OnClick="@OnCreateProduct">
                New Product
            </MudButton>
        </CardHeaderActions>
    </MudCardHeader>

    <MudCardContent>
        @if (_loading)
        {
            <MudProgressLinear Indeterminate="true" />
        }
        else if (_products.Any())
        {
            <MudTable Items="@_products" Hover="true" Breakpoint="Breakpoint.Sm">
                <HeaderContent>
                    <MudTh>Name</MudTh>
                    <MudTh>SKU</MudTh>
                    <MudTh>Price</MudTh>
                    <MudTh>Actions</MudTh>
                </HeaderContent>
                <RowTemplate>
                    <MudTd DataLabel="Name">@context.Name</MudTd>
                    <MudTd DataLabel="SKU">@context.Sku</MudTd>
                    <MudTd DataLabel="Price">@context.Price.ToString("C2")</MudTd>
                    <MudTd DataLabel="Actions">
                        <MudIconButton Icon="@Icons.Material.Filled.Edit"
                                       Size="Size.Small"
                                       OnClick="@(() => OnEditProduct(context.Id))" />
                        <MudIconButton Icon="@Icons.Material.Filled.Delete"
                                       Size="Size.Small"
                                       Color="Color.Error"
                                       OnClick="@(() => OnDeleteProduct(context.Id))" />
                    </MudTd>
                </RowTemplate>
            </MudTable>
        }
        else
        {
            <MudAlert Severity="Severity.Info">No products found</MudAlert>
        }
    </MudCardContent>
</MudCard>

@code {
    private List<ProductDto> _products = new();
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            _loading = true;
            _products = await ProductsClient.GetAllAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading products: {ex.Message}", Severity.Error);
        }
        finally
        {
            _loading = false;
        }
    }

    private void OnCreateProduct()
    {
        // Navigate to create page or show dialog
    }

    private void OnEditProduct(Guid id)
    {
        // Navigate to edit page or show dialog
    }

    private async Task OnDeleteProduct(Guid id)
    {
        // Show confirmation dialog
    }
}
```

## MudBlazor Patterns

### Form with Validation
```razor
@* ProductForm.razor *@
<EditForm Model="@_model" OnValidSubmit="@HandleSubmit">
    <FluentValidationValidator />

    <MudCard>
        <MudCardContent>
            <MudTextField Label="Product Name"
                          @bind-Value="_model.Name"
                          For="@(() => _model.Name)"
                          Variant="Variant.Outlined"
                          Required="true" />

            <MudTextField Label="SKU"
                          @bind-Value="_model.Sku"
                          For="@(() => _model.Sku)"
                          Variant="Variant.Outlined"
                          Required="true" />

            <MudNumericField Label="Price"
                             @bind-Value="_model.Price"
                             For="@(() => _model.Price)"
                             Variant="Variant.Outlined"
                             Format="C2"
                             Min="0" />

            <MudSelect Label="Category"
                       @bind-Value="_model.CategoryId"
                       For="@(() => _model.CategoryId)"
                       Variant="Variant.Outlined"
                       Required="true">
                @foreach (var category in _categories)
                {
                    <MudSelectItem Value="@category.Id">@category.Name</MudSelectItem>
                }
            </MudSelect>

            <MudSwitch @bind-Checked="_model.IsActive"
                       Label="Active"
                       Color="Color.Primary" />
        </MudCardContent>

        <MudCardActions>
            <MudButton ButtonType="ButtonType.Submit"
                       Variant="Variant.Filled"
                       Color="Color.Primary"
                       Disabled="@_saving">
                @if (_saving)
                {
                    <MudProgressCircular Size="Size.Small" Indeterminate="true" />
                    <MudText Class="ml-2">Saving...</MudText>
                }
                else
                {
                    <MudText>Save</MudText>
                }
            </MudButton>
            <MudButton OnClick="@OnCancel" Variant="Variant.Text">Cancel</MudButton>
        </MudCardActions>
    </MudCard>
</EditForm>

@code {
    [Parameter] public Guid? ProductId { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }
    [Parameter] public EventCallback OnCancelled { get; set; }

    private ProductFormModel _model = new();
    private List<CategoryDto> _categories = new();
    private bool _saving = false;

    protected override async Task OnInitializedAsync()
    {
        _categories = await CategoriesClient.GetAllAsync();

        if (ProductId.HasValue)
        {
            var product = await ProductsClient.GetByIdAsync(ProductId.Value);
            _model = new ProductFormModel
            {
                Name = product.Name,
                Sku = product.Sku,
                Price = product.Price,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive
            };
        }
    }

    private async Task HandleSubmit()
    {
        try
        {
            _saving = true;

            if (ProductId.HasValue)
            {
                await ProductsClient.UpdateAsync(ProductId.Value, _model);
                Snackbar.Add("Product updated successfully", Severity.Success);
            }
            else
            {
                await ProductsClient.CreateAsync(_model);
                Snackbar.Add("Product created successfully", Severity.Success);
            }

            await OnSaved.InvokeAsync();
        }
        catch (ApiException ex)
        {
            Snackbar.Add($"Error saving product: {ex.Message}", Severity.Error);
        }
        finally
        {
            _saving = false;
        }
    }

    private async Task OnCancel()
    {
        await OnCancelled.InvokeAsync();
    }
}
```

### Dialog
```razor
@* ConfirmDialog.razor *@
@inject IDialogService DialogService

<MudDialog>
    <DialogContent>
        <MudText>@Message</MudText>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Cancel</MudButton>
        <MudButton Color="Color.Error" Variant="Variant.Filled" OnClick="Confirm">
            @ConfirmText
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public string Message { get; set; } = "Are you sure?";
    [Parameter] public string ConfirmText { get; set; } = "Confirm";

    private void Cancel() => MudDialog.Cancel();
    private void Confirm() => MudDialog.Close(DialogResult.Ok(true));
}

// Usage in parent component
private async Task OnDeleteProduct(Guid id)
{
    var parameters = new DialogParameters
    {
        { "Message", "Are you sure you want to delete this product?" },
        { "ConfirmText", "Delete" }
    };

    var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirm Delete", parameters);
    var result = await dialog.Result;

    if (!result.Canceled)
    {
        await ProductsClient.DeleteAsync(id);
        Snackbar.Add("Product deleted", Severity.Success);
        await LoadProductsAsync();
    }
}
```

## State Management

### Cascading Parameters
```razor
@* App.razor *@
<CascadingAuthenticationState>
    <CascadingValue Value="@_tenantInfo">
        <Router AppAssembly="@typeof(App).Assembly">
            <Found Context="routeData">
                <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
            </Found>
            <NotFound>
                <PageTitle>Not found</PageTitle>
                <LayoutView Layout="@typeof(MainLayout)">
                    <p>Sorry, there's nothing at this address.</p>
                </LayoutView>
            </NotFound>
        </Router>
    </CascadingValue>
</CascadingAuthenticationState>

@code {
    private TenantInfo _tenantInfo = new();

    protected override async Task OnInitializedAsync()
    {
        _tenantInfo = await TenantService.GetCurrentTenantAsync();
    }
}

// Child component
@code {
    [CascadingParameter] private TenantInfo TenantInfo { get; set; } = null!;
}
```

### Scoped Services
```csharp
// Program.cs
builder.Services.AddScoped<AppState>();

// AppState.cs
public class AppState
{
    public event Action? OnChange;

    private string? _selectedProduct;

    public string? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

// Component usage
@inject AppState AppState
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        AppState.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        AppState.OnChange -= StateHasChanged;
    }
}
```

## Component Lifecycle

```razor
@code {
    // 1. SetParametersAsync - Before parameter values are set
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
    }

    // 2. OnInitialized/OnInitializedAsync - Component is initialized
    protected override async Task OnInitializedAsync()
    {
        // Load initial data
        await LoadDataAsync();
    }

    // 3. OnParametersSet/OnParametersSetAsync - After parameters are set
    protected override async Task OnParametersSetAsync()
    {
        // React to parameter changes
        if (ProductId.HasValue)
        {
            await LoadProductAsync(ProductId.Value);
        }
    }

    // 4. OnAfterRender/OnAfterRenderAsync - After component renders
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Initialize JavaScript interop
            await JSRuntime.InvokeVoidAsync("initializePage");
        }
    }

    // Cleanup
    public void Dispose()
    {
        // Unsubscribe from events, dispose resources
    }
}
```

## Performance Optimization

### Virtualization for Large Lists
```razor
<MudVirtualize Items="@_products" Context="product">
    <MudListItem>
        @product.Name - @product.Price.ToString("C2")
    </MudListItem>
</MudVirtualize>
```

### Debounced Search
```razor
<MudTextField Label="Search"
              @bind-Value="_searchTerm"
              Immediate="true"
              DebounceInterval="500"
              OnDebounceIntervalElapsed="@OnSearchAsync" />

@code {
    private string _searchTerm = string.Empty;

    private async Task OnSearchAsync()
    {
        await LoadProductsAsync(_searchTerm);
    }
}
```

### ShouldRender Optimization
```razor
@code {
    private bool _dataChanged = false;

    protected override bool ShouldRender()
    {
        if (_dataChanged)
        {
            _dataChanged = false;
            return true;
        }
        return false;
    }

    private async Task LoadDataAsync()
    {
        _products = await ProductsClient.GetAllAsync();
        _dataChanged = true;
        StateHasChanged();
    }
}
```

## Error Handling

### Error Boundary
```razor
@* ErrorBoundary.razor *@
<ErrorBoundary @ref="_errorBoundary">
    <ChildContent>
        @ChildContent
    </ChildContent>
    <ErrorContent>
        <MudAlert Severity="Severity.Error">
            <MudText Typo="Typo.h6">An error occurred</MudText>
            <MudText>@context.Message</MudText>
            <MudButton OnClick="@Recover" Variant="Variant.Text">Retry</MudButton>
        </MudAlert>
    </ErrorContent>
</ErrorBoundary>

@code {
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;

    private ErrorBoundary? _errorBoundary;

    private void Recover()
    {
        _errorBoundary?.Recover();
    }
}

// Usage
<ErrorBoundary>
    <ProductList />
</ErrorBoundary>
```

### Try-Catch with Snackbar
```razor
@code {
    private async Task DeleteProductAsync(Guid id)
    {
        try
        {
            await ProductsClient.DeleteAsync(id);
            Snackbar.Add("Product deleted successfully", Severity.Success);
            await LoadProductsAsync();
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            Snackbar.Add("Product not found", Severity.Warning);
        }
        catch (ApiException ex) when (ex.StatusCode == 403)
        {
            Snackbar.Add("You don't have permission to delete products", Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"An error occurred: {ex.Message}", Severity.Error);
        }
    }
}
```

## Authentication & Authorization

### Authorize Attribute
```razor
@page "/products"
@attribute [Authorize(Roles = "Admin,Manager")]

<h3>Products (Admin Only)</h3>
```

### Conditional Rendering
```razor
<AuthorizeView Roles="Admin,Manager">
    <Authorized>
        <MudButton Color="Color.Error" OnClick="@DeleteProduct">Delete</MudButton>
    </Authorized>
    <NotAuthorized>
        <MudText>You don't have permission to delete products</MudText>
    </NotAuthorized>
</AuthorizeView>

<AuthorizeView Policy="CanEditProducts">
    <MudButton OnClick="@EditProduct">Edit</MudButton>
</AuthorizeView>
```

## API Client

### Generated Client (NSwag/Refit)
```csharp
// Generated by NSwag
public interface IProductsClient
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<ProductDto> CreateAsync(CreateProductRequest request);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request);
    Task DeleteAsync(Guid id);
}

// Program.cs
builder.Services.AddHttpClient<IProductsClient, ProductsClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/");
});
```

## Best Practices Checklist

### Component Design
- [ ] Use MudBlazor components consistently
- [ ] Separate presentation from logic
- [ ] Keep components small and focused
- [ ] Use proper parameter validation
- [ ] Implement IDisposable when needed

### Performance
- [ ] Use virtualization for large lists
- [ ] Debounce user inputs
- [ ] Optimize ShouldRender
- [ ] Lazy load heavy components
- [ ] Use streaming rendering for long operations

### Error Handling
- [ ] Wrap components in ErrorBoundary
- [ ] Handle API exceptions gracefully
- [ ] Show user-friendly error messages
- [ ] Log errors to server

### State Management
- [ ] Use cascading parameters for global state
- [ ] Use scoped services for shared state
- [ ] Minimize state updates
- [ ] Notify components of state changes

### Security
- [ ] Use [Authorize] attribute on pages
- [ ] Check permissions in code
- [ ] Never expose sensitive data in client
- [ ] Validate all user inputs

---

**Remember**: Blazor Server maintains a SignalR connection - optimize for performance!
