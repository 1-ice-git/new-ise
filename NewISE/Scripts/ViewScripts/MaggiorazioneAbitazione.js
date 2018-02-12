function GestionePulsantiNotificaAttivaAnnullaMAB() {
    //debugger;
    var rotta = "/Anticipi/GestionePulsantiAnticipi";
    var idPrimaSistemazione = parseInt('@idPrimaSistemazione');

    $.ajax({
        type: "POST",
        url: rotta,
        data: { idPrimaSistemazione: idPrimaSistemazione },
        dataType: 'html',
        beforeSend: function () {
            //debugger;
            VerificaAutenticazione();
        },
        success: function (result) {
            //debugger;
            $("#divAttivazioneAnticipi").empty();
            $("#divAttivazioneAnticipi").html(result);
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

function AttivitaMAB() {
    //debugger;
    var rotta = "/MaggiorazioneAbitazione/AttivitaMAB";
    var idMAB = parseInt(@idTrasferimento);

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