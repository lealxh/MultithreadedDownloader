using System.Threading.Tasks;

namespace DescargaCartolas
{
    public interface ICartolasDownloader
    {
        Task DownloadCartola(string desde, string hasta, string rutcliente);
    }
}