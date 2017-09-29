using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel.dtObj
{
    public enum TipoTrasferimento
    {
        SedeEstero = 1,
        EsteroEstero = 2,
        EsteroEsteroStessaRegiona = 3
    }
    public class dtIndennitaSistemazione : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IndennitaSistemazioneModel GetIndennitaSistemazione()
        {
            return null;
        }

    }
}