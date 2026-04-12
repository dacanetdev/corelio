using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Cfdi;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Cfdi;

/// <summary>
/// Implementation of CFDI HTTP service using AuthenticatedHttpClient.
/// </summary>
public class CfdiHttpService(AuthenticatedHttpClient httpClient, ILogger<CfdiHttpService> logger)
    : ICfdiHttpService
{
    private const string InvoicesUrl = "/api/v1/invoices";
    private const string SettingsUrl = "/api/v1/tenants/cfdi";

    public async Task<Result<PagedResult<InvoiceListModel>>> GetInvoicesAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(status))
            {
                queryParams.Add($"status={Uri.EscapeDataString(status)}");
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            }

            var url = $"{InvoicesUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<InvoiceListModel>>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<InvoiceListModel>>.Success(result);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<InvoiceListModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading invoices");
            return Result<PagedResult<InvoiceListModel>>.Failure($"Error loading invoices: {ex.Message}");
        }
    }

    public async Task<Result<InvoiceModel>> GetInvoiceByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{InvoicesUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var invoice = await response.Content.ReadFromJsonAsync<InvoiceModel>(
                    JsonOptions.Default, cancellationToken);
                if (invoice is not null)
                {
                    return Result<InvoiceModel>.Success(invoice);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<InvoiceModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading invoice {Id}", id);
            return Result<InvoiceModel>.Failure($"Error loading invoice: {ex.Message}");
        }
    }

    public async Task<Result<Guid>> GenerateInvoiceAsync(
        GenerateInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{InvoicesUrl}/generate", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadFromJsonAsync<GeneratedIdResponse>(
                    JsonOptions.Default, cancellationToken);
                if (body is not null)
                {
                    return Result<Guid>.Success(body.Id);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<Guid>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating invoice for sale {SaleId}", request.SaleId);
            return Result<Guid>.Failure($"Error generating invoice: {ex.Message}");
        }
    }

    public async Task<Result<bool>> StampInvoiceAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{InvoicesUrl}/{id}/stamp", new { }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error stamping invoice {Id}", id);
            return Result<bool>.Failure($"Error stamping invoice: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CancelInvoiceAsync(
        Guid id,
        string reason,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync(
                $"{InvoicesUrl}/{id}?reason={Uri.EscapeDataString(reason)}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cancelling invoice {Id}", id);
            return Result<bool>.Failure($"Error cancelling invoice: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> DownloadXmlAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{InvoicesUrl}/{id}/xml", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                return Result<byte[]>.Success(bytes);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<byte[]>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading XML for invoice {Id}", id);
            return Result<byte[]>.Failure($"Error downloading XML: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> DownloadPdfAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{InvoicesUrl}/{id}/pdf", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                return Result<byte[]>.Success(bytes);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<byte[]>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading PDF for invoice {Id}", id);
            return Result<byte[]>.Failure($"Error downloading PDF: {ex.Message}");
        }
    }

    public async Task<Result<CfdiSettingsModel>> GetSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{SettingsUrl}/settings", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var settings = await response.Content.ReadFromJsonAsync<CfdiSettingsModel>(
                    JsonOptions.Default, cancellationToken);
                if (settings is not null)
                {
                    return Result<CfdiSettingsModel>.Success(settings);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<CfdiSettingsModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading CFDI settings");
            return Result<CfdiSettingsModel>.Failure($"Error loading settings: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateSettingsAsync(
        UpdateCfdiSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"{SettingsUrl}/settings", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating CFDI settings");
            return Result<bool>.Failure($"Error saving settings: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UploadCertificateAsync(
        Stream fileStream,
        string fileName,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(fileStream);
            content.Add(fileContent, "file", fileName);
            content.Add(new StringContent(password), "password");

            var response = await httpClient.PostAsync($"{SettingsUrl}/certificate", content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading certificate");
            return Result<bool>.Failure($"Error uploading certificate: {ex.Message}");
        }
    }

    private sealed record GeneratedIdResponse(Guid Id);
}
