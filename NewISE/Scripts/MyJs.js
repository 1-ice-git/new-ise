function Blocca() {
    $.blockUI({ message: '<img src="../..//Immagini/39.gif" /><h1>Attendere prego...</h1>' });
}

function Sblocca() {
    $.unblockUI();
}

$(document).ajaxStart(function () {
    Blocca();
});

$(document).ajaxStop(function () {
    Sblocca();
});

