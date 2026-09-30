public class Security
{
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsRestricted { get; set; } = false;
    public double Drift { get; set; } = 0.05;
    public double Volatility { get; set; } = 0.20;
}
