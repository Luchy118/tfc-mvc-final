using Microsoft.Reporting.WebForms;
using System;
using static WebApplication1.Models.GeneralModel;

namespace VisualNet
{
    public partial class PaginaInforme : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DatosInforme datosInforme = (DatosInforme)Session["Informe_Datos"];
                    foreach (var origen in datosInforme.OrigenesDatos)
                        ReportViewer.LocalReport.DataSources.Add(origen);
                    ReportViewer.LocalReport.ReportPath = datosInforme.DireccionInforme;
                    //ReportViewer.LocalReport.SetParameters(datosInforme.Parametros);
                    ReportViewer.LocalReport.EnableExternalImages = true;
                    if (!ReportViewer.LocalReport.IsReadyForRendering)
                        throw new Exception("No se ha podido cargar el informe");
                    ReportViewer.LocalReport.Refresh();
                }
            }
            catch (Exception ex)
            {
                divError.Visible = true;
                txtError.InnerText = ex.Message;
                if (ex.InnerException != null && ex.Message != ex.InnerException.Message)
                    txtError2.InnerText = ex.InnerException.Message;
            }
        }
    }

}