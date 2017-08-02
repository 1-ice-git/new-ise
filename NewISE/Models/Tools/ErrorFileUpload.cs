using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewISE.Models.Tools
{
    [Serializable]
    public class ErrorFileUpload
    {
        //var errore = new
        //{
        //    error: "Imagem deve estar na proporção 16:9.",
        //    initialPreview: [],
        //    initialPreviewConfig: [],
        //    initialPreviewThumbTags: [],
        //    append: true,
        //};

        public string error { get; set; }
        public ArrayList initialPreview { get; set; }
        public ArrayList initialPreviewConfig { get; set; }
        public ArrayList initialPreviewThumbTags { get; set; }
        public bool append { get; set; }
    }
}