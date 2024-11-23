document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('caseSearchForm');
    const searchButton = document.getElementById('caseSearchButton');
    const filterButton = document.getElementById('caseFilterButton');
    const filterForm = document.getElementById('caseFilterForm');
    const userSearchButton = document.getElementById('userSearchButton');
    const userSearchForm = document.getElementById('userSearchForm');
    const caseworkerSearchButton = document.getElementById('caseworkerSearchButton');
    const caseworkerSearchForm = document.getElementById('caseworkerSearchForm');

    searchButton.addEventListener('click', filterCases);
    filterButton.addEventListener('click', filterCases);
    searchForm.addEventListener('submit', function (event) {
        event.preventDefault();
        filterCases();
    });
    filterForm.addEventListener('submit', function (event) {
        event.preventDefault();
        filterCases();
    });
    userSearchButton.addEventListener('click', filterUsers);
    userSearchForm.addEventListener('submit', function (event) {
        event.preventDefault();
        filterUsers();
    });
    caseworkerSearchButton.addEventListener('click', filterCaseworkers);
    caseworkerSearchForm.addEventListener('submit', function (event) {
        event.preventDefault();
        filterCaseworkers();
    });
});
function filterCases() {
    const searchTerm = document.getElementById('caseSearchInput').value.toLowerCase();
    const kommuneNameTerm = document.getElementById('kommuneSearchInput').value.toLowerCase();
    const issueTypeTerm = document.getElementById('issueTypeSearchInput').options[document.getElementById('issueTypeSearchInput').selectedIndex].text.toLowerCase().trim();
    const startDate = document.getElementById('startDateInput').value;
    const endDate = document.getElementById('endDateInput').value;
    const cases = document.querySelectorAll('#casesAccordion .accordion-collapse');

    cases.forEach(caseElement => {
        const caseNo = caseElement.id.split('-')[1];
        const caseButton = caseElement.previousElementSibling;
        const caseDetails = caseElement.querySelector('.issue-location-details');
        const caseKommuneName = caseDetails.querySelector('table:nth-of-type(2) tbody tr td:nth-child(1)').innerText.toLowerCase().trim();
        const caseIssueTypeName = caseDetails.querySelector('table:nth-of-type(1) tbody tr td:nth-child(2)').innerText.toLowerCase().trim();
        const caseDateText = caseDetails.querySelector('table:nth-of-type(1) tbody tr td:nth-child(3)').innerText.trim();

        let showCase = true;

        if (searchTerm && caseNo.toLowerCase() !== searchTerm) {
            showCase = false;
        }

        if (kommuneNameTerm && !caseKommuneName.includes(kommuneNameTerm)) {
            showCase = false;
        }

        if (issueTypeTerm && issueTypeTerm !== "ingen type valgt" && !caseIssueTypeName.includes(issueTypeTerm)) {
            showCase = false;
        }

        // Convert dates to comparable format (yyyy-mm-dd)
        const [caseDay, caseMonth, caseYear] = caseDateText.split('.');
        const caseDateComparable = `${caseYear}-${caseMonth}-${caseDay}`;

        if (startDate && caseDateComparable < startDate) {
            showCase = false;
        }

        if (endDate && caseDateComparable > endDate) {
            console.log(`Case ${caseNo} is filtered out because caseDate ${caseDateText} is after endDate ${endDate}`);
            showCase = false;
        }

        if (showCase) {
            caseButton.style.display = 'block'; // Show the accordion button
            caseElement.classList.remove('show'); // Ensure the content is hidden
            caseButton.classList.add('collapsed'); // Ensure the button is collapsed
        } else {
            caseButton.style.display = 'none'; // Hide the accordion button
            caseElement.classList.remove('show'); // Ensure the content is hidden
            caseButton.classList.add('collapsed'); // Ensure the button is collapsed
        }
    });
}

function filterUsers() {
    const userIDTerm = document.getElementById('userIDSearchInput').value.toLowerCase();
    const userNameTerm = document.getElementById('userNameSearchInput').value.toLowerCase();
    const userCasesTerm = document.getElementById('userCasesSearchInput').value.toLowerCase();
    const users = document.querySelectorAll('#usersAccordion .accordion-collapse');

    users.forEach(userElement => {
        const userID = userElement.id.split('-')[1];
        const userButton = userElement.previousElementSibling;
        const userDetails = userElement.querySelector('.issue-location-details');
        const userName = userDetails.querySelector('table:nth-of-type(1) tbody tr td:nth-child(2)').innerText.toLowerCase().trim();
        const userCases = Array.from(userDetails.querySelectorAll('table:nth-of-type(2) tbody tr td')).map(td => td.innerText.toLowerCase().trim());

        let showUser = true;

        if (userIDTerm && userID.toLowerCase() !== userIDTerm) {
            showUser = false;
        }

        if (userNameTerm && !userName.includes(userNameTerm)) {
            showUser = false;
        }

        if (userCasesTerm && !userCases.some(caseID => caseID.includes(userCasesTerm))) {
            showUser = false;
        }

        if (showUser) {
            userButton.style.display = 'block'; // Show the accordion button
            userElement.classList.remove('show'); // Ensure the content is hidden
            userButton.classList.add('collapsed'); // Ensure the button is collapsed
        } else {
            userButton.style.display = 'none'; // Hide the accordion button
            userElement.classList.remove('show'); // Ensure the content is hidden
            userButton.classList.add('collapsed'); // Ensure the button is collapsed
        }
    });
}

function filterCaseworkers() {
    const caseworkerIDTerm = document.getElementById('CWIDSearchInput').value.toLowerCase();
    const caseworkers = document.querySelectorAll('#caseworkersAccordion .accordion-collapse');

    caseworkers.forEach(caseworkerElement => {
        const caseworkerID = caseworkerElement.id.split('-')[1];
        const caseworkerButton = caseworkerElement.previousElementSibling;

        let showCaseworker = !caseworkerIDTerm || caseworkerID.toLowerCase() === caseworkerIDTerm;

        if (showCaseworker) {
            caseworkerButton.style.display = 'block'; // Show the accordion button
            caseworkerElement.classList.remove('show'); // Ensure the content is hidden
            caseworkerButton.classList.add('collapsed'); // Ensure the button is collapsed
        } else {
            caseworkerButton.style.display = 'none'; // Hide the accordion button
            caseworkerElement.classList.remove('show'); // Ensure the content is hidden
            caseworkerButton.classList.add('collapsed'); // Ensure the button is collapsed
        }
    });
}