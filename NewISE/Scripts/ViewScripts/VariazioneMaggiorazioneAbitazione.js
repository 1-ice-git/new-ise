function GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/GestionePulsantiMAB_var";
    $.ajax({
        type: "POST",
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#divAttivazioneMAB_var").empty();
            $("#divAttivazioneMAB_var").html(result);
            //CalcolaAnticipo();
            Sblocca();
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }

    });
}

function AttivitaMAB_var(idTrasferimento) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/AttivitaMAB_var";

    $.ajax({
        type: "POST",
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#tabAttivitaMAB_var").empty();
            $("#tabAttivitaMAB_var").html(result);
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }

    });
}

function GestioneMAB_var(idTrasferimento) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/GestioneMAB_var";

    $.ajax({
        type: "POST",
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#divMaggiorazioneAbitazione_var").empty();
            $("#divMaggiorazioneAbitazione_var").html(result);
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }

    });
}

function UploadDocumentoMABModal_var(idTipoDocumento, idTrasferimento) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/DocumentoMAB_var";
    $('#hiIdTipoDocumento').val(idTipoDocumento);

    $.ajax({
        url: rotta,
        type: "POST", //Le info testuali saranno passate in POST
        data: { idTrasferimento: idTrasferimento, idTipoDocumento: idTipoDocumento },
        dataType: 'html',
        async: false,
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
            $('#inviaDocMABModal_var').modal('show');
            //Blocca();

        },
        success: function (result) {
            //debugger;
            $("#viewDocumentoMAB_var").empty();
            $("#viewDocumentoMAB_var").html(result);

        },
        error: function (error) {
            //debugger;
            //Sblocca();
            var msg = error.responseText;
            ErroreElaborazioneAjax(msg);
        }
    });

}

function CloseTimeModalMAB_var(idTrasferimento, idMab) {
    //debugger;
    $("#btUploadDocMAB_var").attr("disabled", "disabled");
    $("#btUploadDocMAB_var").addClass("disabled");
    $("#btAnnullaDocMAB_var").attr("disabled", "disabled");
    $("#btAnnullaDocMAB_var").addClass("disabled");
    setTimeout(CloseModalFileMAB_var(idTrasferimento, idMab), 2000);
}

function CloseModalFileMAB_var(idTrasferimento, idMab) {
    //debugger;
    $('#ModalNuovoFormularioMAB_Doc').modal('hide');
    setTimeout(ElencoFormulariMABInseriti(idTrasferimento, idMab), 1000);
}

function ValidazioneMAB_var(idMab) {
    //debugger;
    var ret = false;
    var c1 = false;
    var c2 = false;
    var c3 = false;

    var file = $("#file").val();

    if (idMab > 0) {


        if (file != null && file != undefined && file != "") {
            $("#divFile").removeClass('BordoRosso');
            c3 = true;
        }
        else {
            $("#divFile").addClass('BordoRosso');
            c3 = false;
        }

        if (c3) {
            ret = true;
        }
        else {
            ret = false;
        }

    }

    return ret;
}

