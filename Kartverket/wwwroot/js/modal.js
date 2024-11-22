var imageModal = document.getElementById('imageModal');
imageModal.addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    var imgSrc = button.getAttribute('data-img-src');
    var modalImage = imageModal.querySelector('#modalImage');
    modalImage.src = imgSrc;
});