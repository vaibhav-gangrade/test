function ValidateCATC() {
    var isValid = true;
    var prodCode = $('#prodCodeTxt')[0].value;
    var fees = $('#feesTxt')[0].value;
    var discount = $('#discountTxt')[0].value;
    var NOfAllowedCourses = $('#noOfAllowedCoursesTxt')[0].value;
    

    if (prodCode.length == 0) {
        $('.errorProdCode')[0].innerHTML = "Please enter product code.";
        isValid = false;
    }
    else {
        $('.errorProdCode')[0].innerHTML = "";
    }
    if (fees.length == 0) {
        $('.errorFees')[0].innerHTML = "Please enter fees.";
        isValid = false;
    }
    else {
        $('.errorFees')[0].innerHTML = "";
    }

    if (discount == 0) {
        $('.errorDiscount')[0].innerHTML = "Please enter discount.";
        isValid = false;
    }
    else {
        $('.errorDiscount')[0].innerHTML = "";
    }
    if (NOfAllowedCourses.length == 0) {
        $('.errorAllowedCourses')[0].innerHTML = "Please enter allowed courses value.";
        isValid = false;
    }
    else {
        $('.errorAllowedCourses')[0].innerHTML = "";
    }
   
    if (isValid == false) {
        return false;
    }
    else {
        return true;
    }
}