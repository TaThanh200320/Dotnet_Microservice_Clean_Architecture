using Application.Interfaces.Services.Elasticsearch;
using Contracts.ApiWrapper;
using Elastic.Clients.Elasticsearch;
using IdentityDomain.Aggregates.AuditLogs;
using Mediator;
using SharedKernel.Models;

namespace IdentityApplication.Features.AuditLogs.Queries;

public class ListAuditLogHandler(IElasticsearchServiceFactory? elasticsearch = null)
    : IRequestHandler<ListAuditLogQuery, Result<PaginationResponse<ListAuditLogResponse>>>
{
    public async ValueTask<Result<PaginationResponse<ListAuditLogResponse>>> Handle(
        ListAuditLogQuery request,
        CancellationToken cancellationToken
    )
    {
        if (elasticsearch == null)
        {
            throw new NotImplementedException("Elasticsearch has not enabled");
        }

        SearchResponse<AuditLog> searchResponse = await elasticsearch
            .Get<AuditLog>()
            .ListAsync(request);

        PaginationResponse<ListAuditLogResponse> paginationResponse =
            new(
                searchResponse.Documents.ToListAuditLogResponse(),
                (int)searchResponse.Total,
                request.Page,
                request.PageSize
            );

        return Result<PaginationResponse<ListAuditLogResponse>>.Success(paginationResponse);
    }
}
