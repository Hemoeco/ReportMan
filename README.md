# reportman
ReportMan VS 2019
Esta es una versión compilada en VS 2019 a partir de la desacarga de "reportmannet_source_3_4_6.zip" de la página

https://sourceforge.net/p/reportman/activity/?page=0&limit=100#615ad5c647012700a256ad7c

incluye la actualización para .Net Framwork 4.6.1

Uso desde un Web API de NET Core 3.1

    using Microsoft.AspNetCore.Mvc;
    using System.IO;

    using Reportman.Drawing;
    using Reportman.Reporting;
    using System.Text;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        [HttpGet]
        [Route("ot/{idOT}")]
        public async Task<ActionResult> OT(string idOT)
        {
            Report rp = new Report();
            rp.LoadFromFile("C:\\Users\\cesar\\source\\repos\\SCORE\\HemoecoAPI\\ot-net.rep");
            rp.ConvertToDotNet();
            // FixReport
            rp.AsyncExecution = false;
            PrintOutPDF printpdf = new PrintOutPDF();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Perform the conversion from one encoding to the other.
            if (rp.Params.Count > 0)
            {
                // No funciona el Unicode
                string titulo = $"Orden de trabajo {idOT}"; ; // "你好"; // $"Orden de trabajo {idOT}";
                byte[] unicodeBytes = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, Encoding.ASCII.GetBytes(titulo));
                rp.Params[0].Value = Encoding.UTF8.GetString(unicodeBytes);
            }
            printpdf.FileName = $"ot-{idOT}.pdf";
            printpdf.Compressed = false;
            
            if (printpdf.Print(rp.MetaFile))
            {
                // Download Files using Web API. Changhui Xu. https://codeburst.io/download-files-using-web-api-ae1d1025f0a9
                var bytes = await System.IO.File.ReadAllBytesAsync(printpdf.FileName);
                return File(bytes, "application/pdf", printpdf.FileName);
            }

            // todo: Regresar archivo con falla indicada
            return null;
        }
      }
    }
        
