let startDate = null;
let endDate = null;

const calendarBody = document.querySelector('.datepicker-calender');
const monthLabel = document.querySelector('.month-name');
const prevBtn = document.querySelectorAll('.month-selector .arrow')[0];
const nextBtn = document.querySelectorAll('.month-selector .arrow')[1];
const calendarInput = document.getElementById('calendar-input');

let currentDate = new Date();

function formatDisplayDate(date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

function updateInputField() {
    const container = document.getElementById('date-container');
    container.innerHTML = ''; // Réinitialise le contenu du conteneur

    const calendarInput = document.getElementById('calendar-input');
    calendarInput.style.display = 'none';

    if (startDate && !endDate) {
        // Crée un seul input pour la première date
        const input = document.createElement('input');
        input.type = 'text';
        input.classList.add('input');
        input.id = ('input-start');
        input.value = formatDisplayDate(startDate);
        container.appendChild(input);

        addStopPropagation();
        addInputListeners();
    } else if (startDate && endDate) {
        const inputStart = document.createElement('input');
        inputStart.classList.add('input');
        inputStart.id = ('input-start');
        inputStart.type = 'text';
        inputStart.value = formatDisplayDate(startDate);

        container.appendChild(inputStart);

        const paragraph = document.createElement('p');
        paragraph.textContent = 'jusqu\'à';
        container.appendChild(paragraph);

        const inputEnd = document.createElement('input');
        inputEnd.type = 'text';
        inputEnd.classList.add('input');
        inputEnd.id = ('input-end');
        inputEnd.value = formatDisplayDate(endDate);

        container.appendChild(inputEnd);

        addStopPropagation();
        addInputListeners();
    } else {
        // Si aucune date n'est sélectionnée, affiche un paragraphe vide
        const paragraph = document.createElement('p');
        paragraph.textContent = '';
        container.appendChild(paragraph);
    }
}

function addStopPropagation() {
    const calendarPopup = document.getElementById('calendarPopup');
    const calendarInputStart = document.getElementById('input-start');
    const calendarInputEnd = document.getElementById('input-end');

    console.log("calendarInputStart", calendarInputStart);
    if (calendarInputStart){
        calendarInputStart.addEventListener('click', (e) => {
            e.stopPropagation();
            calendarPopup.style.display = 'block';
        });
    }

    console.log("calendarInputEnd", calendarInputEnd);
    if (calendarInputEnd) {
        calendarInputEnd.addEventListener('click', (e) => {
            e.stopPropagation();
            calendarPopup.style.display = 'block';
        });
    }
}

function addInputListeners() {
    const inputStart = document.getElementById('input-start');
    const inputEnd = document.getElementById('input-end');

    if (inputStart) {
        inputStart.addEventListener('input', (e) => {
            const [day, month, year] = e.target.value.split('/');
            startDate = new Date(year, month - 1, day);
            renderCalendar(currentDate);
        });
    }

    if (inputEnd) {
        inputEnd.addEventListener('input', (e) => {
            const [day, month, year] = e.target.value.split('/');
            endDate = new Date(year, month - 1, day);
            renderCalendar(currentDate);
        });
    }
}



function clearSelection() {
    calendarBody.querySelectorAll('button.date').forEach(btn => {
        btn.classList.remove('start-date', 'end-date', 'in-range');
    });
    startDate = null;
    endDate = null;
    updateInputField();
}

function renderCalendar(date) {
    const year = date.getFullYear();
    const month = date.getMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    monthLabel.textContent = date.toLocaleDateString('fr-FR', {
        year: 'numeric',
        month: 'long'
    });

    calendarBody.innerHTML = '';

    for (let i = 1; i <= daysInMonth; i++) {
        const btn = document.createElement('button');
        btn.classList.add('date');
        btn.id = 'calendar-button';

        btn.textContent = i;

        const thisDate = new Date(year, month, i);

        if (startDate && thisDate.getTime() === startDate.getTime()) btn.classList.add('start-date');
        if (endDate && thisDate.getTime() === endDate.getTime()) btn.classList.add('end-date');
        if (startDate && endDate && thisDate > startDate && thisDate < endDate) btn.classList.add('in-range');

        btn.addEventListener('click', () => {

            if (!startDate || (startDate && endDate)) {
                clearSelection();
                startDate = thisDate;
                endDate = null;
            } else {
                if (thisDate < startDate) {
                    endDate = startDate;
                    startDate = thisDate;
                } else {
                    endDate = thisDate;
                }
            }
            updateInputField();
            renderCalendar(currentDate);
        });

        calendarBody.appendChild(btn);
    }
}

prevBtn.addEventListener('click', () => {
    currentDate.setMonth(currentDate.getMonth() - 1);
    renderCalendar(currentDate);
});

nextBtn.addEventListener('click', () => {
    currentDate.setMonth(currentDate.getMonth() + 1);
    renderCalendar(currentDate);
});

renderCalendar(currentDate);

const calendarPopup = document.getElementById('calendarPopup');
// const serviceInput = document.getElementById('serviceInput');


// Ouvrir le popup au clic sur l'input
calendarInput.addEventListener('click', (e) => {
    e.stopPropagation();
    calendarPopup.style.display = 'block';
});

calendarPopup.addEventListener('click', (e) => {
    e.stopPropagation();
});

// serviceInput.addEventListener('click', (e) => {
//     e.stopPropagation();
//     calendarPopup.style.display = 'none';
// });

document.addEventListener('click', (e) => {
    if (!calendarPopup.contains(e.target) && e.target !== calendarInput) {
        calendarPopup.style.display = 'none';
    }
});