//function Blocca() {
//    $.blockUI({ message: '<img src="../..//Immagini/39.gif" /><h1>Attendere prego...</h1>' });
//}

function Blocca(msg) {
    //debugger;
    if (typeof msg === "undefined" || msg == "") {
        msg = "Attendere prego...";
    }
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        },
        message: '<h1><img src="../../Immagini/2.GIF" alt="" /></h1>'
    });
    //<br /><h1>' + msg + '</h1>
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

