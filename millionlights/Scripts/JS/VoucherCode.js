

//function ValidateVouchers() {

$(document).ready(function () {
    $('#submit-contact').click(function () {
        var isValid = true;
        var partner = $('#ddlPartner')[0].value;
        var course = $('#ddlCourse')[0].value;
        var voucherType = $('#voucherTypeTxt')[0].value;
        var expDate = $('#expiryDateTxt')[0].value;
        var discount = $('#discountTxt')[0].value;
        var noOfAllowedCourses = $('#maxAllowedCoursesTxt')[0].value;
        var selectedCourses = $('ul.multiselect-container').find('li.active');
        var isAllCoursesSelected= $('ul.multiselect-container').find('li.multiselect-all').hasClass('active');

        if (partner.length == 0) {
            $('#errorPartnerName')[0].innerHTML = "Please select the partner.";
            isValid = false;
        }
        else {
            $('#errorPartnerName')[0].innerHTML = "";
        }
        if (course.length == 0) {
            $('#errorCourseName')[0].innerHTML = "Please select the course(s).";
            isValid = false;
        }
        else if (noOfAllowedCourses!="") {
            if (selectedCourses.length > 0) {
                if ((isAllCoursesSelected == true) && ((selectedCourses.length - 1) > noOfAllowedCourses)) {
                    $('#errorCourseName')[0].innerHTML = "Number of allowed course(s) for this voucher is only " + noOfAllowedCourses;
                    isValid = false;
                }
                else if ((isAllCoursesSelected == false) && ((selectedCourses.length) > noOfAllowedCourses)) {
                    $('#errorCourseName')[0].innerHTML = "Number of allowed course(s) for this voucher is only " + noOfAllowedCourses;
                    isValid = false;
                }
                else {
                    $('#errorCourseName')[0].innerHTML = "";
                }
            }
        }
        else {
            $('#errorCourseName')[0].innerHTML = "";
        }

        if (voucherType.length == 0) {
            $('#errorVoucherType')[0].innerHTML = "Please enter voucher type.";
            isValid = false;
        }
        else {
            $('#errorVoucherType')[0].innerHTML = "";
        }
        if (expDate.length == 0) {
            $('#errorExpiryDate')[0].innerHTML = "Please enter expiry date.";
            isValid = false;
        }
        else {
            $('#errorExpiryDate')[0].innerHTML = "";
        }
        if (discount.length == 0) {
            $('#errorDiscount')[0].innerHTML = "Please enter discount.";
            isValid = false;
        }
        else {
            $('#errorDiscount')[0].innerHTML = "";
        }
        if (noOfAllowedCourses.length == 0) {
            $('#errorMaxAllowedCourses')[0].innerHTML = "Please enter allowed courses value.";
            isValid = false;
        }
        else {
            $('#errorMaxAllowedCourses')[0].innerHTML = "";
        }

        if (isValid == false) {
            return false;
        }
        else {
            return true;
        }
    });


    $('#cancel-button').click(function () {
        $("#VoucherList").show();
        $("#create-vouchercode").hide();
    });
})