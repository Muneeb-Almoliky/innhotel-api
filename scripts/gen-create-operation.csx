#!/usr/bin/env dotnet-script
/*
|--------------------------------------------------------------------------
| InnHotel API Create Operation Generator
|--------------------------------------------------------------------------
|
| Quick Start:
|   dotnet script ./scripts/gen-create-operation.csx -- --entity <singular> --plural <plural>
|
| Example:
|   dotnet script ./scripts/gen-create-operation.csx -- --entity Branch --plural Branches
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
        return $@"using InnHotel.UseCases.{_entityNamePlural}.Create;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Create a new {_entityNameSingular}.
/// </summary>
/// <remarks>
/// Creates a new {_entityNameSingular} with the provided details.
/// </remarks>
public class Create(IMediator _mediator)
    : Endpoint<Create{_entityNameSingular}Request, object>
{{
    public override void Configure()
    {{
        Post(Create{_entityNameSingular}Request.Route);
    }}

    public override async Task HandleAsync(
        Create{_entityNameSingular}Request request,
        CancellationToken cancellationToken)
    {{
        var command = new Create{_entityNameSingular}Command(
            // Map properties from request to command
        );

        var result = await _mediator.Send(command, cancellationToken);

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
                new {{ status = 201, message = ""{_entityNameSingular} created successfully"", data = {_entityNameSingular.ToLower()}Record }},
                statusCode: 201,
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

    public string GetValidatorContent()
    {
        return $@"using FastEndpoints;
using FluentValidation;

namespace InnHotel.Web.{_entityNamePlural};

public class Create{_entityNameSingular}Validator : Validator<Create{_entityNameSingular}Request>
{{
    public Create{_entityNameSingular}Validator()
    {{
        // Add validation rules for your properties
        // Example:
        // RuleFor(x => x.Name)
        //     .NotEmpty().WithMessage(""{_entityNameSingular} name is required"")
        //     .MinimumLength(2).WithMessage(""{_entityNameSingular} name must be at least 2 characters"")
        //     .MaximumLength(100).WithMessage(""{_entityNameSingular} name must not exceed 100 characters"");
    }}
}}";
    }

    public string GetRequestContent()
    {
        return $@"namespace InnHotel.Web.{_entityNamePlural};

public class Create{_entityNameSingular}Request
{{
    public const string Route = ""api/{_entityNamePlural}"";

    // Add your request properties here
    // Example:
    // public string Name {{ get; set; }} = string.Empty;
}}";
    }

    public string GetDTOContent()
    {
        return $@"namespace InnHotel.UseCases.{_entityNamePlural};

public record {_entityNameSingular}DTO(
    int Id
    // Add other DTO properties here
);";
    }

    public string GetRecordContent()
    {
        return $@"namespace InnHotel.Web.{_entityNamePlural};

public record {_entityNameSingular}Record(
    int Id
    // Add other record properties here
);";
    }

    public string GetCommandContent()
    {
        return $@"namespace InnHotel.UseCases.{_entityNamePlural}.Create;

public record Create{_entityNameSingular}Command(
    // Add command properties here
) : ICommand<Result<{_entityNameSingular}DTO>>;";
    }

    public string GetHandlerContent()
    {
        return $@"using InnHotel.Core.{_entityNameSingular}Aggregate;
using InnHotel.Core.{_entityNameSingular}Aggregate.Specifications;

namespace InnHotel.UseCases.{_entityNamePlural}.Create;

public class Create{_entityNameSingular}Handler(IRepository<{_entityNameSingular}> _repository)
    : ICommandHandler<Create{_entityNameSingular}Command, Result<{_entityNameSingular}DTO>>
{{
    public async Task<Result<{_entityNameSingular}DTO>> Handle(Create{_entityNameSingular}Command request, CancellationToken cancellationToken)
    {{
        // Add validation using specifications
        // Example:
        // var spec = new {_entityNameSingular}ByUniqueFieldsSpec(...);
        // var existing = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        // if (existing != null)
        //     return Result.Error(""A {_entityNameSingular.ToLower()} with these details already exists."");

        var {_entityNameSingular.ToLower()} = new {_entityNameSingular}(
            // Map properties from request to entity constructor
        );

        await _repository.AddAsync({_entityNameSingular.ToLower()}, cancellationToken);

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
            Path.Combine(webEntityDir, "Create.cs"),
            GetEndpointContent()
        );

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"Create.Create{_entityNameSingular}Validator.cs"),
            GetValidatorContent()
        );

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"Create.Create{_entityNameSingular}Request.cs"),
            GetRequestContent()
        );

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"{_entityNameSingular}Record.cs"),
            GetRecordContent()
        );

        // Create Use Cases layer files
        var useCasesEntityDir = Path.Combine(Config.UseCasesRoot, _entityNamePlural);
        FileOperations.EnsureDirectory(useCasesEntityDir);

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"{_entityNameSingular}DTO.cs"),
            GetDTOContent()
        );

        var createDir = Path.Combine(useCasesEntityDir, "Create");
        FileOperations.EnsureDirectory(createDir);

        FileOperations.CreateFile(
            Path.Combine(createDir, $"Create{_entityNameSingular}Command.cs"),
            GetCommandContent()
        );

        FileOperations.CreateFile(
            Path.Combine(createDir, $"Create{_entityNameSingular}Handler.cs"),
            GetHandlerContent()
        );

        WriteLine($"\nCreate operation for {_entityNameSingular} has been created successfully!");
        WriteLine("\n⚠️ IMPORTANT: The generated files require additional implementation:");
        WriteLine("1. Add appropriate properties to:");
        WriteLine($"   - Create{_entityNameSingular}Request");
        WriteLine($"   - {_entityNameSingular}DTO");
        WriteLine($"   - {_entityNameSingular}Record");
        WriteLine($"   - Create{_entityNameSingular}Command");
        WriteLine("2. Add validation rules in:");
        WriteLine($"   - Create{_entityNameSingular}Validator");
        WriteLine("3. Implement property mapping in:");
        WriteLine("   - Create.cs endpoint");
        WriteLine($"   - Create{_entityNameSingular}Handler");
        WriteLine($"4. Add necessary specifications in {_entityNameSingular}Aggregate for validation");
        WriteLine("   Example: Uniqueness constraints, relationship validations, etc.");
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

var rootCommand = new RootCommand("Creates create operation files for InnHotel API")
{
    entitySingularOption,
    entityPluralOption
};

rootCommand.SetHandler((string entitySingular, string entityPlural) =>
{
    try
    {
        WriteLine($"Project Root: {Config.ProjectRoot}");
        WriteLine($"Creating Create operation for {entitySingular} (plural: {entityPlural})...\n");

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