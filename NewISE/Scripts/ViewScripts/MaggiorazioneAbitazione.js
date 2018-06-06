function GestionePulsantiNotificaAttivaAnnullaMABPartenza(idTrasferimento) {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/GestionePulsantiMAB';
    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $('#divAttivazioneMAB').empty();
            $('#divAttivazioneMAB').html(result);
            //CalcolaAnticipo();
            //Sblocca();
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


function FormulariMAB(idTrasferimento) {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/FormulariMAB';
    var id_Trasferimento = parseInt(idTrasferimento);

    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idTrasferimento: id_Trasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $('#tabFormulariMAB').empty();
            $('#tabFormulariMAB').html(result);
            //Sblocca();
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


function AttivitaMAB(idTrasferimento) {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/AttivitaMAB';

    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $('#tabAttivitaMAB').empty();
            $('#tabAttivitaMAB').html(result);
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

function GestioneMAB(idTrasferimento) {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/GestioneMAB';

    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $('#divMaggiorazioneAbitazione').empty();
            $('#divMaggiorazioneAbitazione').html(result);
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


function UploadDocumentoMABModal(idTipoDocumento, idTrasferimento) {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/DocumentoMAB';
    $('#hiIdTipoDocumento').val(idTipoDocumento);

    $.ajax({
        url: rotta,
        type: 'POST', //Le info testuali saranno passate in POST
        data: { idTrasferimento: idTrasferimento, idTipoDocumento: idTipoDocumento },
        dataType: 'html',
        async: false,
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
            $('#inviaDocMABModal').modal('show');
            //Blocca();

        },
        success: function (result) {
            //debugger;
            $('#viewDocumentoMAB').empty();
            $('#viewDocumentoMAB').html(result);

        },
        error: function (error) {
            //debugger;
            //Sblocca();
            var msg = error.responseText;
            ErroreElaborazioneAjax(msg);
        }
    });

}

function CloseTimeModalMAB(idTrasferimento) {
    //debugger;
    $('#btUploadDocMAB').attr('disabled', 'disabled');
    $('#btUploadDocMAB').addClass('disabled');
    $('#btAnnullaDocMAB').attr('disabled', 'disabled');
    $('#btAnnullaDocMAB').addClass('disabled');
    setTimeout(CloseModalFileMAB(idTrasferimento), 2000);
}

function CloseModalFileMAB(idTrasferimento) {
    //debugger;
    $('#inviaDocMABModal').modal('hide');
    setTimeout(FormulariMAB(idTrasferimento), 1000);
}


function ValidazioneMAB(idTrasferimento) {
    //debugger;
    var ret = false;
    var c1 = false;
    var c2 = false;
    var c3 = false;

    var file = $('#file').val();

    if (idTrasferimento > 0) {


        if (file != null && file != undefined && file != '') {
            $('#divFile').removeClass('BordoRosso');
            c3 = true;
        }
        else {
            $('#divFile').addClass('BordoRosso');
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

function SalvaDocumentoMAB(idTrasferimento, idTipoDocumento) {

    //debugger;
    var datiForm = new FormData();
    var rotta = '/MaggiorazioneAbitazione/InserisciDocumentoMAB';

    //var idTrasferimento = parseInt($('#idTrasferimento').val());
    //var idTipoDocumento = parseInt('@idTipoDocumento');
    var file = $('#file')[0].files[0];

    if (ValidazioneMAB(idTrasferimento)) {

        datiForm.append('idTrasferimento', idTrasferimento);
        datiForm.append('idTipoDocumento', idTipoDocumento);
        datiForm.append('file', file);

        $.ajax({
            url: rotta,
            type: 'POST', //Le info testuali saranno passate in POST
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
                if (result.err != '' && result.err != undefined) {
                    MsgErroreJson(result.err);
                }
                else {
                    MsgErroreJson(result.msg);

                    CloseTimeModalMAB(idTrasferimento);

                    GestionePulsantiNotificaAttivaAnnullaMABPartenza(idTrasferimento);
                }

            },
            complete: function () {
                //$('#btUpload').removeAttr('disabled');

            },
            error: function (error) {
                //debugger;
                //Sblocca();
                $('#btUploadDocMAB').removeAttr('disabled');
                $('#btUploadDocMAB').removeClass('disabled');
                $('#btAnnullaDocMAB').removeAttr('disabled');
                $('#btAnnullaDocMAB').removeClass('disabled');
                var msg = error.responseText;
                MsgErroreJson(msg);
            }
        });
    }
    else {
        $('#btUploadDocMAB').removeAttr('disabled');
        $('#btUploadDocMAB').removeClass('disabled');
        $('#btAnnullaDocMAB').removeAttr('disabled');
        $('#btAnnullaDocMAB').removeClass('disabled');
    }

}

function MsgErroreJson(msg) {
    $('#spMsgError').html(msg);
}

function ConfermaNotificaRichiestaMAB() {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/ConfermaNotificaRichiestaMAB';
    var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
    var idAttivazioneMAB = parseInt($('#hi_idAttivazioneMAB').val());


    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idAttivazioneMAB: idAttivazioneMAB },
        dataType: 'json',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            if (result.err == '') {
                //InfoElaborazioneAjax(result.msg);
                AttivitaMAB(idTrasferimento);
                FormulariMAB(idTrasferimento);
                GestioneRinunciaMABPartenza(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMABPartenza(idTrasferimento);
                GestioneAttivitaTrasferimento();
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

function ConfermaAnnullaRichiestaMAB() {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/ConfermaAnnullaRichiestaMAB';
    var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
    var idAttivazioneMAB = parseInt($('#hi_idAttivazioneMAB').val());
    var testoAnnullaMAB = $('#FullDescription').val();

    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idAttivazioneMAB: idAttivazioneMAB, msg:testoAnnullaMAB },
        dataType: 'json',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            if (result.err == '') {
                //InfoElaborazioneAjax(result.msg);
                AttivitaMAB(idTrasferimento);
                FormulariMAB(idTrasferimento);
                GestioneRinunciaMABPartenza(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMABPartenza(idTrasferimento);
                GestioneAttivitaTrasferimento();
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

function ConfermaAttivaRichiestaMAB() {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/ConfermaAttivaRichiestaMAB';
    var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
    var idAttivazioneMAB = parseInt($('#hi_idAttivazioneMAB').val());
    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idAttivazioneMAB: idAttivazioneMAB },
        dataType: 'json',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
            //$('#DialogNewDoc').dialog('destroy');
            //$('#divEffettoLoadAutNoDoc').show('slow');
        },
        success: function (result) {
            //debugger;
            if (result.err == '') {
                //InfoElaborazioneAjax(result.msg);
                AttivitaMAB(idTrasferimento);
                FormulariMAB(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMABPartenza(idTrasferimento);
                GestioneAttivitaTrasferimento();
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


function NuovaMAB(idTrasferimento) {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/NuovaMAB';

    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $('#divMAB').empty();
            $('#divMAB').html(result);
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

function MessaggioAnnullaRichiestaMAB() {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/MessaggioAnnullaMAB';
    var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $('#viewAnnullaRichiestaMAB').empty();
            $('#viewAnnullaRichiestaMAB').html(result);
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

function GestioneRinunciaMABPartenza(idTrasferimento) {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/GestioneRinunciaMABPartenza';

    $.ajax({
        type: 'POST',
        url: rotta,
        data: { idTrasferimento: idTrasferimento },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $('#divRinunciaMABPartenza').empty();
            $('#divRinunciaMABPartenza').html(result);
            //Sblocca();
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

function AggiornaRinunciaMABPartenza(idMAB) {
    //debugger;
    var rotta = '/MaggiorazioneAbitazione/AggiornaRinunciaMABPartenza';

    $.ajax({
        url: rotta,
        type: 'POST', //Le info testuali saranno passate in POST
        data: { idMAB: idMAB },
        dataType: 'json',
        async: false,
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
            Blocca();

        },
        success: function (result) {
            //debugger;
            if (result.errore === '') {
                var idTrasferimento = parseInt($('#hi_idTrasferimento').val());
                AttivitaMAB(idTrasferimento);
                FormulariMAB(idTrasferimento);
                GestionePulsantiNotificaAttivaAnnullaMABPartenza(idTrasferimento);
            } else {
                ErroreElaborazioneAjax(result.msg);
            }
        },
        complete: function () {
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
