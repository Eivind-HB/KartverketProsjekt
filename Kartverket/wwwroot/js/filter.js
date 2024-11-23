document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('caseSearchForm');
    const searchButton = document.getElementById('caseSearchButton');
    const filterButton = document.getElementById('caseFilterButton');
    const filterForm = document.getElementById('caseFilterForm');

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
});
function filterCases() {
    const searchTerm = document.getElementById('caseSearchInput').value.toLowerCase();
    const kommuneNameTerm = document.getElementById('kommuneSearchInput').value.toLowerCase();
    const issueTypeTerm = document.getElementById('issueTypeSearchInput').options[document.getElementById('issueTypeSearchInput').selectedIndex].text.toLowerCase().trim();
    const startDate = document.getElementById('startDateInput').value;
    const endDate = document.getElementById('endDateInput').value;
    const cases = document.querySelectorAll('#casesAccordion .accordion-collapse');

    console.log(`Filtering by: searchTerm=${searchTerm}, kommuneNameTerm=${kommuneNameTerm}, issueTypeTerm=${issueTypeTerm}, startDate=${startDate}, endDate=${endDate}`);

    cases.forEach(caseElement => {
        const caseNo = caseElement.id.split('-')[1];
        const caseButton = caseElement.previousElementSibling;
        const caseDetails = caseElement.querySelector('.issue-location-details');
        const caseKommuneName = caseDetails.querySelector('table:nth-of-type(2) tbody tr td:nth-child(1)').innerText.toLowerCase().trim();
        const caseIssueTypeName = caseDetails.querySelector('table:nth-of-type(1) tbody tr td:nth-child(2)').innerText.toLowerCase().trim();
        const caseDateText = caseDetails.querySelector('table:nth-of-type(1) tbody tr td:nth-child(3)').innerText.trim();

        console.log(`Case ${caseNo}: caseKommuneName=${caseKommuneName}, caseIssueTypeName=${caseIssueTypeName}, caseDate=${caseDateText}`);

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
            caseElement.classList.add('show'); // Ensure the content is shown
            caseButton.classList.remove('collapsed'); // Ensure the button is not collapsed
        } else {
            caseButton.style.display = 'none'; // Hide the accordion button
            caseElement.classList.remove('show'); // Ensure the content is hidden
            caseButton.classList.add('collapsed'); // Ensure the button is collapsed
        }
    });
}