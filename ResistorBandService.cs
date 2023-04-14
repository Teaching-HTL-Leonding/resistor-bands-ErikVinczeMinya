/*public class ResistorBandService
{
    ResistorBand[] resistorBands = new ResistorBand[10];
    SpezialResistorBand[] spezialResistorBands = new SpezialResistorBand[2];

    public ResistorBandService()
    {
        this.resistorBands[0] = new ResistorBand("Black", 0, 1, 0);
        this.resistorBands[1] = new ResistorBand("Brown", 1, 10, 0.01);
        this.resistorBands[2] = new ResistorBand("Red", 2, 100, 0.02);
        this.resistorBands[3] = new ResistorBand("Orange", 3, 1_000, 0);
        this.resistorBands[4] = new ResistorBand("Yellow", 4, 10_000, 0);
        this.resistorBands[5] = new ResistorBand("Green", 5, 100_000, 0.005);
        this.resistorBands[6] = new ResistorBand("Blue", 6, 1_000_000, 0.0025);
        this.resistorBands[7] = new ResistorBand("Violet", 7, 10_000_000, 0.001);
        this.resistorBands[8] = new ResistorBand("Grey", 8, 100_000_000, 0.0005);
        this.resistorBands[9] = new ResistorBand("White", 9, 1_000_000_000, 0);
        this.spezialResistorBands[0] = new SpezialResistorBand("Gold", 0.1, 0.05);
        this.spezialResistorBands[1] = new SpezialResistorBand("Silver", 0.01, 0.1);
    }

    public String[] getColors(){
        String[] colors = new String[this.resistorBands.Length];
        for(int i = 0; i < this.resistorBands.Length; i++){
            colors[i] = this.resistorBands[i].Color;
        }
        for(int i = 0; i < this.spezialResistorBands.Length; i++){
            colors[i] = this.spezialResistorBands[i].Color;
        }
        return colors;
    }

    public ResistorBand getResistorBand(String color){
        for(int i = 0; i < this.resistorBands.Length; i++){
            if(this.resistorBands[i].Color.ToLower() == color.ToLower()){
                return this.resistorBands[i];
            }
        }
        for(int i = 0; i < this.spezialResistorBands.Length; i++){
            if(this.spezialResistorBands[i].Color.ToLower() == color.ToLower()){
                return new ResistorBand(this.spezialResistorBands[i].Color, 0, this.spezialResistorBands[i].Multiplier, this.spezialResistorBands[i].Tolerance);
            }
        }
        return null;
    }

    public ReturnCalculatedResistor calculateResistorFourBand(String[] colors){
        double value = 0;
        double tolerance = 0;
        value += this.getResistorBand(colors[0]).Value * 10;
        value += this.getResistorBand(colors[1]).Value;
        value *= this.getResistorBand(colors[2]).Multiplier;
        tolerance = this.getResistorBand(colors[3]).Tolerance;
        return new ReturnCalculatedResistor(value, tolerance);
    }


    public ReturnCalculatedResistor calculateResistorFiveBand(String[] colors){
        double value = 0;
        double tolerance = 0;
        value += this.getResistorBand(colors[0]).Value * 100;
        value += this.getResistorBand(colors[1]).Value * 10;
        value += this.getResistorBand(colors[2]).Value;
        value *= this.getResistorBand(colors[3]).Multiplier;
        tolerance = this.getResistorBand(colors[4]).Tolerance;
        return new ReturnCalculatedResistor(value, tolerance);
    }
}*/

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

public class ResistorBandService{
    ConcurrentDictionary<string, ResistorBand> resistorBands = new(){
        ["black"] = new(0,0,0),
        ["brown"] = new(1,1,0.01),
        ["red"] = new(2,2,0.02),
        ["orange"] = new(3,3,0),
        ["yellow"] = new(4,4,0),
        ["green"] = new(5,5,0.005),
        ["blue"] = new(6,6,0.0025),
        ["violet"] = new(7,7,0.001),
        ["grey"] = new(8,8,0.0005),
        ["white"] = new(9,9,0),
        ["gold"] = new(0,-1,0.05),
        ["silver"] = new(0,-2,0.1)
    };

    public ICollection<string> getColors() => resistorBands.Keys;

    public ResistorBand getResistorBand(string color) => resistorBands[color.ToLower()];

    public ReturnCalculatedResistor calculateResistorFourBand(string[] colors){
        var value = 0;
        var tolerance = 0;
        value += this.getResistorBand(colors[0]).Value * 10;
        value += this.getResistorBand(colors[1]).Value;
        value *= (int)Math.Pow(10, this.getResistorBand(colors[2]).Multiplier);
        tolerance = (int)this.getResistorBand(colors[3]).Tolerance;
        return new ReturnCalculatedResistor(value, tolerance);
    }

    public ReturnCalculatedResistor calculateResistorFiveBand(string[] colors){
        var value = 0;
        var tolerance = 0;
        value += this.getResistorBand(colors[0]).Value * 100;
        value += this.getResistorBand(colors[1]).Value * 10;
        value += this.getResistorBand(colors[2]).Value;
        value *= (int)Math.Pow(10, this.getResistorBand(colors[3]).Multiplier);
        tolerance = (int)this.getResistorBand(colors[4]).Tolerance;
        return new ReturnCalculatedResistor(value, tolerance);
    }
}

public record ResistorBand(int Value, int Multiplier, double Tolerance){
    /// <summary>
    /// Meaning of the value bands
    /// </summary>
    [Required]
    public int Value { get; init; } = Value;
    /// <summary>
    /// Meaning of the multiplier band
    /// </summary>
    [Required]
    public int Multiplier { get; init; } = Multiplier;
    /// <summary>
    /// Meaning of the tolerance band. Note that this band does not exist for some colors.
    /// </summary>
    public double Tolerance { get; init; } = Tolerance;
}

public record ReturnCalculatedResistor(double Value, double Tolerance)
{
    /// <summary>
    /// Resistor value in Ohm
    /// </summary>
    [Required]
    public double Value { get; init; } = Value;
    /// <summary>
    /// Tolerance in percentage
    /// </summary>
    [Required]
    public double Tolerance { get; init; } = Tolerance;
}
