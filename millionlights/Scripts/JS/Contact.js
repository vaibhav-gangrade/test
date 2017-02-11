function ValidateContacts() {
    var isValid = true;
    var Name = $('#Nametxt')[0].value;
    var LName = $('#lNameTxt')[0].value;
    var LastName = $('#LastNametxt')[0].value;
    var Email = $('#Emailtxt')[0].value;
    var PhoneNo = $('#PhoneNotxt')[0].value;
    var Message = $('#Messagetxt')[0].value;

    if (LName.length == "") {
       $('#erroLname')[0].innerHtml = "Please Enter Your Name";
        isValid = false;
    }
else
{
       $('#erroLname')[0].innerHtml = "";
       $('#erroLname')[0].innerHtml = "Please Enter Your Name";
}
    if (Name.length == 0) {
        $('#errorName')[0].innerHtml = "Please Enter Your Name";
        $('#errorNamelbl')[0].innerHtml = "Please Enter Your Name";
        isValid = false;
    }
    else
    {
        $('#errorName')[0].innerHtml = "";
        $('#errorName')[0].innerHtml = "Please Enter Your Name";
    }
    if (LastName.length == 0) {
        $('#errorLastName')[0].innerHtml = "Please Enter Last Name";
        isvalid = false;
    }
    else
    {
        $('#errorLastName')[0].innerHtml = "";
    }
    if (Email.length == 0) {
        $('#errorEmail')[0].innerHtml = "Please Enter Email Id";
        isvalid = false;
    }
    else
    {
        $('#errorEmail')[0].innerHtml = "";
    }
    if (PhoneNo.length == 0) {
        $('#errorPhoneNo')[0].innerHtml = "Please Enter Phone Number";
        isvalid = false;
    }
    else
    {
        $('#errorPhoneNo')[0].innerHtml = "";
    }
    if (Message.length == 0) {
        $('#errorMessage')[0].innerHtml = "Please Enter Message";
        isValid = false;
    }
    else
    {
        $('#errorMessage')[0].innerHtml = "";
    }
    if (Email.length > 0)
    {
        var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if(!filter.test(Email.value))
        {
            $('#errorEmail')[0].innerHtml = "Please Enter Valid Email Id";
            isValid = false;
        }
    }
    else
    {
        $('#errorEmail')[0].innerHtml = "";
    }
    if (PhoneNo.length > 0) {
        var checkphoneno = /^\d{10}$/;
        if (!checkphoneno.test(PhoneNo)) {
            $('#errorPhoneNo')[0].innerHtml = "Please Enter Valid Phone Number";
            isValid = false;
        }
    }

    if (isValid == false) {
        return false;
    }
    else {
        return true;
    }

}

//function ValidateContactUs()
//{
//    
//    isValid = true;
//    var LName = $('#lNameTxt')[0].value.trim();

//    if (LName.length == "") {
//        $('#erroLname')[0].innerHtml = "Please Enter Your Name";
//        $('#erroLname2')[0].innerHtml = "Please Enter Your2 Name";
//        isValid = false;
//    }
//    else {
//        $('#erroLname')[0].innerHtml = "";
       
//    }
//    if (isValid == false) {
//        return false;
//    }
//    else {
//        return true;
//    }
//}