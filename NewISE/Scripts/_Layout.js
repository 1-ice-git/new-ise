﻿function number_format(number, decimals, dec_point, thousands_sep) {
    // Strip all characters but numerical ones.
    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
        sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec);
            return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}

function blink(selector) {
    $(selector).fadeOut('slow', function () {
        $(this).fadeIn('slow', function () {
            blink(this);
        });
    });
}

function is_numeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

var autoNumericOptionsEuro = {
    digitGroupSeparator: '.',
    decimalCharacter: ',',
    decimalCharacterAlternative: '.',
    showOnlyNumbersOnFocus: true,
    //currencySymbol: '\u202f€',
    //currencySymbolPlacement: AutoNumeric.options.currencySymbolPlacement.suffix,
    //roundingMethod: AutoNumeric.options.roundingMethod.halfUpSymmetric,
};


//var autoNumericOptionsEuroDueDec = {
//    digitGroupSeparator: '.',
//    decimalCharacter: ',',
//    decimalCharacterAlternative: '.',
//    eDec: 2,
//    decimalPlacesShownOnBlur: 2
//    //currencySymbol: '\u202f€',
//    //currencySymbolPlacement: AutoNumeric.options.currencySymbolPlacement.suffix,
//    //roundingMethod: AutoNumeric.options.roundingMethod.halfUpSymmetric,
//};

var autoNumericOptionsEuroTreDec = {
    digitGroupSeparator: '.',
    decimalCharacter: ',',
    decimalCharacterAlternative: '.',
    eDec: 3,
    decimalPlacesShownOnBlur: 3,
    currencySymbol: '\u202f%',
    currencySymbolPlacement: AutoNumeric.options.currencySymbolPlacement.suffix,
    roundingMethod: AutoNumeric.options.roundingMethod.halfUpSymmetric,
    minimumValue: 0,
    maximumValue: 100,
    showOnlyNumbersOnFocus: true
};

var autoNumericOptionsEuroOttoDec = {
    digitGroupSeparator: '.',
    decimalCharacter: ',',
    decimalCharacterAlternative: '.',
    eDec: 8,
    decimalPlacesShownOnBlur: 8
    //currencySymbol: '\u202f€',
    //currencySymbolPlacement: AutoNumeric.options.currencySymbolPlacement.suffix,
    //roundingMethod: AutoNumeric.options.roundingMethod.halfUpSymmetric,
};
