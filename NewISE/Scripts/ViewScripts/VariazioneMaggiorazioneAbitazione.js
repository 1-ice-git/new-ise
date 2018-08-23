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

function FormulariMAB_var(idTrasferimento) {
    //debugger;
    var rotta = "/VariazioneMaggiorazioneAbitazione/FormulariMAB_var";
    var idTrasferimento = parseInt(idTrasferimento);

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
            $("#tabFormulariMAB_var").empty();
            $("#tabFormulariMAB_var").html(result);
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

function CloseTimeModalMAB_var(idTrasferimento) {
    //debugger;
    $("#btUploadDocMAB_var").attr("disabled", "disabled");
    $("#btUploadDocMAB_var").addClass("disabled");
    $("#btAnnullaDocMAB_var").attr("disabled", "disabled");
    $("#btAnnullaDocMAB_var").addClass("disabled");
    setTimeout(CloseModalFileMAB_var(idTrasferimento), 2000);
}

function CloseModalFileMAB_var(idTrasferimento) {
    //debugger;
    $('#inviaDocMABModal_var').modal('hide');
    setTimeout(FormulariMAB_var(idTrasferimento), 1000);
}

function ValidazioneMAB_var(idTrasferimento) {
    //debugger;
    var ret = false;
    var c1 = false;
    var c2 = false;
    var c3 = false;

    var file = $("#file").val();

    if (idTrasferimento > 0) {


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

function SalvaDocumentoMAB_var(idTrasferimento, idTipoDocumento) {

    //debugger;
    var datiForm = new FormData();
    var rotta = "/VariazioneMaggiorazioneAbitazione/InserisciDocumentoMAB_var";

    //var idTrasferimento = parseInt($("#idTrasferimento").val());
    //var idTipoDocumento = parseInt('@idTipoDocumento');
    var file = $("#file")[0].files[0];

    if (ValidazioneMAB_var(idTrasferimento)) {

        datiForm.append("idTrasferimento", idTrasferimento);
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

                    CloseTimeModalMAB_var(idTrasferimento);

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
        $("#btUploadDocMAB_var").removeAttr("disabled");
        $("#btUploadDocMAB_var").removeClass("disabled");
        $("#btAnnullaDocMAB_var").removeAttr("disabled");
        $("#btAnnullaDocMAB_var").removeClass("disabled");
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
                FormulariMAB_var(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
                GestioneAttivitaTrasferimento_var();
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
                FormulariMAB_var(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
                //GestioneAttivitaTrasferimento();
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
                FormulariMAB_var(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMAB_var(idTrasferimento);
                //GestioneAttivitaTrasferimento();
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
            $("#divMAB").empty();
            $("#divMAB").html(result);
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
