function ValidateUserRegister() {
    var isValid = true;
    var term = $("#termCondCheck")[0].checked;
    var firstname = $('#firstnameTxt')[0].value;
    var email = $('#emailIdTxt')[0].value;
    var password = $('#passwordTxt')[0].value;
    var phone = $('#phoneTxt')[0].value;
    var lastname = $('#lastnameTxt')[0].value;
    var confirmpassword = $('#confirmpassTxt')[0].value;
    var zipcode = $('#zipcodeTxt')[0].value;
    var zipcodeRegex = /^[0-9-+ ]+$/;
    var phNoRegx = /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/;
    //var refCode = $('#ReferralCode')[0].value;
    //$('.errorRefCode')[0].innerHTML = "";

    if (firstname.length == 0) {
        $('.errorFirstname')[0].innerHTML = "Please enter first name.";
        isValid = false;
    }
    else {
        $('.errorFirstname')[0].innerHTML = "";
    }

    if (email.length == 0) {
        $('.errorEmailId')[0].innerHTML = "Please enter Email Id.";
        isValid = false;
    }
    else {
        $('.errorEmailId')[0].innerHTML = "";
    }

    if (password.length == 0) {
        $('.errorpassword')[0].innerHTML = "Please enter Password";
        isValid = false;
    }
    else if (password.length < 8) {
        $('.errorpassword')[0].innerHTML = "Minimum password length is 8 char/digits ";
        isValid = false;
    }
    else {
        $('.errorpassword')[0].innerHTML = "";
    }

    ;
    if (email.length > 0 || phone.length > 0) {
        $.ajax({
            type: "POST",
            async: false,
            data: JSON.stringify({ "emailid": email, "phone": phone }),
            contentType: 'application/json; charset=utf-8',
            url: "/UserRegister/CheckEmailNPhoneExist",
            success: function (response) {
                
                var resp = JSON.parse(response);
                if (resp.EmailExist == true) {
                    $('.errorEmailId')[0].innerHTML = "Email Id already Exists.";
                    isValid = false;
                }
                else {
                    $('.errorEmailId')[0].innerHTML = "";
                }
                if (resp.PhoneExist == true) {
                    $('.errorphone')[0].innerHTML = "Phone number already Exists.";
                    isValid = false;
                }
                else {
                    $('.errorphone')[0].innerHTML = "";
                }
            },
            error: function (response) { }
        });
    }

    if (lastname.length == 0) {
        $('.errorlastname')[0].innerHTML = "Please enter last name.";
        isValid = false;
    }
    else {
        $('.errorlastname')[0].innerHTML = "";
    }

    if (confirmpassword.length == 0) {
        $('.errorconfirm')[0].innerHTML = "Please enter Confirm password";
        isValid = false;
    }
    else if (password != confirmpassword) {
        $('.errorconfirm')[0].innerHTML = "Confirm password is incorrect";
        isValid = false;
    }
    else {
        $('.errorconfirm')[0].innerHTML = "";
    }

    if (zipcode.length > 0 && zipcode.length != 6) {
        $('.errorzip')[0].innerHTML = "Zipcode must be 6 digits.";
        isValid = false;
    }
    else {
        $('.errorzip')[0].innerHTML = "";
    }
    if (term == false) {
        isValid = false;
        $('.errorTerm')[0].innerHTML = "Please accept the terms and conditions.";
    }
    else {
        $('.errorTerm')[0].innerHTML = "";
    }
    //if (refCode.trim().length == 0) {
    //    localStorage.CouponCode = "";
    //}
    //if (refCode.length > 0 && isValid == true) {
        
    //    //    $('.errorRefCode')[0].innerHTML = "Please Enter Referral Code";
    //    //    isValid = false;
    //    //}
    //    //else {
    //    localStorage.CouponCode = refCode.trim();
    //    $('.errorRefCode')[0].innerHTML = "";

    //    if (localStorage.CouponCode.length == 8) {

    //        $.ajax({
    //            type: "POST",
    //            async: false,
    //            data: JSON.stringify({ "UserId": null, "isLoggedIn": "false", "EmailId": email, "PhoneNumber": phone }),
    //            contentType: 'application/json; charset=utf-8',
    //            url: "/UserRegister/CheckReferralCode",
    //            success: function (response) {
                    
    //                var resp = JSON.parse(response);
    //                if (resp.RefCodeFound == false) {
    //                    $('.errorRefCode')[0].innerHTML = "Sorry, we couldn't find the referral code. Please try valid code.";
    //                    isValid = false;
    //                }
    //                else if (resp.CodeExpires == true) {
    //                    $('.errorRefCode')[0].innerHTML = "Sorry, the code you are trying to redeem is already expired. Please try new code.";
    //                    isValid = false;
    //                }
    //                else if (resp.CodeShared == false) {
    //                    $('.errorRefCode')[0].innerHTML = "Sorry, the code you submitted is not shared with you. Please submit valid referral code.";
    //                    isValid = false;
    //                }
    //                else if (resp.AllowReferralCode == false) {
    //                    $('.errorRefCode')[0].innerHTML = "You can't activate your own referral code.";
    //                    isValid = false;
    //                }
    //                else if (resp.AlreadyUsed == true) {
    //                    $('.errorRefCode')[0].innerHTML = "You have already activated this code. Please try new code.";
    //                    isValid = false;
    //                }
    //                else if (resp.Success == false) {
    //                    $('.errorRefCode')[0].innerHTML = "Sorry, their is some problem at our side. Please try again.";
    //                    isValid = false;
    //                }
    //                else if (resp.NotAReceiver == true) {
    //                    $('.errorRefCode')[0].innerHTML = "Sorry, this coupon is not alloted to you. Please check the EmailId";
    //                    isValid = false;
    //                }
    //                else {
    //                    //isValid = true;
    //                    $('.errorRefCode')[0].innerHTML = "Congratulations! The reward amount of Rs. " + resp.RewardAmount + " has been added to your wallet. Please register with your email: " + email + " to get the reward amount in your wallet.";
    //                    //bootbox.dialog({
    //                    //    message: "Congratulations! The reward amount of Rs. " + resp.RewardAmount + " has been added to your wallet. Please register with your email: " + email + " to get the reward amount in your wallet.",
    //                    //    buttons: {
    //                    //        "success": {
    //                    //            "label": "Ok",
    //                    //            "className": "btn-sm btn-primary",
    //                    //            callback: function () {
    //                    //                //$('#finalWalletAmt')[0].innerHTML = "";
    //                    //                //$('#finalWalletAmt')[0].innerHTML = "Rs. " + resp.UpdatedWalletAmount;
    //                    //            }
    //                    //        },
    //                    //    }
    //                    //});
    //                }
    //            },
    //            error: function (response) { }
    //        });
    //    }
    //}
    if (isValid == false) {
        return false;
    }
    else {
        return true;
    }
}

function isEmail(emailid) {
    var emailRegex = "^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$";
    if (!emailid.match(emailRegex)) {
        return false;
    }
    else {
        return true;
    }
}


