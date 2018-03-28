function TornaElencoIb() {
        var rotta = "/Notifiche/ListaNotifiche";
        $.ajax({
            type: "POST",
            url: rotta,
            //  data: { idTipoContributo: idTipoContributo, escludiAnnullati: chk },
            dataType: 'html',
            beforeSend: function () {
                //debugger;
                //VerificaAutenticazione();
                //$("#DialogNewDoc").dialog("destroy");
                //$("#divEffettoLoadAutNoDoc").show("slow");
            },
            success: function (result) {
                //debugger;
                //$("#divEffettoLoadAutNoDoc").hide("slow");
                $("#divPanelNotifiche").empty();
                $("#divPanelNotifiche").html(result);
                //RicercaDocumenti();
            },
            complete: function () {
                //$("#divEffettoLoadAutNoDoc").hide("slow");
                //                $("#Centro").getNiceScroll().resize();
            },
            error: function (jqXHR, textStatus, errorThrow) {
                //debugger;
                var msg = errorThrow.err;
                AlertDialog(msg);
            }
        });
    }
//$("#lDestinatari").select2({
//    placeholder: "Seleziona tipo trasferimento",
//    allowClear: true,
//    language: "it",
//    width: '150',
//    tags: true
//});
$(".js-example-tokenizer").select2({
    tags: true,
    tokenSeparators: [',', ' ']
})

function RemoveAll1()
{
    var listaDest1 = $("#lDestinatari").select2('val');
    if (listaDest1 != null) {
        //  debugger;
        var l1 = listaDest1.toString().split(',');
        for (var j = 1; j < l1.length; j++) {//j=1 in quanto tutti è di indice 0
            var $select = $('#lDestinatari');
            var idToRemove = l1[j];
            var values = $select.val();
            if (values) {
                var i = values.indexOf(idToRemove);
                if (i >= 0) {
                    values.splice(i, 1);
                    $select.val(values).change();
                }
            }
        }
    }
    //for the ToCc
    var listaDest2 = $("#toCc").select2('val');
    if (listaDest2 != null) {
        //  debugger;
        var l2 = listaDest2.toString().split(',');
        for (var j = 0; j < l2.length; j++) {      //qui tutto devi levare          
            var $select = $('#toCc');
            var idToRemove = l2[j];
            var values = $select.val();
            if (values) {
                var i = values.indexOf(idToRemove);
                if (i >= 0) {
                    values.splice(i, 1);
                    $select.val(values).change();
                }
            }
        }
    }
}

function PrelevaPVListaDestinatari()
{
  //  debugger;
    var listaDest1 = $("#lDestinatari").select2('val');
    var listaDest2 = $("#toCc").select2('val');
    // alert("1:"+listaDest1 +'\n\n2:'+listaDest2 );

    if (listaDest1 != null && listaDest1.indexOf(",") == -1)
        listaDest1 = listaDest1.toString() + ",";
    if (listaDest2 != null && listaDest2.indexOf(",") == -1)
        listaDest2 = listaDest2.toString() + ",";

    if (listaDest1 != null) {
        if (listaDest1.indexOf('TUTTI') != -1) {
            RemoveAll1();
            $('#toCc').attr('disabled', true);
        }
        else {
            $('#toCc').attr('disabled', false);
        }
    }
    else $('#toCc').attr('disabled', false);

    if (listaDest2 != null && listaDest1 != null) {
        var l1 = listaDest1.toString().toString().split(',');
        for (var j = 0; j < l1.length; j++) {
            if (l1[j] != '') {
                if (listaDest2.toString().indexOf(l1[j]) != -1) {
                    //alert('Attenzione: ' + l1[j] + ' presente nel settore Cc');
                    var $select = $('#lDestinatari');
                    var idToRemove = l1[j];
                    var values = $select.val();
                    if (values) {
                        var i = values.toString().indexOf(idToRemove);
                        if (i >= 0) {
                            values.splice(i, 1);
                            $select.val(values).change();
                        }
                    }
                    $('#vai1').show();
                    return;
                }
            }
        }               
    }
    hideAll();
}
function PrelevaPVListaCC()
{
   // debugger;
    var listaDest1 = $("#lDestinatari").select2('val');
    var listaDest2 = $("#toCc").select2('val');

    if (listaDest1 != null && listaDest1.indexOf(",") == -1)
        listaDest1 = listaDest1.toString() + ",";
    if (listaDest2 != null && listaDest2.indexOf(",") == -1)
        listaDest2 = listaDest2.toString() + ",";

    if (listaDest2 != null && listaDest1 != null) {
        var l2 = listaDest2.toString().split(',');
        for (j = 0; j < l2.length; j++) {
            if (l2[j] != '') {
                if (listaDest1.toString().indexOf(l2[j]) != -1) {
                    // alert('Attenzione: ' + l2[j] + ' presente nel settore Destinatari');
                    var $select = $('#toCc');
                    var idToRemove = l2[j];
                    var values = $select.val();
                    if (values) {
                        var i = values.indexOf(idToRemove);
                        if (i >= 0) {
                            values.splice(i, 1);
                            $select.val(values).change();
                        }
                    }
                    $('#vai2').show();
                    return;
                }
            }
        }           
    }
    hideAll();
}
function hideAll()
{
    $('#vai2').hide(); $('#vai1').hide();
    $('#vaiOgg').hide(); $('#vaiDest').hide();
    $('#vaiCorpoMess').hide();
}

