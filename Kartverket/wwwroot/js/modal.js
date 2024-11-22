// This script handles the display of an image in a modal when the modal is shown.
// It listens for the 'show.bs.modal' event, retrieves the case ID from the button that triggered the event,
// fetches the image source from the server using the case ID, and sets the source of the image in the modal.
var imageModal = document.getElementById('imageModal');
imageModal.addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    var caseId = button.getAttribute('data-case-id');
    fetch(`/Case/GetCaseImage?caseId=${caseId}`)
        .then(response => response.json())
        .then(data => {
            var modalImage = imageModal.querySelector('#modalImage');
            modalImage.src = data.imgSrc;
        });
});