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
    ...
        private async Task<ActionResult> GenerarPdf(string pdfDestinoSinExt, string repOrigen, ConfigurarParametros configParams)
        {
            // SCOREEntitiesHelper.WriteToLog($"Llamada a GenerarPdf({pdfDestino}, {repOrigen}, ...)");
            // return new JsonResult($"log: {SCOREEntitiesHelper.LogPath}");

            try
            {
                // SCOREEntitiesHelper.WriteToLog($"new ReporteHemoeco", false);

                ReporteHemoeco rp = new ReporteHemoeco(repOrigen, ref pdfDestinoSinExt, _config);

                // Configurar los parámetros requeridos (delegate)
                if (configParams != null)
                {
                    configParams(rp);
                }

                // SCOREEntitiesHelper.WriteToLog($"Imprimir pdf", false);
                if (rp.ImprimirPdf())
                {
                    return await EnviarArchivoRespuesta(pdfDestinoSinExt);
                }
            }
            catch (Exception ex)
            {
                // todo: Regresar archivo con falla indicada
                return new JsonResult(ex.Message);
            }

            // Si 'rp.ImprimirPdf()' regresa false, sin excepción, puede ser que el reporte esté configurado para imprimir sólo cuando hay datos.
            // Esto se puede revisar en report designer, en la configuración del subreporte, donde se configura la conexión de datos principal
            const string ERR_DESCONOCIDO = "Error al generar el pdf. Por favor revise la bitacora.";
            string err = ERR_DESCONOCIDO;
            SCOREEntitiesHelper.WriteToLog(ERR_DESCONOCIDO);
            if (_config.DebugExtendido)
            {
                err += $" Ubicación del log: {SCOREEntitiesHelper.LogPath}";

                SCOREEntitiesHelper.WriteToLog("Este escenario puede suceder si el reporte activa la opción 'Imprimir sólo si hay datos diponibles' y no hay datos.", false);
            }
            return new JsonResult(err);
        }    
        
        delegate void ConfigurarParametros(ReporteHemoeco rp);
        
        private void ConfigurarNumOT(ReporteHemoeco rp)
        {
            // Asignar el numero de OT al reporte.
            if (rp.Params.Count > 0)
            {
                // SCOREEntitiesHelper.WriteToLog($"asignar parametros");
                // NUM_OT es un parametro tipo entero en el reporte OT
                rp.Params["NUM_OT"].Value = _idOT;
            }
        }
        
    }

Clase personalizada de 'Report':

        // Specialized class derived from reportman's 'Report'
        ...
        public class ReporteHemoeco : Report
        {
        ...
        internal PrintOutPDF PrintPdf
        {
            get
            {
                if (_printPdf == null)
                {
                    _printPdf = new PrintOutPDF();
                }

                return _printPdf;
            }
        }

        private PrintOutPDF _printPdf = null;
        ...
        internal bool ImprimirPdf()
        {
            try
            {
                PrintPdf.FileName = _nombrePdf;
                PrintPdf.Compressed = true;

                return PrintPdf.Print(MetaFile);
            }
            catch (Exception ex)
            {
                SCOREEntitiesHelper.WriteToLog(ex);
                return false;
            }
        }
            
        }
                 
Reporteador tomado de https://opensource.com/article/21/10/disagreement-open-source utilizado para diseñar formatos WYSIWYG e imprimir en pdf desde score.

Reportman designer utiliza .Net Framework 4.6.1

En Score se utiliza dentro de HemoecoAPI que utiliza .Net Core 3.1, las bibliotecas requeridas se adaptaron para tal fin.

Se utiliza Visual Studio 2019 y 2022 como IDE para estos proyectos.

Se recopila un poo de información en cuanto a nuestra experiencia con reportman en el documento 'Impresión de documentos pdf' (https://docs.google.com/document/d/1R3f3Rc-Dd38Su0PhVQCt8prwvU6BCZuLJXUIKCROHik/edit#heading=h.94yreql7w12j)


        