function SalvaDocumentoMAB_var(idMab, idTipoDocumento)
{
    //debugger;
    var datiForm = new FormData();
    var rotta = "/VariazioneMaggiorazioneAbitazione/InserisciDocumentoMAB_var";
    
    var idTrasferimento = parseInt($("#idTrasferimento").val());
    //var idTipoDocumento = parseInt('@idTipoDocumento');
    var file = $("#file")[0].files[0];

    if (ValidazioneMAB_var(idMab)) {

        datiForm.append("idMab", idMab);
        datiForm.append("idTipoDocumento", idTipoDocumento);
        datiForm.append("file", file);


        $.ajax({
            url: rotta,
            type: "POST", //Le info testuali saranno passate in POST
            data: datiForm, //I dati, forniti sotto forma di oggetto FormData
            dataType: 'json',
            cache: false,
            async: false,
            beforeSend: function () {
                //debugger;
                VerificaAutenticazione();
                //Blocca();
            },
            processData: false, //Serve per NON far convertire l’oggetto
            //FormData in una stringa, preservando il file
            contentType: false, //Serve per NON far inserire automaticamente
            //un content type errato
            success: function (result) {
                //debugger;
                if (result.err != "" && result.err != undefined) {
                    MsgErroreJson(result.err);
                }
                else {
                    MsgErroreJson(result.msg);

                    CloseTimeModalMAB_var(idTrasferimento, idMab);

                    GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
                }

            },
            complete: function () {
                //$("#btUpload").removeAttr("disabled");

            },
            error: function (error) {
                //debugger;
                //Sblocca();
                $("#btUploadDocMAB_var").removeAttr("disabled");
                $("#btUploadDocMAB_var").removeClass("disabled");
                $("#btAnnullaDocMAB_var").removeAttr("disabled");
                $("#btAnnullaDocMAB_var").removeClass("disabled");
                var msg = error.responseText;
                MsgErroreJson(msg);
            }
        });
    }
    else {
        $("#spantxtError").html("Selezionare documento.");
        $("#spanNomeFile").html("");
    }

}

function MsgErroreJson(msg) {
    $("#spMsgError").html(msg);
}

function ConfermaNotificaRichiestaMAB_var() {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ConfermaNotificaRichiestaMAB_var";
    var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
    var idAttivazioneMAB = parseInt($('#hi_idAttivazioneMAB').val());


    $.ajax({
        type: "POST",
        url: rotta,
        data: { idAttivazioneMAB: idAttivazioneMAB },
        dataType: 'json',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            if (result.err == "") {
                //InfoElaborazioneAjax(result.msg);
                AttivitaMAB_var(idTrasferimento);
                ElencoFormulariMABInseriti(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
            } else {
                ErroreElaborazioneAjax(result.err);
            }
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }

    });
}

function ConfermaAnnullaRichiestaMAB_var() {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ConfermaAnnullaRichiestaMAB_var";
    var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
    var idAttivazioneMAB = parseInt($('#hi_idAttivazioneMAB').val());
    var testoAnnullaMAB = $('#FullDescription').val();

    $.ajax({
        type: "POST",
        url: rotta,
        data: { idAttivazioneMAB: idAttivazioneMAB, msg:testoAnnullaMAB },
        dataType: 'json',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            if (result.err == "") {
                //InfoElaborazioneAjax(result.msg);
                AttivitaMAB_var(idTrasferimento);
                ElencoFormulariMABInseriti(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
            } else {
                ErroreElaborazioneAjax(result.err);
            }
        },
        complete: function () {
        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }
    });
}

function ConfermaAttivaRichiestaMAB_var() {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ConfermaAttivaRichiestaMAB_var";
    var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
    var idAttivazioneMAB = parseInt($('#hi_idAttivazioneMAB').val());
    $.ajax({
        type: "POST",
        url: rotta,
        data: { idAttivazioneMAB: idAttivazioneMAB },
        dataType: 'json',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
            //$("#DialogNewDoc").dialog("destroy");
            //$("#divEffettoLoadAutNoDoc").show("slow");
        },
        success: function (result) {
            //debugger;
            if (result.err == "") {
                //InfoElaborazioneAjax(result.msg);
                AttivitaMAB_var(idTrasferimento);
                ElencoFormulariMABInseriti(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
            } else {
                ErroreElaborazioneAjax(result.err);
            }
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }
    });
}

function NuovaMAB_var(idTrasferimento) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/NuovaMAB_var";

    $.ajax({
        type: "POST",
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#tabAttivitaMAB_var").empty();
            $("#tabAttivitaMAB_var").html(result);
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }

    });
}

function ModificaMAB_var(idMAB) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ModificaMAB_var";

    $.ajax({
        type: "POST",
        url: rotta,
        data: { idMAB: idMAB },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#tabAttivitaMAB_var").empty();
            $("#tabAttivitaMAB_var").html(result);
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }

    });
}

