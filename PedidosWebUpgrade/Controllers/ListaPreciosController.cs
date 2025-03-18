using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Mvc;
using PedidosWebUpgrade.Domain.Entities;
using PedidosWebUpgrade.Domain.ViewModels;
using PedidosWebUpgrade.Infrastructure.Repository;
using PedidosWebUpgrade.Infrastructure.Utilities;
using Shyjus.BrowserDetection;
using System.Collections;

namespace PedidosWebUpgrade.Web.Controllers
{
    public class ListaPreciosController : Controller
    {
        private readonly ConfigVariables _configVariables;
        private readonly IBrowserDetector _browserDetector;
        private readonly IWebHostEnvironment _env;

        public ListaPreciosController(ConfigVariables configVariables, IBrowserDetector browserDetector, IWebHostEnvironment env)
        {
            _configVariables = configVariables;
            _browserDetector = browserDetector;
            _env = env;
        }
        
        // GET: ListaPrecios
        [HttpGet()]
        public IActionResult ConsultarCabListaPrecios()
        {
            List<F45520> Modelo = new List<F45520>();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "ListaPrecios/ConsultarCabListaPrecios");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo = new ListaPreciosRepository(_configVariables).ConsultarCabeceraListoPrecio().ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ListaPreciosController", "HttpGet-ConsultarCabListaPrecios", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpGet()]
        public IActionResult ConsultaDetListaPrecios(string phdoco, string phdcto)
        {
            ListaPreciosViewModel Modelo = new ListaPreciosViewModel();
            try
            {
                //PERMISOS DE USUARIO
                List<Menu> _Permisos = new UsuarioRepository(_configVariables).ObtenerPermisos(int.Parse(HttpContext.Session.GetString("idusuario")), "ListaPrecios/ConsultaDetListaPrecios");
                TempData["crear"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeCrear);
                TempData["consultar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeConsultar);
                TempData["actualizar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeActualizar);
                TempData["eliminar"] = Convert.ToInt32(_Permisos.FirstOrDefault().PuedeEliminar);

                Modelo.CabListPrecios = new ListaPreciosRepository(_configVariables).ConsultarCabeceraListoPrecio().FirstOrDefault();
                Modelo.ListDetListPrecios = new ListaPreciosRepository(_configVariables).ConsultarDetalleListaPrecio(phdoco, phdcto).ToList();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ListaPreciosController", "HttpGet-ConsultaDetListaPrecios", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return View(Modelo);
        }

        [HttpGet]
        public IActionResult ImprimirReporteListaPrecios(string doco, string dcto)
        {
            try
            {
                byte[] bytes;
                bytes = ConstruirPDF(doco, dcto, int.Parse(HttpContext.Session.GetString("idusuario")));
                return File(bytes, "application/pdf", "ReporteListPrecios.pdf");

                //Response.Clear();
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("Content-Disposition", "attachment; filename=ReporteListPrecios.pdf");
                //Response.Buffer = true;
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.BinaryWrite(bytes);
                //Response.End();
            }
            catch (Exception e)
            {
                CustomUtility.RegistrarExcepcion(_configVariables.LogDirectory,"ListaPreciosController", "HttpGet()-GenerarPDF()", e.ToString(), _browserDetector.Browser.Name, _browserDetector.Browser.Version);
            }
            return RedirectToAction("ConsultarCabListaPrecios", "ListaPrecios");
        }

        public byte[] ConstruirPDF(string doco, string dcto, int IdUsuario)
        {
            List<ListPreciosPrint> _Detalle = new List<ListPreciosPrint>();
            _Detalle = new ListaPreciosRepository(_configVariables).ConsultarDetallesReporteListaPrecios(doco, dcto, IdUsuario);

            byte[] bytes;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document doc = new Document(PageSize.LETTER, 40f, 40f, 40f, 40f);
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                //fuente del contenido
                Font _smallFontC = new Font(Font.FontFamily.HELVETICA, 7, Font.BOLD, BaseColor.BLACK);
                Font _smallFontWhite = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL, BaseColor.WHITE);
                Font _standardFontC = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK);
                Font _standardFontP = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.BLACK);
                Font _standardFontN = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
                Font _standardFontG = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.BLACK);
                Font _standardFontS = new Font(Font.FontFamily.HELVETICA, 10, Font.UNDERLINE, BaseColor.BLACK);
                Paragraph spacerParagraph = new Paragraph();

