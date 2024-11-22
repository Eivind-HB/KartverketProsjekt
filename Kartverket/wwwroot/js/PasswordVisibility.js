//This script handles the visibility of a password when the user clicks the checkbox.
//It listens for the 'change' event on the checkbox, and changes the type of all password inputs on the page.
//It also changes the text of the label next to the checkbox.
//It selects the checkbox and all password inputs on the page.
    document.addEventListener('DOMContentLoaded', function () {
            var toggle = document.getElementById('togglePassword');
    var passwordInputs = document.querySelectorAll('.password-input');

    toggle.addEventListener('change', function() {
                var inputType = this.checked ? 'text' : 'password';
    passwordInputs.forEach(function(input) {
        input.type = inputType;
                });

    var label = this.nextElementSibling;
    label.textContent = this.checked ? 'Skjul passord' : 'Vis passord';
            });
        });