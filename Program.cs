using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Resistor Color Coding", Version = "v1" });

    var locationOfExecutable = Assembly.GetExecutingAssembly().Location;
    var execFileNameWithoutExtension = Path.GetFileNameWithoutExtension(locationOfExecutable);
    var execFilePath = Path.GetDirectoryName(locationOfExecutable);
    var xmlFilePath = Path.Combine(execFilePath!, $"{execFileNameWithoutExtension}.xml");

    options.IncludeXmlComments(xmlFilePath);
});

var resistorBands = new ResistorBandService();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/colors", () => resistorBands.getColors())
    .WithTags("Color")
    .WithOpenApi(o =>
    {
        o.OperationId = "GetColors";
        o.Summary = "Return all colors for bands on resistors";
        o.Responses[((int)StatusCodes.Status200OK).ToString()].Description = "A list of all colors";
        return o;
    });

app.MapGet("/colors/{color}", (string color) =>
    {
        if(resistorBands.getResistorBand(color) != null){
            return Results.Ok(resistorBands.getResistorBand(color));
        }
        return Results.NotFound();
    })
    .WithTags("Color")
    .Produces<ResistorBand>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithOpenApi(o =>
    {
        o.OperationId = "GetColorByID";
        o.Summary = "Return details for a color band";
        o.Responses[((int)StatusCodes.Status200OK).ToString()].Description = "Details for a color band";
        o.Responses[((int)StatusCodes.Status404NotFound).ToString()].Description = "Unknown color";
        return o;
    });

app.MapPost("/resistors/value-from-bands", (ResistorBands colors) =>
{
    if (colors.FirstBand == null)
    {
        return resistorBands.calculateResistorFourBand(new string[] { colors.FirstBand, colors.SecondBand, colors.Multiplier, colors.Tolerance });
    }
    else if (colors.FirstBand != null)
    {
        return resistorBands.calculateResistorFiveBand(new string[] { colors.FirstBand, colors.SecondBand, colors.ThirdBand, colors.Multiplier, colors.Tolerance });
    }
    return null;
})
    .WithTags("Resistors")
    .Produces<ReturnCalculatedResistor>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .WithOpenApi(o =>
    {
        o.OperationId = "GetColorByID";
        o.Summary = "Calculates the resistor value based on given color bands (using POST).";
        o.Responses[((int)StatusCodes.Status200OK).ToString()].Description = "Resistor value could be decoded correctly";
        o.Responses[((int)StatusCodes.Status400BadRequest).ToString()].Description = "The request body contains invalid data";
        return o;
    });

app.MapGet("/resistors/value-from-bands", (string firstBand, string secondBand, string? thirdBand, string multiplier, string tolerance) =>
{
    if (thirdBand == null)
    {
        return resistorBands.calculateResistorFourBand(new string[] { firstBand, secondBand, multiplier, tolerance });
    }
    else if (thirdBand != null)
    {
        return resistorBands.calculateResistorFiveBand(new string[] { firstBand, secondBand, thirdBand, multiplier, tolerance });
    }
    return null;
})
    .WithTags("Resistors")
    .Produces<ReturnCalculatedResistor>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .WithOpenApi(o =>
    {
        o.OperationId = "GetColorByID";
        o.Summary = "Calculates the resistor value based on given color bands (using GET).";
        o.Parameters[0].Description = "Color of the 1st band";
        o.Parameters[1].Description = "Color of the 2nd band";
        o.Parameters[2].Description = "Color of the 3rd band. Note that this band can be left out for 4-band-coded resistors";
        o.Parameters[3].Description = "Color of the multiplier band";
        o.Parameters[4].Description = "Color of the tolerance band";

        o.Responses[((int)StatusCodes.Status200OK).ToString()].Description = "Resistor value could be decoded correctly";
        o.Responses[((int)StatusCodes.Status400BadRequest).ToString()].Description = "The request body contains invalid data";
        return o;
    });

app.Run();

record ResistorBands(string FirstBand, string SecondBand, string? ThirdBand, string Multiplier, string Tolerance)
{
    /// <summary>
    /// Color of the 1st band
    /// </summary>
    [Required]
    public string FirstBand { get; init; } = FirstBand;
    /// <summary>
    /// Color of the 2nd band
    /// </summary>
    [Required]
    public string SecondBand { get; init; } = SecondBand;
    /// <summary>
    /// Color of the 3rd band. Note that this band can be left out for 4-band-coded resistors
    /// </summary>
    public string? ThirdBand { get; init; } = ThirdBand;
    /// <summary>
    /// Color of the multiplier band
    /// </summary>
    [Required]
    public string Multiplier { get; init; } = Multiplier;
    /// <summary>
    /// Color of the tolerance band
    /// </summary>
    [Required]
    public string Tolerance { get; init; } = Tolerance;
}