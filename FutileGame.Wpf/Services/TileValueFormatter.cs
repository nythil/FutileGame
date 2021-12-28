namespace FutileGame.Services
{
    public interface ITileValueFormatter
    {
        string FormatValue(int value);
    }

    public class DefaultTileValueFormatter : ITileValueFormatter
    {
        public string FormatValue(int value) => value switch
        {
            0 => "",
            5 => "1",
            _ => value.ToString()
        };
    }
}
