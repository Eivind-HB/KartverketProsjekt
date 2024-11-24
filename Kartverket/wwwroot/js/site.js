// Function to highlight the current pages navitem in a light green color.
// PageIdentifier of the page you want to light up. var(--light-green) from site.css
function highlightNavItem(pageIdentifier, backgroundColor = 'var(--light-green)') {
    const navItem = document.querySelector(`.nav-item a[href*="${pageIdentifier}"]`);
    if (navItem) {
        navItem.parentElement.style.backgroundColor = backgroundColor;
    }
}