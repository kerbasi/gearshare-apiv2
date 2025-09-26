## Auto Parts API Plan (Scalable, AWS-ready)

### Stack
- **Backend**: .NET 8 Web API, C#
- **Data**: SQL Server via EF Core (SqlServer provider)
- **Auth**: ASP.NET Identity + JWT (access + refresh tokens), roles: admin, seller, customer
- **Docs/Validation/Logging**: Swagger/OpenAPI, FluentValidation, Serilog
- **Mediator/CQRS**: MediatR (commands/queries + pipeline behaviors)

### Domain (current DB)
- `Manufacturers(manufacturer_id, name UNIQUE, country)`
- `Cars(car_id, make, model, year_start, year_end)`
- `Parts(part_id, sku UNIQUE, name, description, price >=0, stock_quantity >=0 DEFAULT 0, manufacturer_id FK)`
- `PartCompatibility(part_id, car_id)` composite PK linking parts↔cars

### Architecture
- Layered: `Api` → `Application` (use-cases/validators) → `Domain` (entities) → `Infrastructure` (EF Core, external services)
- Feature modules: `Catalog` (parts, manufacturers, cars, fitment), `Identity`, `Cart`, `Orders`, `Payments`, `Inventory`, `Media`, `Search`
- Patterns: pagination, DTO projections, optimistic concurrency (`rowversion` on inventory), idempotency keys for checkout, Outbox → SQS for reliable events, background workers for async flows

### CQRS in the Application layer (explicit)
- Use MediatR for command/query segregation:
  - Commands mutate state and return minimal results (ids/status). Examples: `CreatePartCommand`, `UpdateStockCommand`, `CreateOrderCommand`.
  - Queries never mutate state and return DTOs. Examples: `GetPartsQuery`, `GetPartsForCarQuery`, `GetCarsForPartQuery`.
- Pipeline behaviors:
  - Validation (FluentValidation) on commands/queries
  - Authorization (roles/claims) before handler execution
  - Logging/Tracing (Serilog + OpenTelemetry/Activity)
  - TransactionBehavior for commands (EF Core Unit of Work)
- Read side:
  - Start with EF Core LINQ projections to DTOs
  - Optionally Dapper for hot, complex reads
  - Consider specialized read models populated via Outbox → SQS → projection worker when needed
- Errors: return RFC7807 problem+json consistently from handlers

### NuGet packages
- MediatR
- MediatR.Extensions.Microsoft.DependencyInjection
- FluentValidation
- Serilog.AspNetCore
 - (Optional later) HotChocolate.AspNetCore (GraphQL)
 - Microsoft.AspNetCore.Mvc.Versioning
 - Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer (for Swagger grouping)

### Advanced searches
- For complex search UIs, provide `POST /parts/search` accepting a JSON payload with filter criteria, sort, and pagination. This avoids long URLs, header size limits, and is easier to extend.
- Keep `GET /parts` for simple, cacheable queries.

### GraphQL (future, side-by-side with REST)
- Add a GraphQL endpoint to let clients shape responses (e.g., a part with its manufacturer and compatible cars in one request).
- Suggested stack: Hot Chocolate (schema-first), persisted queries, authorization directives, and depth/complexity limits.

### Endpoints (initial)
- Versioned base path: `/api/v1/`
- Manufacturers: GET list/detail, POST, PUT, DELETE
- Cars: GET with filters (make, model, year), CRUD
- Parts: 
  - GET /api/v1/parts?sku=&q=&make=&model=&year=&minPrice=&maxPrice=&manufacturerId=
  - GET /api/v1/parts/{id}, CRUD
  - POST /api/v1/parts/search with JSON body for complex filters and paging
- Compatibility:
  - GET /api/v1/cars/{carId}/parts
  - GET /api/v1/parts/{partId}/cars
  - POST/DELETE link { partId, carId }

### API Versioning
- Strategy: default to URL path versioning (`/api/v1/...`), support header `Api-Version` for clients that prefer it.
- Defaults: assume default version when unspecified (v1), deprecated versions return `Sunset`/`Deprecation` headers.
- Swagger: group by API version using ApiExplorer so UI shows v1, v2 tabs.
- Routing example: `[ApiVersion("1.0")]`, route `api/v{version:apiVersion}/[controller]`.
- Policy: maintain N-1 versions; add breaking changes only in new versions.

### Indices (SQL Server)
- Cars: (make, model, year_start, year_end)
- Parts: sku UNIQUE, (manufacturer_id, price)
- PartCompatibility: PK(part_id, car_id), nonclustered on (car_id)

### Growth roadmap
- Auth: register/login, email confirm, password reset, refresh tokens, roles
- Cart/Checkout: carts, addresses, taxes placeholder, payment intent, order creation
- Orders: statuses, order items, totals; events to SQS; email receipts
- Inventory: reservations on checkout, decrement on capture, restock on cancel/refund
- Media: S3 pre-signed uploads, `PartMedia` table
- Search: server-side filtering; optional OpenSearch later
- Categories (optional now): `Categories`, `PartCategories`
 - GraphQL alongside REST for flexible graph queries (parts ⇄ cars ⇄ manufacturers)

### AWS Deployment
- DB: Amazon RDS for SQL Server
- Compute: Containerized (Docker) → ECS Fargate behind ALB (blue/green deploys)
- Secrets: AWS Secrets Manager (DB, JWT)
- Storage/CDN: S3 for media, CloudFront
- Caching: ElastiCache (Redis)
- Async: SQS for outbox/notifications/emails
- Observability: CloudWatch logs/metrics, OpenTelemetry + AWS X-Ray, health checks
- CI/CD: GitHub Actions → build/test → push image to ECR → deploy ECS; run EF migrations on deploy

### Non-functional
- Rate limiting; request size/time limits
- Strong input validation with RFC7807 problem+json responses
- OpenAPI-first; generate typed client for React
- Seed data; integration tests for critical flows

### Near-term tasks
1) Scaffold solution (layers, EF Core, Swagger/Serilog/FluentValidation)
2) Implement Catalog entities, DbContext, migrations
3) Deliver initial endpoints (list/search, fitment queries)
4) Add JWT auth (Identity), roles
5) Add S3 media upload (pre-signed URLs)
6) Containerize, ECS task/Service, GitHub Actions pipeline
7) Add Redis caching for hot searches
8) Seed data and integration tests


