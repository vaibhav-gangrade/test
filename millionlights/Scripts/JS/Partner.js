
$(document).ready(function () {
    //function ValidateCreatePartner() {      
    debugger
    $('#submit-partner').click(function () {
        debugger
        //    var PartnerName = $('#PartnerNameTxt')[0].value;
        //    var ContactPersonName = $('#ContactPersonTxt')[0].value;
        //    var Email = $('#EmailTxt')[0].value;
        //    var PhoneNo = $('#PhoneNumberTxt')[0].value;
        //    var Address = $('#AddressTxt')[0].value;
        //    var imglink = $('#uploadID')[0].value;
        //    var Country = $('#CountryTxt')[0].value;
        //    var State = $('#StateTxt')[0].value;
        //    var city = $('#CityTxt')[0].value;
        //    var PartnerUrl = $('#PartnerUrlTxt')[0].value;
        //    var PartnerType = $('#ddlPartnerType')[0].value;
        //    var phoneno = /^\d{10}$/;
        //    var isValid = true;

        //    if (PartnerName.length == 0) {
        //        $('.errorPartnerName')[0].innerHTML = "Please Enter Partner Name.";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorPartnerName')[0].innerHTML = "";
        //    }

        //    if (ContactPersonName.length == 0) {
        //        $('.errorContactPersonName')[0].innerHTML = "Please Enter Contact Person Name.";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorContactPersonName')[0].innerHTML = "";
        //    }
       

        //    if (PhoneNo.length == 0) {
        //        $('.errorPhoneNo')[0].innerHTML = "Please Enter Phone Number.";
        //        isValid = false;
        //    }
        //    else if (!PhoneNo.match(phoneno)) {
        //        $('.errorPhoneNo')[0].innerHTML = "Please Enter Valid Phone Number";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorPhoneNo')[0].innerHTML = "";
        //    }
        //    if (Address.length == 0) {
        //        $('.errorAddress')[0].innerHTML = "Please Enter Address.";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorAddress')[0].innerHTML = "";
        //    }
       
        //    if (imglink.length == 0) {
        //        $('.errorImglink')[0].innerHTML = "Please Select Image to Upload.";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorImglink')[0].innerHTML = "";
        //    }
        //    if (PartnerUrl.length == 0)
        //    {
        //        $('.errorPartnerUrl')[0].innerHTML = "Please Enter Partner URL";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorPartnerUrl')[0].innerHTML = "";
        //    }
        //    if (Country.length == 0) {
        //        $('.errorCountry')[0].innerHTML = "Please Enter Country";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorCountry')[0].innerHTML = "";
        //    }
        //    if (State.length == 0) {
        //        $('.errorState')[0].innerHTML = "Please Enter State";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorState')[0].innerHTML = "";
        //    }
        //    if (city.length == 0) {
        //        $('.errorCity')[0].innerHTML = "Please Enter City";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorCity')[0].innerHTML = "";
        //    }
        //    if (PartnerType.length == 0) {
        //        $('.errorPartnerTypeName')[0].innerHTML = "Please Select Partner Type";
        //        isValid = false;
        //    }
        //    else {
        //        $('.errorPartnerTypeName')[0].innerHTML = "";
        //    }
        //    return isValid;
        //});

  
    });

    function validateEditPartners()
    {
        debugger
        //   var PartnerName = $('#PartnerNameTxt')[0].value;
        //   var ContactPersonName = $('#ContactPersonTxt')[0].value;
        ////   var Email = $('#EmailTxt')[0].value;
        //   var PhoneNo = $('#PhoneNumberTxt')[0].value;
        //   var Address = $('#AddressTxt')[0].value;
        
        //   var phoneno = /^\d{10}$/;
        //   var isValid = true;

        //   var ViewPartnerJson = '@Html.Raw(Json.Encode(partnerDetails))';
        //   var ViewPartner = JSON.parse(ViewPartnerJson);

        //   if (Email.length == 0) {
        //       $('.errorEmail')[0].innerHTML = "Please Enter Email Id.";
        //       isValid = false;
        //   }
        //   else if (isEmail(Email) == false) {
        //       $('.errorEmail')[0].innerHTML = "Please enter valid email id.";
        //       isValid = false;
        //   }
        //   else if (Email.length > 0) {
        //       for (var i = 0; i < ViewPartner.length; i++) {
        //           if (ViewPartner[i].Email == Email) {
        //               $('.errorEmail')[0].innerHTML = "Email Id already Exist.";
        //               isValid = false;
        //           }
        //       }
        //   }
        //   else {
        //       $('.errorEmail')[0].innerHTML = "";
        //   }
        //   if ($('.errorImglink')[0].innerHTML != null) {
        //       isValid = false;
        //   }


        //   if (PartnerName.length == 0) {
        //       $('.errorPartnerName')[0].innerHTML = "Please Enter Partner Name.";
        //       isValid = false;
        //   }
        //   else {
        //       $('.errorPartnerName')[0].innerHTML = "";
        //   }

        //   if (ContactPersonName.length == 0) {
        //       $('.errorContactPersonName')[0].innerHTML = "Please Enter Contact Person Name.";
        //       isValid = false;
        //   }
        //   else {
        //       $('.errorContactPersonName')[0].innerHTML = "";
        //   }
        //   //if (Email.length == 0) {
        //   //    $('.errorEmail')[0].innerHTML = "Please Enter Email Id.";
        //   //    isValid = false;
        //   //}
        //   //else if (isEmail(Email) == false) {
        //   //    $('.errorEmail')[0].innerHTML = "Please enter valid email id.";
        //   //    isValid = false;
        //   //}
        //   //else {
        //   //    $('.errorEmail')[0].innerHTML = "";
        //   //}

        //   if (PhoneNo.length == 0) {
        //       $('.errorPhoneNo')[0].innerHTML = "Please Enter Phone Number.";
        //       isValid = false;
        //   }
        //   else if (!PhoneNo.match(phoneno)) {
        //       $('.errorPhoneNo')[0].innerHTML = "Please Enter Valid Phone Number";
        //       isValid = false;
        //   }
        //   else {
        //       $('.errorPhoneNo')[0].innerHTML = "";
        //   }
        //   if (Address.length == 0) {
        //       $('.errorAddress')[0].innerHTML = "Please Enter Address.";
        //       isValid = false;
        //   }
        //   else {
        //       $('.errorAddress')[0].innerHTML = "";
        //   }
        //   if ($('.errorImglink')[0].innerHTML != null) {
        //       isValid = false;
        //   }
        //   return isValid;
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

    function readImage(file) {
        debugger
        var reader = new FileReader();
        var image = new Image();

        reader.readAsDataURL(file);
        reader.onload = function (_file) {
            // url.createObjectURL(file);
            image.onload = function () {
                debugger
                var w = this.width,
                    h = this.height,
                    t = file.type                          // ext only: // file.type.split('/')[1],
                ValidateFile(w, h);
            };
            image.src = _file.target.result;
            image.onerror = function () {
                alert('Invalid file type: ' + file.type);
            };
        };

    }
    $("#uploadID").change(function (e) {
        debugger
        if (this.disabled) return alert('File upload not supported!');
        var F = this.files;
        if (F && F[0]) for (var i = 0; i < F.length; i++) readImage(F[i]);
    });
    function ValidateFile(w, h) {
        debugger
        var isValid = true;
        var fuData = document.getElementById('uploadID');
        var FileUploadPath = fuData.value;
        //To check if user upload any file
        if (FileUploadPath == '') {
            alert("Please upload an image");

        } else {
            var Extension = FileUploadPath.substring(
                    FileUploadPath.lastIndexOf('.') + 1).toLowerCase();
            //The file uploaded is an image

            if (Extension == "gif" || Extension == "png" || Extension == "bmp"
                                || Extension == "jpeg" || Extension == "jpg") {
                isValid = true;
            }
            else {
                $('.errorImglink')[0].innerHTML = "Please Enter File with png,bmp,gif,jpeg,jpg only.";
                isValid = false;
            }
            if (w == 178 && h == 100) {
                $('.errorImglink')[0].innerHTML = " ";
                isValid = true;
            }
            else {
                $('.errorImglink')[0].innerHTML = "Please Enter Image 178 X 100 Dimensions Only.";
                isValid = false;
            }
        }
        return isValid;
    }

