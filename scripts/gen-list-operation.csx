#!/usr/bin/env dotnet-script
/*
|--------------------------------------------------------------------------
| InnHotel API List Operation Generator
|--------------------------------------------------------------------------
|
| Quick Start:
|   dotnet script ./scripts/gen-list-operation.csx -- --entity <singular> --plural <plural>
|
| Example:
|   dotnet script ./scripts/gen-list-operation.csx -- --entity Branch --plural Branches
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

    public string GetEndpointContent()
    {
        return $@"using InnHotel.UseCases.{_entityNamePlural}.List;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// List all {_entityNamePlural} with pagination support.
/// </summary>
/// <remarks>
/// Returns a paginated list of {_entityNameSingular} records.
/// </remarks>
public class List(IMediator _mediator)
    : Endpoint<PaginationRequest, object>
{{
    public override void Configure()
    {{
        Get(List{_entityNameSingular}Request.Route);
    }}

    public override async Task HandleAsync(PaginationRequest request, CancellationToken cancellationToken)
    {{
        var query = new List{_entityNameSingular}Query(request.PageNumber, request.PageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {{
            var (items, totalCount) = result.Value;
            var {_entityNameSingular.ToLower()}Records = items.Select(e => 
                new {_entityNameSingular}Record(
                    e.Id
                    // Add other properties here based on your Record type
                )).ToList();

            var response = new PagedResponse<{_entityNameSingular}Record>(
                {_entityNameSingular.ToLower()}Records, 
                totalCount, 
                request.PageNumber, 
                request.PageSize);

            await SendOkAsync(response, cancellationToken);
            return;
        }}

        await SendAsync(
            new FailureResponse(500, ""An unexpected error occurred.""), 
            statusCode: 500, 
            cancellation: cancellationToken);
    }}
}}";
    }

    public string GetRequestContent()
    {
        return $@"namespace InnHotel.Web.{_entityNamePlural};

public class List{_entityNameSingular}Request
{{
    public const string Route = ""api/{_entityNamePlural}"";
    public static string BuildRoute() => Route;
}}";
    }

    public string GetQueryContent()
    {
        return $@"namespace InnHotel.UseCases.{_entityNamePlural}.List;

public record List{_entityNameSingular}Query(int PageNumber, int PageSize) 
    : IQuery<Result<(List<{_entityNameSingular}DTO> Items, int TotalCount)>>;";
    }

    public string GetHandlerContent()
    {
        return $@"using InnHotel.Core.{_entityNameSingular}Aggregate;

namespace InnHotel.UseCases.{_entityNamePlural}.List;

public class List{_entityNameSingular}Handler(IReadRepository<{_entityNameSingular}> _repository)
    : IQueryHandler<List{_entityNameSingular}Query, Result<(List<{_entityNameSingular}DTO> Items, int TotalCount)>>
{{
    public async Task<Result<(List<{_entityNameSingular}DTO> Items, int TotalCount)>> Handle(List{_entityNameSingular}Query request, CancellationToken cancellationToken)
    {{
        var totalCount = await _repository.CountAsync(cancellationToken);
        
        var {_entityNamePlural.ToLower()} = await _repository.ListAsync(cancellationToken);
        var paged{_entityNamePlural} = {_entityNamePlural.ToLower()}
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var {_entityNameSingular.ToLower()}Dtos = paged{_entityNamePlural}.Select(entity => new {_entityNameSingular}DTO(
            entity.Id
            // Add other properties here based on your DTO
            )).ToList();

        return ({_entityNameSingular.ToLower()}Dtos, totalCount);
    }}
}}";
    }

    public void CreateFiles()
    {
        // Create Web layer files
        var webEntityDir = Path.Combine(Config.WebRoot, _entityNamePlural);
        FileOperations.EnsureDirectory(webEntityDir);

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, "List.cs"),
            GetEndpointContent()
        );

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"List.List{_entityNameSingular}Request.cs"),
            GetRequestContent()
        );

        // Create Use Cases layer files
        var useCasesEntityDir = Path.Combine(Config.UseCasesRoot, _entityNamePlural, "List");
        FileOperations.EnsureDirectory(useCasesEntityDir);

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"List{_entityNameSingular}Query.cs"),
            GetQueryContent()
        );

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"List{_entityNameSingular}Handler.cs"),
            GetHandlerContent()
        );

        WriteLine($"\nList operation for {_entityNameSingular} has been created successfully!");
        WriteLine("\n⚠️ IMPORTANT: The generated files require additional implementation:");
        WriteLine("1. Add appropriate properties to:");
        WriteLine($"   - {_entityNameSingular}DTO mapping in the Handler");
        WriteLine($"   - {_entityNameSingular}Record mapping in the Endpoint");
        WriteLine("2. Add any required specifications if needed for filtering");
        WriteLine("3. Ensure you have the necessary Record type defined");
        WriteLine("4. Add any required authorization attributes to the endpoint");
        WriteLine("5. Consider adding sorting and filtering capabilities if needed");
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

var rootCommand = new RootCommand("Creates list operation files for InnHotel API")
{
    entitySingularOption,
    entityPluralOption
};

rootCommand.SetHandler((string entitySingular, string entityPlural) =>
{
    try
    {
        WriteLine($"Project Root: {Config.ProjectRoot}");
        WriteLine($"Creating List operation for {entitySingular} (plural: {entityPlural})...\n");

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