//$('#formNuovaNotifica').submit(function (e) {
function Register() {
    hideAll();
    var listaDest1 = $("#lDestinatari").select2('val');
    // var listaDest2 = $("#toCc").select2('val');
    if (listaDest1 == null) {
        $('#vaiDest').show();
        return false;
    }
    if ($("#Oggetto").val().toString().trim() == '') {
        $('#vaiOgg').show();
        return false;
    }
    //debugger;

    // $('#corpoMessaggio').val(CKEDITOR.instances.corpoMessaggio.getData());
    CKEDITOR.instances["corpoMessaggio"].updateElement();
    // alert(CKEDITOR.instances.corpoMessaggio.getData());
    if (CKEDITOR.instances.corpoMessaggio.getData().toString().trim() == '') {
        $('#vaiCorpoMess').show();
        return false;
    }

    // alert(CKEDITOR.instances.corpoMessaggio.getData().toString());//.replace('<p>', '').replace('</p>', ''));
    var contenutoMessaggio = CKEDITOR.instances.corpoMessaggio.getData().toString();
    var rotta = "/Notifiche/RegistraNotifiche";
    $.ajax({
        type: "POST",
        url: rotta,
        data: { contenutoMessaggio: contenutoMessaggio },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
            //$("#DialogNewDoc").dialog("destroy");
            //$("#divEffettoLoadAutNoDoc").show("slow");
        },
        success: function (result) {
            //debugger;
            //$("#divEffettoLoadAutNoDoc").hide("slow");
            $("#divPanelNotifiche").empty();
            $("#divPanelNotifiche").html(result);
        },
        complete: function () {
            //$("#divEffettoLoadAutNoDoc").hide("slow");
            //                $("#Centro").getNiceScroll().resize();
        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            AlertDialog(msg);
        }
    });
}

//$('#formNuovaNotifica').submit(function (e) {
function SalvaNotificaConPDF() {
    //  debugger;
   
    hideAll();
  
    var listaDest1 = $("#lDestinatari").select2('val');
     var listaDest2 = $("#toCc").select2('val');
    if (listaDest1 == null) {
        $('#vaiDest').show();
        $("#lDestinatari").focus();
        return false;
    }
    if ($("#Oggetto").val().toString().trim() == '') {
        $('#vaiOgg').show();
        $("#Oggetto").focus();
        return false;
    }
    var Oggetto = $("#Oggetto").val().toString().trim();
    //   debugger;
    CKEDITOR.instances["corpoMessaggio"].updateElement();
    if (CKEDITOR.instances.corpoMessaggio.getData().toString().trim() == '') {
        $('#vaiCorpoMess').show();
        CKEDITOR.instances.corpoMessaggio.focus();
        return false;
    }    
    var CorpoMessaggio = CKEDITOR.instances.corpoMessaggio.getData().toString().trim();
   
    SalvaDocumentoNotifica(listaDest1, listaDest2, Oggetto, CorpoMessaggio);
}
function SalvaDocumentoNotifica(listaMailPrincipale, listaMailToCc, Oggetto, CorpoMessaggio)
{
    // debugger;
    Blocca('Please wait...');
    var datiForm = new FormData();
    var rotta = "/Notifiche/InserisciNuovaNotifica";

    var file = $("#PDFUpload")[0].files[0];

        datiForm.append("listaMailPrincipale", listaMailPrincipale);
        datiForm.append("listaMailToCc", listaMailToCc);
        datiForm.append("Oggetto", Oggetto);
        datiForm.append("CorpoMessaggio", CorpoMessaggio);
        datiForm.append("file", file);

        $.ajax({
            url: rotta,
            type: "POST", //Le info testuali saranno passate in POST
            data: datiForm, //I dati, forniti sotto forma di oggetto FormData
            dataType: 'html',
            cache: false,
            async: false,
            beforeSend: function () {
                //debugger;
               VerificaAutenticazione();
                Blocca();
            },
            processData: false, //Serve per NON far convertire l’oggetto
            //FormData in una stringa, preservando il file
            contentType: false, //Serve per NON far inserire automaticamente
            //un content type errato
            success: function (result) {
                if (result.err != "" && result.err != undefined) {
                    MsgErroreJson(result.err);
                }
                $("#divPanelNotifiche").empty();
                $("#divPanelNotifiche").html(result);
            },
            complete: function () {
                //$("#btUpload").removeAttr("disabled");
            },
            error: function (error) {
                debugger;               
                var msg = error.responseText;
                MsgErroreJson(msg);
            }
        });
    
}



