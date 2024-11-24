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

function assignCaseworker(caseNo) {
    var caseworkerID = prompt("Enter Caseworker ID:");
    if (caseworkerID) {
        $.ajax({
            type: 'POST',
            url: '/Case/AssignCaseworker',
            data: {
                caseNo: caseNo,
                caseworkerID: caseworkerID,
                paidHours: 0
            },
            success: function (response) {
                alert('Caseworker assigned successfully.');
                // Update the UI with the new case assignment
                var caseworkerCasesList = $('#caseworker-cases-' + caseworkerID);
                if (caseworkerCasesList.length) {
                    caseworkerCasesList.append('<li>' + caseNo + '</li>');
                } else {
                    // If the list doesn't exist, create it
                    $('#caseworker-' + caseworkerID).append('<ul id="caseworker-cases-' + caseworkerID + '"><li>' + caseNo + '</li></ul>');
                }
                // Refresh the page to update the UI
                location.reload();
            },
            error: function () {
                alert('Error assigning caseworker.');
            }
        });
    }
}