namespace FutileGame.Services
{
    public interface ISquareValueFormatter
    {
        string FormatValue(int value);
    }

    public class DefaultSquareValueFormatter : ISquareValueFormatter
    {
        public string FormatValue(int value) => value switch
        {
            0 => "",
            5 => "1",
            _ => value.ToString()
        };
    }
}
