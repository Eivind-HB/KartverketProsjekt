//Handles the display of an image in a modal when the modal is shown
//Listens for the show.bs.modal event, retrieves the image source from the button that triggered the event
//Sets the source of the image in the modal to the source retrieved from the button
var imageModal = document.getElementById('imageModal');
imageModal.addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    var imgSrc = button.getAttribute('data-img-src');
    var modalImage = imageModal.querySelector('#modalImage');
    modalImage.src = imgSrc;
});