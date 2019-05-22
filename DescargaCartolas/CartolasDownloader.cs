using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace DescargaCartolas
{
    public class CartolasDownloader : ICartolasDownloader, IDisposable
    {
        private readonly IWsSource soapClient;
        private readonly IFileController logController;
        private readonly IFileController pdfController;

        public CartolasDownloader(IWsSource soapClient, IFileController logController, IFileController pdfController)
        {
            this.soapClient = soapClient;
            this.logController = logController;
            this.pdfController = pdfController;
        }

        public CartolasDownloader(IWsSource soapClient)
        {
            this.soapClient = soapClient;
        }

        public dynamic ReadCartola(string desde, string hasta, string rut)
        { 
          
             WsDTO param = new WsDTO();
             param.ServiceName = "ServiceCartolas";
             param.MethodName = "ObtenerCartolaConsolidadaSecurityPrueba";
             param.Args = new object[] { desde, hasta, rut };
             dynamic result =  soapClient.CallWebService(param);
             return result;
            
        }


        public Task DownloadCartola(string desde, string hasta, string rut)
        {
           return Task.Run(()=> {
                try
                {
                    var cartola = ReadCartola(desde, hasta, rut);
                    pdfController.WriteBinaryFile(cartola);

                    Console.WriteLine($"{rut}_{desde}-{hasta}.pdf Downloaded!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {rut}_{desde}-{hasta}.pdf");
                    logController.WriteTextFile($"Error processing {rut}_{desde}-{hasta}.pdf " + Environment.NewLine);
                    logController.WriteTextFile(ex.ToString() + Environment.NewLine);

                }


            });
           

        }





        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CartolasDownloader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
