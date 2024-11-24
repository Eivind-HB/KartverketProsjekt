// Toggle the edit form visibility
function toggleEditForm(caseNo) {
    $('#edit-form-' + caseNo).toggle();
}

function submitEditForm(event, caseNo) {
    event.preventDefault();
    var form = $('#edit-form-' + caseNo);
    $.ajax({
        type: form.attr('method'),
        url: form.attr('action'),
        data: form.serialize(),
        success: function (response) {
            $('#issueType-' + caseNo).text(response.newIssueTypeName);
            $('#status-' + caseNo).text(response.newStatusName);
            $('#description-' + caseNo).text(response.newDescription);
            toggleEditForm(caseNo);
        },
        error: function () {
            alert('Error saving changes.');
        }
    });
}