function CanoneMAB(idTrasferimento, idMAB) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ElencoCanoneMAB";

    $.ajax({
        type: "POST",
        url: rotta,
        data: {
            idMAB: idMAB,
            idTrasferimento: idTrasferimento
        },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#tabAttivitaMAB_var").empty();
            $("#tabAttivitaMAB_var").html(result);
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }

    });
}

function PagatoCondivisoMAB(idTrasferimento, idMAB) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ElencoPagatoCondivisoMAB";

    $.ajax({
        type: "POST",
        url: rotta,
        data: {
            idMAB: idMAB,
            idTrasferimento: idTrasferimento
        },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#tabAttivitaMAB_var").empty();
            $("#tabAttivitaMAB_var").html(result);
        },
        complete: function () {

        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }

    });
}

function MessaggioAnnullaRichiestaMAB_var() {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/MessaggioAnnullaMAB_var";
    var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
    $.ajax({
        type: "POST",
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#viewAnnullaRichiestaMAB_var").empty();
            $("#viewAnnullaRichiestaMAB_var").html(result);
        },
        complete: function () {
        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }
    });
}

function AggiornaElencoCanoneMAB(idTrasferimento, idMAB) {
    CanoneMAB(idTrasferimento, idMAB) 
}

function AggiornaElencoPagatoCondivisoMAB(idTrasferimento, idMAB) {
    PagatoCondivisoMAB(idTrasferimento, idMAB)
}

function ConfermaAnnullaModificheCanoneMAB() {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ConfermaAnnullaModificheCanoneMAB";
    var idMAB = parseInt($('#hdIdMAB').val());
    var idTrasferimento = parseInt($('#hdIdTrasferimento').val());

    $.ajax({
        type: "POST",
        url: rotta,

        data: {
            idMAB: idMAB
        },
        dataType: 'json',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            if (result.err == "") {
                $('#AnnullaModificheCanoneMABModal').modal('hide');
                AggiornaElencoCanoneMAB(idTrasferimento, idMAB);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
            } else {
                ErroreElaborazioneAjax(result.err);
            }
        },
        complete: function () {
            //debugger;
        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }
    });
}

function ConfermaAnnullaModifichePagatoCondivisoMAB() {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ConfermaAnnullaModifichePagatoCondivisoMAB";
    var idMAB = parseInt($('#hdIdMAB').val());
    var idTrasferimento = parseInt($('#hdIdTrasferimento').val());

    $.ajax({
        type: "POST",
        url: rotta,

        data: {
            idMAB: idMAB
        },
        dataType: 'json',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            if (result.err == "") {
                $('#AnnullaModifichePagatoCondivisoMABModal').modal('hide');
                AggiornaElencoPagatoCondivisoMAB(idTrasferimento, idMAB);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
            } else {
                ErroreElaborazioneAjax(result.err);
            }
        },
        complete: function () {
            //debugger;
        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            ErroreElaborazioneAjax(msg);
        }
    });
}

function ElencoFormulariMABInseriti(idTrasferimento, idMab) {
    var rotta = "/VariazioneMaggiorazioneAbitazione/ElencoFormulariMABInseriti";
    //var idMagFam = parseInt('@idMagFam');
    //var idTrasferimento = parseInt($('#hdIdTrasferimento').val());

    $.ajax({
        type: "POST",
        url: rotta,
        data: {
            idTrasferimento: idTrasferimento,
            idMab:idMab
        },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#tabAttivitaMAB_var").empty();
            $("#tabAttivitaMAB_var").html(result);
        },
        complete: function () {
            GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
        },
        error: function (jqXHR, textStatus, errorThrow) {
            //debugger;
            var msg = errorThrow.err;
            AlertDialog(msg);
        }

    });
}

