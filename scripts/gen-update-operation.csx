#!/usr/bin/env dotnet-script
/*
|--------------------------------------------------------------------------
| InnHotel API Update Operation Generator
|--------------------------------------------------------------------------
|
| Quick Start:
|   dotnet script ./scripts/gen-update-operation.csx -- --entity <singular> --plural <plural>
|
| Example:
|   dotnet script ./scripts/gen-update-operation.csx -- --entity Branch --plural Branches
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
        return $@"using InnHotel.UseCases.{_entityNamePlural}.Update;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Update an existing {_entityNameSingular}.
/// </summary>
/// <remarks>
/// Update an existing {_entityNameSingular} by providing updated details.
/// </remarks>
public class Update(IMediator _mediator)
    : Endpoint<Update{_entityNameSingular}Request, object>
{{
    public override void Configure()
    {{
        Put(Update{_entityNameSingular}Request.Route);
    }}

    public override async Task HandleAsync(
        Update{_entityNameSingular}Request request,
        CancellationToken cancellationToken)
    {{
        var command = new Update{_entityNameSingular}Command(
            request.{_entityNameSingular}Id
            // Map other properties from request to command
        );

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Status == ResultStatus.NotFound)
        {{
            await SendAsync(
                new FailureResponse(404, $""{_entityNameSingular} with ID {{request.{_entityNameSingular}Id}} not found""),
                statusCode: 404,
                cancellation: cancellationToken);
            return;
        }}

        if (result.Status == ResultStatus.Error)
        {{
            await SendAsync(
                new FailureResponse(400, result.Errors.First()),
                statusCode: 400,
                cancellation: cancellationToken);
            return;
        }}

        if (result.IsSuccess)
        {{
            var {_entityNameSingular.ToLower()}Record = new {_entityNameSingular}Record(
                result.Value.Id
                // Map other properties from DTO to record
            );

            await SendAsync(
                new {{ status = 200, message = ""{_entityNameSingular} updated successfully"", data = {_entityNameSingular.ToLower()}Record }},
                statusCode: 200,
                cancellation: cancellationToken);
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

public class Update{_entityNameSingular}Request
{{
    public const string Route = ""api/{_entityNamePlural}/{{{_entityNameSingular}Id:int}}"";
    public static string BuildRoute(int {_entityNameSingular.ToLower()}Id) => Route.Replace(""{{{_entityNameSingular}Id:int}}"", {_entityNameSingular.ToLower()}Id.ToString());

    public int {_entityNameSingular}Id {{ get; set; }}
    // Add your request properties here
    // Example:
    // public string Name {{ get; set; }} = string.Empty;
}}";
    }

    public string GetCommandContent()
    {
        return $@"namespace InnHotel.UseCases.{_entityNamePlural}.Update;

public record Update{_entityNameSingular}Command(
    int {_entityNameSingular}Id
    // Add command properties here
) : ICommand<Result<{_entityNameSingular}DTO>>;";
    }

    public string GetHandlerContent()
    {
        return $@"using InnHotel.Core.{_entityNameSingular}Aggregate;
using InnHotel.Core.{_entityNameSingular}Aggregate.Specifications;
using Microsoft.EntityFrameworkCore;

namespace InnHotel.UseCases.{_entityNamePlural}.Update;

public class Update{_entityNameSingular}Handler(IRepository<{_entityNameSingular}> _repository)
    : ICommandHandler<Update{_entityNameSingular}Command, Result<{_entityNameSingular}DTO>>
{{
    public async Task<Result<{_entityNameSingular}DTO>> Handle(Update{_entityNameSingular}Command request, CancellationToken cancellationToken)
    {{
        var spec = new {_entityNameSingular}ByIdSpec(request.{_entityNameSingular}Id);
        var {_entityNameSingular.ToLower()} = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if ({_entityNameSingular.ToLower()} == null)
            return Result.NotFound();

        // Add validation using specifications if needed
        // Example:
        // var uniqueFieldsSpec = new {_entityNameSingular}ByUniqueFieldsSpec(...);
        // var existing = await _repository.FirstOrDefaultAsync(uniqueFieldsSpec, cancellationToken);
        // if (existing != null && existing.Id != request.{_entityNameSingular}Id)
        //     return Result.Error(""A {_entityNameSingular.ToLower()} with these details already exists."");

        {_entityNameSingular.ToLower()}.UpdateDetails(
            // Map properties from request to entity update method
        );

        try
        {{
            await _repository.UpdateAsync({_entityNameSingular.ToLower()}, cancellationToken);
        }}
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains(""UX_"") == true)
        {{
            return Result.Error(""A {_entityNameSingular.ToLower()} with these details already exists."");
        }}

        return new {_entityNameSingular}DTO(
            {_entityNameSingular.ToLower()}.Id
            // Map other properties from entity to DTO
        );
    }}
}}";
    }

    public void CreateFiles()
    {
        // Create Web layer files
        var webEntityDir = Path.Combine(Config.WebRoot, _entityNamePlural);
        FileOperations.EnsureDirectory(webEntityDir);

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, "Update.cs"),
            GetEndpointContent()
        );

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"Update.Update{_entityNameSingular}Request.cs"),
            GetRequestContent()
        );

        // Create Use Cases layer files
        var useCasesEntityDir = Path.Combine(Config.UseCasesRoot, _entityNamePlural, "Update");
        FileOperations.EnsureDirectory(useCasesEntityDir);

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"Update{_entityNameSingular}Command.cs"),
            GetCommandContent()
        );

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"Update{_entityNameSingular}Handler.cs"),
            GetHandlerContent()
        );

        WriteLine($"\nUpdate operation for {_entityNameSingular} has been created successfully!");
        WriteLine("\n⚠️ IMPORTANT: The generated files require additional implementation:");
        WriteLine("1. Add appropriate properties to:");
        WriteLine($"   - Update{_entityNameSingular}Request");
        WriteLine($"   - Update{_entityNameSingular}Command");
        WriteLine("2. Implement property mapping in:");
        WriteLine("   - Update.cs endpoint");
        WriteLine($"   - Update{_entityNameSingular}Handler");
        WriteLine($"3. Add necessary specifications in {_entityNameSingular}Aggregate for validation");
        WriteLine("   Example: Uniqueness constraints, relationship validations, etc.");
        WriteLine($"4. Implement UpdateDetails() method in {_entityNameSingular} entity");
        WriteLine("   Example:");
        WriteLine($@"   public void UpdateDetails(
        // Add your parameters here
    ) {{
        // Implement property updates with validation
        // Use Guard.Against for validation
    }}");
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

var rootCommand = new RootCommand("Creates update operation files for InnHotel API")
{
    entitySingularOption,
    entityPluralOption
};

rootCommand.SetHandler((string entitySingular, string entityPlural) =>
{
    try
    {
        WriteLine($"Project Root: {Config.ProjectRoot}");
        WriteLine($"Creating Update operation for {entitySingular} (plural: {entityPlural})...\n");

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