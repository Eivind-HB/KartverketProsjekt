// Function to initialize accordions in views
// containerSelector: A CSS selector for the container holding the accordion elements
// map (optional): if a map is initialized in the container it makes sure it fits with Invalidate Size
function initializeAccordion(containerSelector, map = null) {
    // Select all accordion headers within the specified container
    document.querySelectorAll(`${containerSelector} .accordion-header`).forEach(button => {
        button.addEventListener('click', () => {
            const accordionContent = button.nextElementSibling;

            // Toggle the 'active' class on the button
            button.classList.toggle('active');

            // Toggle display style of the accordion content
            if (accordionContent.style.display === 'block') {
                accordionContent.style.display = 'none';
            } else {
                accordionContent.style.display = 'block';
            }

            // If a map object is provided, invalidate its size
            if (map) {
                map.invalidateSize();
            }
        });
    });
}