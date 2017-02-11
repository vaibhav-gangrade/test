function ValidateCATC() {
    var isValid = true;
    var custId = $('#custIdTxt')[0].value;
    var custName = $('#custNameTxt')[0].value;
    var custTypeDesc = $('#custTypeDescTxt')[0].value;
    var accStatus = $('#accStatusTxt')[0].value;
    var sector = $('#sectorTxt')[0].value;
    var countryName = $('#countryNameTxt')[0].value;
    var city = $('#cityTxt')[0].value;
    var zip = $('#zipTxt')[0].value;
    var addLine1 = $('#addLine1Txt')[0].value;
    var addLine2 = $('#addLine2Txt')[0].value;
    var webAddress = $('#webAddressTxt')[0].value;
    var phone = $('#phoneTxt')[0].value;
    var phoneno = /^\d{10}$/;
    var zipcode = /^\d{5}$/;

    if (custId.length == 0) {
        $('.errorCustId')[0].innerHTML = "Please enter customer ID.";
        isValid = false;
    }
    else {
        $('.errorCustId')[0].innerHTML = "";
    }
    if (custName.length == 0) {
        $('.errorCustName')[0].innerHTML = "Please enter customer name.";
        isValid = false;
    }
    else {
        $('.errorCustName')[0].innerHTML = "";
    }

    if (custTypeDesc == 0) {
        $('.errorCustType')[0].innerHTML = "Please enter customer type.";
        isValid = false;
    }
    else {
        $('.errorCustType')[0].innerHTML = "";
    }
    if (accStatus.length == 0) {
        $('.errorAccStatus')[0].innerHTML = "Please enter account status.";
        isValid = false;
    }
    else {
        $('.errorAccStatus')[0].innerHTML = "";
    }
    if (sector.length == 0) {
        $('.errorSector')[0].innerHTML = "Please enter sector.";
        isValid = false;
    }
    else {
        $('.errorSector')[0].innerHTML = "";
    }
    if (countryName.length == 0) {
        $('.errorCountryName')[0].innerHTML = "Please enter country name.";
        isValid = false;
    }
    else {
        $('.errorCountryName')[0].innerHTML = "";
    }
    if (city.length == 0) {
        $('.errorCity')[0].innerHTML = "Please enter city.";
        isValid = false;
    }
    else {
        $('.errorCity')[0].innerHTML = "";
    }
    //if (zip.length == 0) {
    //    $('.errorZipCode')[0].innerHTML = "Please enter zip code.";
    //    isValid = false;
    //}
    //else if (!zip.match(zipcode)) {
    //    $('.errorZipCode')[0].innerHTML = "Please Enter Valid Zip Code ( Only numbers & 5 digits are allowed.)";
    //    isValid = false;
    //}
    if (zip.length >0 && zip.length != 6) {
        $('.errorZipCode')[0].innerHTML = "Zipcode must be 6 digits.";
        isValid = false;
    }
    else {
        $('.errorZipCode')[0].innerHTML = "";
    }
    if (addLine1.length == 0) {
        $('.errorAddressLine1')[0].innerHTML = "Please enter address line1.";
        isValid = false;
    }
    else {
        $('.errorAddressLine1')[0].innerHTML = "";
    }
    if (addLine2.length == 0) {
        $('.errorAddressLine2')[0].innerHTML = "Please enter address line2.";
        isValid = false;
    }
    else {
        $('.errorAddressLine2')[0].innerHTML = "";
    }
    if (webAddress.length == 0) {
        $('.errorWebAddress')[0].innerHTML = "Please enter web address.";
        isValid = false;
    }
    else if (isWebAddress(webAddress) == false) {
        $('.errorWebAddress')[0].innerHTML = "Please enter valid web address.";
        isValid = false;
    }
    else {
        $('.errorWebAddress')[0].innerHTML = "";
    }
    if (phone.length == 0) {
        $('.errorPhone')[0].innerHTML = "Please enter phone.";
        isValid = false;
    }
    else if (!phone.match(phoneno)) {
        $('.errorPhone')[0].innerHTML = "Please Enter Valid Phone Number";
        isValid = false;
    }
    else {
        $('.errorPhone')[0].innerHTML = "";
    }
    if (isValid == false) {
        return false;
    }
    else {
        return true;
    }
}

function isWebAddress(webAddress) {
    var webRegex = "(http(s)?://)?([\w-]+\.)+[\w-]+[.com]+(/[/?%&=]*)?";// "/^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/|www\.)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/";
    if (!webAddress.match(webRegex)) {
        return false;
    }
    else {
        return true;
    }
}
