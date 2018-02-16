function GestionePulsantiNotificaAttivaAnnullaMAB(idTrasferimento) {
    //debugger;
    var rotta = "/MaggiorazioneAbitazione/GestionePulsantiMAB";
   

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
            $("#divAttivazioneMAB").empty();
            $("#divAttivazioneMAB").html(result);
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


function FormulariMAB(idTrasferimento) {
    //debugger;
    var rotta = "/MaggiorazioneAbitazione/FormulariMAB";
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
            $("#divFormulariMAB").empty();
            $("#divFormulariMAB").html(result);
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


function AttivitaMAB(idTrasferimento) {
    //debugger;
    var rotta = "/MaggiorazioneAbitazione/AttivitaMAB";

    $.ajax({
        type: "POST",
        url: rotta,
        data: { idTrasferimento: idTrasferimento},
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            //VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#divAttivitaMAB").empty();
            $("#divAttivitaMAB").html(result);
            //GestionePulsantiNotificaAttivaAnnullaMAB();

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