function NuovoFormularioMAB(idMab) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/NuovoFormularioMAB_TipoDoc";
    //var idTrasferimento = parseInt($('#hdIdTrasferimento').val());

    $.ajax({
        url: rotta,
        type: "POST", //Le info testuali saranno passate in POST
        data: { idMab: idMab },
        dataType: 'html',
        async: false,
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
            //$('#inviaDocModal').modal('show');
            //Blocca();
            $('#ModalNuovoFormularioMAB_TipoDoc').modal('show');
        },
        success: function (result) {
            //debugger;
            $("#divViewNuovoFormularioMAB_TipoDoc").empty();
            $("#divViewNuovoFormularioMAB_TipoDoc").html(result);
        },
        error: function (error) {
            //debugger;
            //Sblocca();
            var msg = error.responseText;
            ErroreElaborazioneAjax(msg);
        }
    });
}

function TabElencoFormulariMAB(idMab) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/TabFormulariMABInseriti";
    //var idTrasferimento = parseInt($('#hdIdTrasferimento').val());

    $.ajax({
        url: rotta,
        type: "POST", //Le info testuali saranno passate in POST
        data: { idMab: idMab },
        dataType: 'html',
        async: false,
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
            //$('#inviaDocModal').modal('show');
            Blocca();
        },
        success: function (result) {
            //debugger;
            $("#divTabElencoFormulariMAB_Var").empty();
            $("#divTabElencoFormulariMAB_Var").html(result);

        },
        error: function (error) {
            //debugger;
            Sblocca();
            var msg = error.responseText;
            ErroreElaborazioneAjax(msg);
        }
    });
}

function FiltraFormulariMAB(idMab,idAttivazione)
{
    //debugger;
    //var idAttivazione =  parseInt($("#idAttivazione").val());

    if (idAttivazione > 0) {

        var rotta = "/VariazioneMaggiorazioneAbitazione/FiltraTabFormulariMABInseriti";
        var idTrasferimento = parseInt($('#hdIdTrasferimento').val());

        $.ajax({
            url: rotta,
            type: "POST", //Le info testuali saranno passate in POST
            data: { idMab: idMab, idAttivazione: idAttivazione },
            dataType: 'html',
            async: false,
            beforeSend: function () {
                //debugger;
                VerificaAutenticazione();
                //Blocca();
            },
            success: function (result) {
                //debugger;
                $("#divTabElencoFormulariMAB_Var").empty();
                $("#divTabElencoFormulariMAB_Var").html(result);

            },
            error: function (error) {
                //debugger;
                //Sblocca();
                var msg = error.responseText;
                ErroreElaborazioneAjax(msg);
            }
        });
    }
    else
    {
        TabElencoFormulariMAB(idMab);
    }
}

function ConfermaAnnullaModificheMAB() {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/ConfermaAnnullaModificheMAB";
    var idMAB = parseInt($("#hiAnnullaModMAB").val());
    var idTrasferimento = parseInt($('#hdIdTrasferimento').val());

    $.ajax({
        url: rotta,
        type: "POST", //Le info testuali saranno passate in POST
        data: {
            idMAB: idMAB
            //idMaggiorazioniFamiliari: idMaggiorazioniFamiliari,
            //solaLettura: solaLettura,
            //check_nuovo_coniuge: check_nuovo_coniuge
        },
        dataType: 'json',
        async: false,
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
            Blocca();
        },
        success: function (result) {
            //debugger;
            if (result.errore === "") {
                $('#AnnullaModificheMABModal').modal('hide');
                AttivitaMAB_var(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
                
            } else {
                ErroreElaborazioneAjax(result.msg);
            }
            //$("#tabFamiliari").empty();
            //$("#tabFamiliari").html(result);

        },
        complete: function () {
            //PulsantiNotificaAttivaMagFam();
            Sblocca();
        },
        error: function (error) {
            //debugger;
            Sblocca();
            var msg = error.responseText;
            ErroreElaborazioneAjax(msg);
        }
    });

}

