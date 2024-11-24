// Toggle the edit form visibility
function toggleEditForm(caseNo) {
    const form = document.getElementById(`edit-form-${caseNo}`);
    const editButtons = document.getElementById(`edit-buttons-${caseNo}`);
    const isHidden = form.style.display === "none";

    form.style.display = isHidden ? "block" : "none";
    editButtons.style.display = isHidden ? "flex" : "none";
}