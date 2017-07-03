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

$(function () {
    if (typeof Array.prototype.contiene !== 'function') {
        Array.prototype.contiene = function (valore) {
            for (i in this) {
                if (this[i] == valore) {
                    return true;
                }
            }
            return false;
        }
    };

    $('.btn').button();

});