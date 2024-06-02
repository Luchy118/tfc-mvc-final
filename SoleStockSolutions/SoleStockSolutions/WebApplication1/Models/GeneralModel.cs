using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class GeneralModel
    {
        public struct DatosInforme
        {
            public string DireccionInforme;
            public IEnumerable<ReportDataSource> OrigenesDatos;
            public IEnumerable<ReportParameter> Parametros;
        }
    }
}