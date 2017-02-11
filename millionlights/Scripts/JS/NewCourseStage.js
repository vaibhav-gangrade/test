$(function () {
    
    $('#btnAdd').click(function () {
        
        
        var num     = $('.clonedInput').length, // how many "duplicatable" input fields we currently have
            newNum  = new Number(num + 1),      // the numeric ID of the new input field being added
            newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
    // manipulate the name/id values of the input inside the new element
        // H2 - section
        newElem.find('.heading-reference').attr('id', 'Stage' + newNum + '_reference').attr('name', 'Stage' + newNum + '_reference').html('Stage #' + newNum);
 
        // Title - select
        newElem.find('#stageNumberlbl').attr('for', 'StageNo' + newNum + '_label');
        newElem.find('.chapternumberTxt').attr('id', 'StageNo' + newNum + '_input').attr('name', 'StageNo' + newNum + '_input').val('');
 
        // First name - text
        newElem.find('#stageNamelbl').attr('for', 'StageName' + newNum + '_label');
        newElem.find('.chapternameTxt').attr('id', 'StageName' + newNum + '_input').attr('name', 'StageName' + newNum + '_input').val('');
 
        // Last name - text
        newElem.find('#stageDesclbl').attr('for', 'StageDesc' + newNum + '_label');
        newElem.find('.chapterDescriptionTxt').attr('id', 'StageDesc' + newNum + '_input').attr('name', 'StageDesc' + newNum + '_input').val('');
 
        
 
    // insert the new element after the last "duplicatable" input field
        $('#entry' + num).after(newElem);
        $('#Stage' + newNum + '_input').focus();
 
    // enable the "remove" button
        $('#btnDel').attr('disabled', false);
 
    // right now you can only add 5 sections. change '5' below to the max number of times the form can be duplicated
        if (newNum == 6)
            $('#btnAdd').attr('disabled', true).prop('value', "You've reached the limit");
        assignNullValue();

    });
 
    $('#btnDel').click(function () {
    // confirmation
        if (confirm("Are you sure you wish to remove this section? This cannot be undone."))
            {
                var num = $('.clonedInput').length;
                // how many "duplicatable" input fields we currently have
                $('#entry' + num).slideUp('slow', function () {$(this).remove(); 
                // if only one element remains, disable the "remove" button
                    if (num -1 === 1)
                $('#btnDel').attr('disabled', true);
                // enable the "add" button
                $('#btnAdd').attr('disabled', false).prop('value', "add section");});
            }
        return false;
        $('#btnAdd').attr('disabled', false);
    });
    var stages=$('.clonedInput').length;
    if (stages == 1) {
        $('#btnDel').attr('disabled', true);
    }
    else {
        $('#btnDel').attr('enabled', true);
    }
});

function assignNullValue() {
    var cloneList = $('#validation-form5').find('div.clonedInput');
    var lastCloneElement = $(cloneList[cloneList.length - 1]);
    $(lastCloneElement).find("#contentLengthHidden")[0].value = "";
}