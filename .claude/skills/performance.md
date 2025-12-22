---
name: performance-optimization
description: Ensures optimal performance for .NET 10, EF Core, and PostgreSQL
---

# Instructions

## Database Performance
1. Use AsNoTracking() for all read-only queries
2. Implement proper indexes on foreign keys and frequently queried columns
3. Use composite indexes for multi-column queries (tenant_id + other columns)
4. Avoid N+1 queries - use Include() or explicit loading
5. Use compiled queries for hot paths (frequently executed queries)
6. Implement pagination for large result sets (Skip + Take)
7. Use projection (Select) to fetch only needed columns, not entire entities
8. Use bulk operations for multiple inserts/updates (EF Core 7+ BulkUpdate)
9. Monitor query execution time - target <100ms for most queries
10. Use connection pooling (configured automatically by EF Core)
11. Implement database caching with Redis for tenant configuration
12. Use database-side filtering before client-side filtering

## API Performance
1. Implement response caching for GET endpoints
2. Use output caching for frequently accessed data
3. Implement rate limiting to prevent abuse
4. Use compression (Gzip/Brotli) for responses
5. Minimize serialization overhead with System.Text.Json
6. Use minimal APIs for hot paths (lighter than controllers)
7. Implement proper HTTP caching headers (ETag, Cache-Control)
8. Use async/await consistently (never block async code)
9. Avoid Task.Result and Task.Wait (causes deadlocks)
10. Use ValueTask for frequently called hot paths

## Caching Strategy
1. Cache tenant configuration in Redis (30-minute TTL)
2. Cache SAT catalogs (product codes, unit codes) - rarely change
3. Cache user permissions (5-minute TTL)
4. Implement distributed caching with Redis for multi-instance deployments
5. Use memory cache for single-instance or non-critical data
6. Implement cache invalidation on updates
7. Use cache-aside pattern (check cache first, then database)
8. Set appropriate TTL based on data volatility

## Blazor Performance
1. Use @key directive for list items to optimize rendering
2. Implement virtualization for long lists (Virtualize component)
3. Use OnAfterRender for DOM operations, not OnInitialized
4. Minimize StateHasChanged() calls
5. Use event throttling/debouncing for rapid events (search input)
6. Implement proper loading states (skeleton, progress indicators)
7. Lazy load components with DynamicComponent
8. Optimize SignalR connection (reconnection logic, message batching)

## Memory Management
1. Dispose IDisposable resources properly (using statements)
2. Implement IAsyncDisposable for async cleanup
3. Avoid memory leaks (unsubscribe from events, dispose handlers)
4. Use object pooling for frequently allocated objects (ArrayPool<T>)
5. Minimize allocations in hot paths
6. Use Span<T> and Memory<T> for high-performance scenarios
7. Monitor memory usage with Application Insights

## Performance Targets
- Product search: <500ms (P95)
- Add to cart: <200ms (P95)
- Complete sale: <3 seconds (P95)
- CFDI generation: <5 seconds (P95)
- Dashboard load: <2 seconds (P95)
- API response: <1 second (P95)

## Monitoring
1. Use Application Insights for performance monitoring
2. Track response times with OpenTelemetry (via Aspire)
3. Monitor database query performance
4. Set up alerts for performance degradation
5. Use profiling tools (dotTrace, dotMemory) for optimization
6. Monitor connection pool exhaustion
7. Track Redis cache hit/miss ratios
