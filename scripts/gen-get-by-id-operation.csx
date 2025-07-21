#!/usr/bin/env dotnet-script
/*
|--------------------------------------------------------------------------
| InnHotel API Get By ID Operation Generator
|--------------------------------------------------------------------------
|
| Quick Start:
|   dotnet script ./scripts/gen-get-by-id-operation.csx -- --entity <singular> --plural <plural>
|
| Example:
|   dotnet script ./scripts/gen-get-by-id-operation.csx -- --entity Room --plural Rooms
|
| For detailed documentation, see:
|   docs/dev/gen-operation-scripts.md
|
*/

#r "nuget: System.CommandLine, 2.0.0-beta4"  // Pinned version for better caching

using System.CommandLine;
using System.CommandLine.Invocation;
using static System.Console;
using System.Text.RegularExpressions;

// Configuration
public static class Config
{
    public static string ProjectRoot = Directory.GetCurrentDirectory();
    public static string SrcRoot = Path.Combine(ProjectRoot, "src");
    public static string WebRoot = Path.Combine(SrcRoot, "InnHotel.Web");
    public static string UseCasesRoot = Path.Combine(SrcRoot, "InnHotel.UseCases");
    public static string CoreRoot = Path.Combine(SrcRoot, "InnHotel.Core");

    static Config()
    {
        if (!Directory.Exists(SrcRoot) || !Directory.Exists(WebRoot) || !Directory.Exists(UseCasesRoot))
        {
            throw new DirectoryNotFoundException(
                $"Project structure not found at {ProjectRoot}. Ensure you're running the script from the project root directory."
            );
        }
    }
}

// Template generator class
public class TemplateGenerator
{
    private readonly string _entityNameSingular;
    private readonly string _entityNamePlural;

    public TemplateGenerator(string entityNameSingular, string entityNamePlural)
    {
        _entityNameSingular = entityNameSingular;
        _entityNamePlural = entityNamePlural;
    }

    public string GetSpecificationContent()
    {
        return $@"using Ardalis.Specification;

namespace InnHotel.Core.{_entityNameSingular}Aggregate.Specifications;

public sealed class {_entityNameSingular}ByIdSpec : Specification<{_entityNameSingular}>
{{
    public {_entityNameSingular}ByIdSpec(int {_entityNameSingular.ToLower()}Id)
    {{
        Query
            .Where({_entityNameSingular.ToLower()} => {_entityNameSingular.ToLower()}.Id == {_entityNameSingular.ToLower()}Id)
            .Include({_entityNameSingular.ToLower()} => {_entityNameSingular.ToLower()}.Branch)
            .Include({_entityNameSingular.ToLower()} => {_entityNameSingular.ToLower()}.RoomType);
    }}
}}";
    }

    public string GetQueryContent()
    {
        return $@"namespace InnHotel.UseCases.{_entityNamePlural}.Get;

public record Get{_entityNameSingular}Query(int {_entityNameSingular}Id) : IQuery<Result<{_entityNameSingular}DTO>>;";
    }

    public string GetHandlerContent()
    {
        return $@"using InnHotel.Core.{_entityNameSingular}Aggregate;
using InnHotel.Core.{_entityNameSingular}Aggregate.Specifications;

namespace InnHotel.UseCases.{_entityNamePlural}.Get;

public class Get{_entityNameSingular}Handler(IReadRepository<{_entityNameSingular}> _repository)
    : IQueryHandler<Get{_entityNameSingular}Query, Result<{_entityNameSingular}DTO>>
{{
    public async Task<Result<{_entityNameSingular}DTO>> Handle(Get{_entityNameSingular}Query request, CancellationToken cancellationToken)
    {{
        var spec = new {_entityNameSingular}ByIdSpec(request.{_entityNameSingular}Id);
        var {_entityNameSingular.ToLower()} = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

        if ({_entityNameSingular.ToLower()} == null)
            return Result.NotFound();

        var {_entityNameSingular.ToLower()}Dto = new {_entityNameSingular}DTO(
            {_entityNameSingular.ToLower()}.Id,
            {_entityNameSingular.ToLower()}.BranchId,
            {_entityNameSingular.ToLower()}.Branch.Name,
            {_entityNameSingular.ToLower()}.RoomTypeId,
            {_entityNameSingular.ToLower()}.RoomType.Name,
            {_entityNameSingular.ToLower()}.RoomType.BasePrice,
            {_entityNameSingular.ToLower()}.RoomType.Capacity,
            {_entityNameSingular.ToLower()}.RoomNumber,
            {_entityNameSingular.ToLower()}.Status,
            {_entityNameSingular.ToLower()}.Floor);

        return {_entityNameSingular.ToLower()}Dto;
    }}
}}";
    }

    public string GetRequestContent()
    {
        return $@"namespace InnHotel.Web.{_entityNamePlural};

public class Get{_entityNameSingular}ByIdRequest
{{
    public const string Route = ""api/{_entityNamePlural}/{{id:int}}"";
    public static string BuildRoute(int id) => Route.Replace(""{{id:int}}"", id.ToString());

    public int {_entityNameSingular}Id {{ get; set; }}
}}";
    }

