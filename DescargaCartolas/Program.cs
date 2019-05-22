using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace DescargaCartolas
{
    class Program
    {

        private string ruta;
        private string[] ruts;
        private string[] intervalos;
        private string endpoint;
        public Program()
        {
            this.ruta = ConfigurationSettings.AppSettings.Get("ruta");
            this.ruts = ConfigurationSettings.AppSettings.Get("ruts").Split(',');
            this.intervalos = ConfigurationSettings.AppSettings.Get("fechas").Split(',');
            this.endpoint= ConfigurationSettings.AppSettings.Get("endpointCartolas");

        }
        public void Procesar()
        {
                IFileController logController = new FileController($"{ruta}Log.txt");
                IWsSource ws = new WsSource(endpoint);

                Stopwatch stopWatch = new Stopwatch();
                Console.WriteLine($"Starting download process for {this.ruts.Count()*this.intervalos.Count()} cartolas");
                stopWatch.Start();
                List<Task> threads = new List<Task>();
                foreach (var rut in this.ruts)
                {
                    foreach (var intervalo in this.intervalos)
                    {
                        String[] fecha = intervalo.Split('-');
                        IFileController pdfController = new FileController($"{ruta}{rut}_{intervalo}.pdf");
                        CartolasDownloader d = new CartolasDownloader(ws, logController, pdfController);
                        threads.Add(d.DownloadCartola(fecha[0], fecha[1], rut));

                    }
                }
              

                Task.WhenAll(threads).
                ContinueWith((anterior)=> {
                    
                    stopWatch.Stop();
                    Console.WriteLine($"Elapsed time: {stopWatch.Elapsed}");
                });
          
        }

       
        

        static void Main(string[] args)
        {
            try
            {
                Program p = new Program();
                p.Procesar();
                Console.ReadKey();
            }
            catch (Exception ex)
            {

                throw;
            }
    
           

        }





    }
}
