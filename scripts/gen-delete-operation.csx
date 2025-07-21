#!/usr/bin/env dotnet-script
/*
|--------------------------------------------------------------------------
| InnHotel API Delete Operation Generator
|--------------------------------------------------------------------------
|
| Quick Start:
|   dotnet script ./scripts/gen-delete-operation.csx -- --entity <singular> --plural <plural>
|
| Example:
|   dotnet script ./scripts/gen-delete-operation.csx -- --entity Branch --plural Branches
|
| ⚠️ Important:
|   Manual DI registration is required in:
|   src/InnHotel.Infrastructure/InfrastructureServiceExtensions.cs
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
    public static string InfrastructureRoot = Path.Combine(SrcRoot, "InnHotel.Infrastructure");

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
        return $@"using InnHotel.UseCases.{_entityNamePlural}.Delete;
using InnHotel.Web.Common;

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// Delete a {_entityNameSingular}.
/// </summary>
/// <remarks>
/// Delete a {_entityNameSingular} by providing a valid integer id.
/// </remarks>
public class Delete(IMediator _mediator)
  : Endpoint<Delete{_entityNameSingular}Request>
{{
  public override void Configure()
  {{
    Delete(Delete{_entityNameSingular}Request.Route);
  }}

  public override async Task HandleAsync(
    Delete{_entityNameSingular}Request request,
    CancellationToken cancellationToken)
  {{
    var command = new Delete{_entityNameSingular}Command(request.{_entityNameSingular}Id);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {{
      var error = new FailureResponse(404, $""{_entityNameSingular} with ID {{request.{_entityNameSingular}Id}} not found"");
      await SendAsync(error, statusCode: 404, cancellation: cancellationToken);
      return;
    }}

    if (result.IsSuccess)
    {{
      await SendAsync(new {{ status = 200, message = $""{_entityNameSingular} with ID {{request.{_entityNameSingular}Id}} was successfully deleted"" }}, 
        statusCode: 200, 
        cancellation: cancellationToken);
      return;
    }}

    await SendAsync(new FailureResponse(500, ""An unexpected error occurred.""), statusCode: 500, cancellation: cancellationToken);
  }}
}}";
    }

    public string GetCommandContent()
    {
        return $@"namespace InnHotel.UseCases.{_entityNamePlural}.Delete;

public record Delete{_entityNameSingular}Command(int {_entityNameSingular}Id) : ICommand<Result>;";
    }

    public string GetHandlerContent()
    {
        return $@"namespace InnHotel.UseCases.{_entityNamePlural}.Delete;
using InnHotel.Core.Interfaces;

public class Delete{_entityNameSingular}Handler(IDelete{_entityNameSingular}Service _delete{_entityNameSingular}Service)
  : ICommandHandler<Delete{_entityNameSingular}Command, Result>
{{
  public async Task<Result> Handle(Delete{_entityNameSingular}Command request, CancellationToken cancellationToken) =>
    await _delete{_entityNameSingular}Service.Delete{_entityNameSingular}(request.{_entityNameSingular}Id);
}}";
    }

    public string GetRequestContent()
    {
        return $@"namespace InnHotel.Web.{_entityNamePlural};

public record Delete{_entityNameSingular}Request
{{
  public const string Route = ""api/{_entityNamePlural}/{{{_entityNameSingular}Id:int}}"";
  public static string BuildRoute(int {_entityNameSingular.ToLower()}Id) => Route.Replace(""{{{_entityNameSingular}Id:int}}"", {_entityNameSingular.ToLower()}Id.ToString());

  public int {_entityNameSingular}Id {{ get; set; }}
}}";
    }

    public string GetValidatorContent()
    {
        return $@"using FastEndpoints;
using FluentValidation;
using global::InnHotel.Web.{_entityNamePlural};

namespace InnHotel.Web.{_entityNamePlural};

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class Delete{_entityNameSingular}Validator : Validator<Delete{_entityNameSingular}Request>
{{
  public Delete{_entityNameSingular}Validator()
  {{
    RuleFor(x => x.{_entityNameSingular}Id)
      .GreaterThan(0);
  }}
}}";
    }

    public string GetDeleteServiceInterfaceContent()
    {
        return $@"namespace InnHotel.Core.Interfaces;

public interface IDelete{_entityNameSingular}Service
{{
  // This service and method exist to provide a place in which to fire domain events
  // when deleting this aggregate root entity
  public Task<Result> Delete{_entityNameSingular}(int {_entityNameSingular.ToLower()}Id);
}}";
    }

    public string GetDeletedEventContent()
    {
        return $@"namespace InnHotel.Core.{_entityNameSingular}Aggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a {_entityNameSingular.ToLower()} is deleted.
/// The Delete{_entityNameSingular}Service is used to dispatch this event.
/// </summary>
internal sealed class {_entityNameSingular}DeletedEvent(int {_entityNameSingular.ToLower()}Id) : DomainEventBase
{{
  public int {_entityNameSingular.ToLower()}Id {{ get; init; }} = {_entityNameSingular.ToLower()}Id;
}}";
    }

    public string GetDeleteServiceContent()
    {
        return $@"namespace InnHotel.Core.Services;
using InnHotel.Core.{_entityNameSingular}Aggregate.Events;
using InnHotel.Core.{_entityNameSingular}Aggregate;
using InnHotel.Core.Interfaces;

/// <summary>
/// This is here mainly so there's an example of a domain service
/// and also to demonstrate how to fire domain events from a service.
/// </summary>
/// <param name=""_repository""></param>
/// <param name=""_mediator""></param>
/// <param name=""_logger""></param>
public class Delete{_entityNameSingular}Service(IRepository<{_entityNameSingular}> _repository,
  IMediator _mediator,
  ILogger<Delete{_entityNameSingular}Service> _logger) : IDelete{_entityNameSingular}Service
{{
  public async Task<Result> Delete{_entityNameSingular}(int {_entityNameSingular.ToLower()}Id)
  {{
    _logger.LogInformation(""Deleting {_entityNameSingular} {{{_entityNameSingular.ToLower()}Id}}"", {_entityNameSingular.ToLower()}Id);
    {_entityNameSingular}? aggregateToDelete = await _repository.GetByIdAsync({_entityNameSingular.ToLower()}Id);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete);
    var domainEvent = new {_entityNameSingular}DeletedEvent({_entityNameSingular.ToLower()}Id);
    await _mediator.Publish(domainEvent);

    return Result.Success();
  }}
}}";
    }

    public void CreateFiles()
    {
        // Create Web layer files
        var webEntityDir = Path.Combine(Config.WebRoot, _entityNamePlural);
        FileOperations.EnsureDirectory(webEntityDir);

        // Create Use Cases layer files
        var useCasesEntityDir = Path.Combine(Config.UseCasesRoot, _entityNamePlural, "Delete");
        FileOperations.EnsureDirectory(useCasesEntityDir);

        // Create Core layer files
        var servicesDir = Path.Combine(Config.CoreRoot, "Services");
        FileOperations.EnsureDirectory(servicesDir);
        FileOperations.CreateFile(
            Path.Combine(servicesDir, $"Delete{_entityNameSingular}Service.cs"),
            GetDeleteServiceContent()
        );

        var eventsDir = Path.Combine(Config.CoreRoot, $"{_entityNameSingular}Aggregate", "Events");
        FileOperations.EnsureDirectory(eventsDir);
        FileOperations.CreateFile(
            Path.Combine(eventsDir, $"{_entityNameSingular}DeletedEvent.cs"),
            GetDeletedEventContent()
        );

        // Generate and write files
        FileOperations.CreateFile(
            Path.Combine(webEntityDir, "Delete.cs"),
            GetEndpointContent()
        );

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"Delete{_entityNameSingular}Command.cs"),
            GetCommandContent()
        );

        FileOperations.CreateFile(
            Path.Combine(useCasesEntityDir, $"Delete{_entityNameSingular}Handler.cs"),
            GetHandlerContent()
        );

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"Delete.Delete{_entityNameSingular}Request.cs"),
            GetRequestContent()
        );

        FileOperations.CreateFile(
            Path.Combine(webEntityDir, $"Delete.Delete{_entityNameSingular}Validator.cs"),
            GetValidatorContent()
        );

        FileOperations.CreateFile(
            Path.Combine(Config.CoreRoot, "Interfaces", $"IDelete{_entityNameSingular}Service.cs"),
            GetDeleteServiceInterfaceContent()
        );

        WriteLine($"\nDelete operation for {_entityNameSingular} has been created successfully!");
        WriteLine("Remember to:");
        WriteLine("1. Register the delete service in the Infrastructure layer");
        WriteLine("2. Add any necessary domain event handlers");
        WriteLine("3. Update repository interfaces if required");
        WriteLine("4. Add any additional validation if needed");
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

var rootCommand = new RootCommand("Creates delete operation files for InnHotel API")
{
    entitySingularOption,
    entityPluralOption
};

rootCommand.SetHandler((string entitySingular, string entityPlural) =>
{
    try
    {
        WriteLine($"Project Root: {Config.ProjectRoot}");
        WriteLine($"Creating Delete operation for {entitySingular} (plural: {entityPlural})...\n");

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