    public string GetEndpointContent()
    {
        return $@"using FastEndpoints;
using FastEndpoints.Security;
using InnHotel.UseCases.{_entityNamePlural}.Get;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Get a {_entityNameSingular} by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching {_entityNameSingular} record.
/// </remarks>
[Authorize]
public class GetById(IMediator _mediator)
    : Endpoint<Get{_entityNameSingular}ByIdRequest, object>
{{
    public override void Configure()
    {{
        Get(Get{_entityNameSingular}ByIdRequest.Route);
        Description(d => d
            .Produces<{_entityNameSingular}Record>(200, ""application/json"")
            .ProducesProblem(404)
            .ProducesProblem(500));
    }}

    public override async Task HandleAsync(Get{_entityNameSingular}ByIdRequest request,
        CancellationToken cancellationToken)
    {{
        var query = new Get{_entityNameSingular}Query(request.{_entityNameSingular}Id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {{
            var error = new FailureResponse(404, $""{_entityNameSingular} with ID {{request.{_entityNameSingular}Id}} not found"");
            await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
            return;
        }}

        if (result.IsSuccess)
        {{
            var {_entityNameSingular.ToLower()} = result.Value;
            Response = new {_entityNameSingular}Record(
                {_entityNameSingular.ToLower()}.Id,
                {_entityNameSingular.ToLower()}.BranchId,
                {_entityNameSingular.ToLower()}.BranchName,
                {_entityNameSingular.ToLower()}.RoomTypeId,
                {_entityNameSingular.ToLower()}.RoomTypeName,
                {_entityNameSingular.ToLower()}.BasePrice,
                {_entityNameSingular.ToLower()}.Capacity,
                {_entityNameSingular.ToLower()}.RoomNumber,
                {_entityNameSingular.ToLower()}.Status,
                {_entityNameSingular.ToLower()}.Floor);
            await SendOkAsync(Response, cancellationToken);
            return;
        }}

        await SendAsync(
            new FailureResponse(500, ""An unexpected error occurred.""), 
            statusCode: 500, 
            cancellation: cancellationToken);
    }}
}}";
    }

    public void CreateFiles()
    {
        // Create Core layer files (Specification)
        var coreEntityDir = Path.Combine(Config.CoreRoot, $"{_entityNameSingular}Aggregate", "Specifications");
        FileOperations.EnsureDirectory(coreEntityDir);

        FileOperations.CreateFile(
            Path.Combine(coreEntityDir, $"{_entityNameSingular}ByIdSpec.cs"),
            GetSpecificationContent()
        );

        // Create Use Cases layer files
        var useCasesEntityDir = Path.Combine(Config.UseCasesRoot, _entityNamePlural, "Get");
        FileOperations.EnsureDirectory(useCasesEntityDir);

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"Get{_entityNameSingular}Query.cs"),
            GetQueryContent()
        );

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"Get{_entityNameSingular}Handler.cs"),
            GetHandlerContent()
        );

        // Create Web layer files
        var webEntityDir = Path.Combine(Config.WebRoot, _entityNamePlural);
        FileOperations.EnsureDirectory(webEntityDir);

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"GetById.Get{_entityNameSingular}ByIdRequest.cs"),
            GetRequestContent()
        );

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, "GetById.cs"),
            GetEndpointContent()
        );

        WriteLine($"\nGet By ID operation for {_entityNameSingular} has been created successfully!");
        WriteLine("\n⚠️ IMPORTANT: The generated files require additional implementation:");
        WriteLine("1. Verify the specification includes all necessary relationships");
        WriteLine("2. Verify the DTO and Record mappings match your entity properties");
        WriteLine("3. Add any required authorization attributes to the endpoint");
        WriteLine("4. Add appropriate validation if needed");
        WriteLine("5. Consider adding caching if appropriate");
    }
}

// File system operations
public static class FileOperations
{
    public static void EnsureDirectory(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
            WriteLine($"Created directory: {fullPath}");
        }
    }

    public static void CreateFile(string path, string content)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, content.TrimStart());
            WriteLine($"Created file: {fullPath}");
        }
        else
        {
            WriteLine($"File already exists: {fullPath}");
        }
    }
}

// Command line parsing and execution
var entitySingularOption = new Option<string>(
    "--entity",
    "The singular name of the entity (e.g., Room, Guest, Branch)"
);

var entityPluralOption = new Option<string>(
    "--plural",
    "The plural name of the entity (e.g., Rooms, Guests, Branches)"
);

var rootCommand = new RootCommand("Creates Get By ID operation files for InnHotel API")
{
    entitySingularOption,
    entityPluralOption
};

rootCommand.SetHandler((string entitySingular, string entityPlural) =>
{
    try
    {
        WriteLine($"Project Root: {Config.ProjectRoot}");
        WriteLine($"Creating Get By ID operation for {entitySingular} (plural: {entityPlural})...\n");

        var generator = new TemplateGenerator(entitySingular, entityPlural);
        generator.CreateFiles();

    }
    catch (Exception ex)
    {
        WriteLine($"Error: {ex.Message}");
        Environment.Exit(1);
    }
}, entitySingularOption, entityPluralOption);

return await rootCommand.InvokeAsync(Args.ToArray());