                //ENCABEZADO
                PdfPTable tblEncabezadoText = new PdfPTable(1);
                tblEncabezadoText.AddCell(new PdfPCell(new Paragraph(_Detalle.FirstOrDefault().Heder_Line1, _standardFontG)) { Border = 0, HorizontalAlignment = 0 });
                tblEncabezadoText.AddCell(new PdfPCell(new Paragraph(_Detalle.FirstOrDefault().Header_Line2, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblEncabezadoText.AddCell(new PdfPCell(new Paragraph(_Detalle.FirstOrDefault().Header_Line3, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblEncabezadoText.AddCell(new PdfPCell(new Paragraph(_Detalle.FirstOrDefault().List_Id, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });

                //ENCABEZADO
                PdfPTable tblEncabezado = new PdfPTable(3);
                float[] anchotblEncabezado = new float[3];
                anchotblEncabezado[0] = 250;
                anchotblEncabezado[1] = 250;
                anchotblEncabezado[2] = 250;
                tblEncabezado.SetWidths(anchotblEncabezado);
                tblEncabezado.WidthPercentage = 100;

                Image img1 = null;
                Image img2 = null;

                //PARA LAS IMAGENES QUE APARECEN EN LA CABECERA
                if (System.IO.File.Exists(_env.WebRootPath + "~/Content/img/sitio/Diplomatico.png"))
                {
                    img1 = Image.GetInstance(_env.WebRootPath + "~/Content/img/sitio/Diplomatico.png");
                    img1.ScaleAbsolute(100f, 53f);
                }

                if (System.IO.File.Exists(_env.WebRootPath + "~/Content/img/sitio/logo_irum_lista_p.png"))
                {
                    img2 = Image.GetInstance(_env.WebRootPath + "~/Content/img/sitio/logo_irum_lista_p.png");
                    img2.ScaleAbsolute(100f, 53f);
                }

                tblEncabezado.AddCell(new PdfPCell(tblEncabezadoText) { Border = 0, HorizontalAlignment = 0 });//0=Left, 1=Center, 2=Right
                tblEncabezado.AddCell(new PdfPCell(img1) { Border = 0, HorizontalAlignment = 1 });
                tblEncabezado.AddCell(new PdfPCell(img2) { Border = 0, HorizontalAlignment = 2 });

                doc.Add(tblEncabezado);
                doc.Add(spacerParagraph);
                doc.Add(Chunk.NEWLINE);

                //PARA LOS DATOS EL CUERPO DEL REPORTE
                string clasification = string.Empty;
                ushort count = 0;
                PdfPTable tblgeneral = new PdfPTable(3);
                PdfPTable tblHelpTbl = new PdfPTable(1);
                foreach (ListPreciosPrint item in _Detalle)
                {
                    if (count == 0)
                    {
                        clasification = item.Clasificacion;
                        tblgeneral = new PdfPTable(3);
                        float[] anchotblitems = new float[3];
                        anchotblitems[0] = 250;
                        anchotblitems[1] = 250;
                        anchotblitems[2] = 250;
                        tblgeneral.SetWidths(anchotblitems);
                        tblgeneral.WidthPercentage = 100;

                        tblgeneral.AddCell(new PdfPCell(new Paragraph(item.Clasificacion, _standardFontG)) { Border = 0, BorderColor = new BaseColor(11, 93, 24), HorizontalAlignment = 1, VerticalAlignment = 4, Colspan = 3 });
                        tblgeneral.AddCell(new PdfPCell(new Paragraph("Product", _smallFontWhite)) { Border = 0, HorizontalAlignment = 1, BackgroundColor = new BaseColor(11, 93, 24) });
                        tblgeneral.AddCell(new PdfPCell(new Paragraph("Cases Of", _smallFontWhite)) { Border = 0, HorizontalAlignment = 1, BackgroundColor = new BaseColor(11, 93, 24) });
                        tblgeneral.AddCell(new PdfPCell(new Paragraph("Price Per Case" + " \n" + item.Price_Header, _smallFontWhite)) { Border = 0, HorizontalAlignment = 1, BackgroundColor = new BaseColor(11, 93, 24) });
                    }

                    if (clasification.ToLower().Trim() != item.Clasificacion.ToLower().Trim())
                    {
                        doc.Add(tblgeneral);
                        doc.Add(spacerParagraph);
                        doc.Add(Chunk.NEWLINE);

                        tblgeneral = new PdfPTable(3);
                        float[] anchotblitems = new float[3];
                        anchotblitems[0] = 250;
                        anchotblitems[1] = 250;
                        anchotblitems[2] = 250;
                        tblgeneral.SetWidths(anchotblitems);
                        tblgeneral.WidthPercentage = 100;

                        tblgeneral.AddCell(new PdfPCell(new Paragraph(item.Clasificacion, _standardFontG)) { Border = 0, BorderColor = new BaseColor(11, 93, 24), HorizontalAlignment = 1, VerticalAlignment = 4, Colspan = 3 });
                        tblgeneral.AddCell(new PdfPCell(new Paragraph("Product", _smallFontWhite)) { Border = 0, HorizontalAlignment = 1, BackgroundColor = new BaseColor(11, 93, 24) });
                        tblgeneral.AddCell(new PdfPCell(new Paragraph("Cases Of", _smallFontWhite)) { Border = 0, HorizontalAlignment = 1, BackgroundColor = new BaseColor(11, 93, 24) });
                        tblgeneral.AddCell(new PdfPCell(new Paragraph("Price Per Case" + " \n" + item.Price_Header, _smallFontWhite)) { Border = 0, HorizontalAlignment = 1, BackgroundColor = new BaseColor(11, 93, 24) });
                    }

                    tblHelpTbl = new PdfPTable(1);
                    tblHelpTbl.AddCell(new PdfPCell(new Paragraph(item.Product_Brand_Line1, _standardFontC)) { Border = 0, HorizontalAlignment = 1 });
                    tblHelpTbl.AddCell(new PdfPCell(new Paragraph(item.Product_Brand_Line2, _standardFontN)) { Border = 0, HorizontalAlignment = 1 });
                    tblHelpTbl.AddCell(new PdfPCell(new Paragraph(item.Product_Brand_Line3, _standardFontC)) { Border = 0, HorizontalAlignment = 1 });
                    tblgeneral.AddCell(new PdfPCell(tblHelpTbl) { Border = 3, BorderColor = new BaseColor(11, 93, 24), HorizontalAlignment = 1 });

                    tblHelpTbl = new PdfPTable(1);
                    tblHelpTbl.AddCell(new PdfPCell(new Paragraph(item.CasesOf_Line1, _standardFontC)) { Border = 0, HorizontalAlignment = 1 });
                    tblHelpTbl.AddCell(new PdfPCell(new Paragraph(item.CasesOf_Line2, _standardFontN)) { Border = 0, HorizontalAlignment = 1 });
                    tblHelpTbl.AddCell(new PdfPCell(new Paragraph(item.CasesOf_Line3, _standardFontC)) { Border = 0, HorizontalAlignment = 1 });
                    tblgeneral.AddCell(new PdfPCell(tblHelpTbl) { Border = 3, BorderColor = new BaseColor(11, 93, 24), HorizontalAlignment = 1 });

                    tblgeneral.AddCell(new PdfPCell(new Paragraph(" \n" + item.Price, _standardFontN)) { Border = 3, BorderColor = new BaseColor(11, 93, 24), HorizontalAlignment = 1, VerticalAlignment = 1 });

                    count++;
                    clasification = item.Clasificacion;

                    if (count == _Detalle.Count)
                    {
                        doc.Add(tblgeneral);
                        doc.Add(spacerParagraph);
                        doc.Add(Chunk.NEWLINE);
                    }
                }

                //COMENTARIOS
                PdfPTable tblcoments = new PdfPTable(1);
                float[] anchotblcoments = new float[1];
                anchotblcoments[0] = 600;
                tblcoments.SetWidths(anchotblcoments);
                tblcoments.WidthPercentage = 100;

                tblcoments.AddCell(new PdfPCell(new Paragraph("Please note:", _standardFontP)) { Border = 0, HorizontalAlignment = 0 });
                tblcoments.AddCell(new PdfPCell(new Paragraph("  " + _Detalle.FirstOrDefault().Note1, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblcoments.AddCell(new PdfPCell(new Paragraph("  " + _Detalle.FirstOrDefault().Note2, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblcoments.AddCell(new PdfPCell(new Paragraph("  " + _Detalle.FirstOrDefault().Note3, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblcoments.AddCell(new PdfPCell(new Paragraph("  " + _Detalle.FirstOrDefault().Note4, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblcoments.AddCell(new PdfPCell(new Paragraph("  " + _Detalle.FirstOrDefault().Note5, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });

                doc.Add(tblcoments);
                doc.Add(Chunk.NEWLINE);

                //CONTACTS
                PdfPTable tblfooter = new PdfPTable(1);
                float[] anchotblfooter = new float[1];
                anchotblfooter[0] = 600;
                tblfooter.SetWidths(anchotblfooter);
                tblfooter.WidthPercentage = 100;
                tblfooter.AddCell(new PdfPCell(new Paragraph("Contact:", _standardFontP)) { Border = 0, HorizontalAlignment = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Detalle.FirstOrDefault().Contact_1, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Detalle.FirstOrDefault().Contact_2, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Detalle.FirstOrDefault().Contact_3, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                tblfooter.AddCell(new PdfPCell(new Paragraph(_Detalle.FirstOrDefault().Contact_4, _standardFontC)) { Border = 0, HorizontalAlignment = 0 });
                doc.Add(tblfooter);

                doc.Close();
                bytes = memoryStream.ToArray();
                memoryStream.Close();
            }
            return bytes;
        }
    }
}
