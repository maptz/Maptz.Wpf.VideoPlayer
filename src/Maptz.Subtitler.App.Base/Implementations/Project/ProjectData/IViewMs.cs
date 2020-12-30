namespace Maptz.Subtitler.App
{

  public interface IViewMs
    {
        long EndMs { get; set; }
        long MaxEndMs { get; set; }
        long MaxStartMs { get; set; }
        long StartMs { get; set; }
    }
}